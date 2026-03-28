using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using Demo.DataAccess.IRepository;
using Demo.Models;

namespace Demo.DataAccess
{
    public struct StripeProcessDto
    {
        public IEnumerable<IProductOrderItem> items;
        public int headerId;
        public string area;
        public string page;
        public string sucessAction;
        public string failAction;
        public bool sucessUsesId;
        public bool failUsesId;

        public readonly string SucessFullAction => FormatFullAction(sucessAction, sucessUsesId, headerId);
        public readonly string FailFullAction => FormatFullAction(failAction, failUsesId, headerId);

        public static string FormatFullAction(string action, bool usesId, int id, string idTag = "id")
        {
            return usesId ? $"{action}?{idTag}={id}" : action;
        }
    }

    public static class StripeUtility
    {
        public static IActionResult PromptStripePayment(
            IUnitOfWork unitOfWork, 
            HttpResponse Response,  
            StripeProcessDto dto)
        {
            var options = BuildStripeSessionOptions(dto);
            var service = new SessionService();
            var session = service.Create(options);
            
            Response.Headers.Add("Location", session.Url);

            //We update the session id so that it can be found on confirmation validation
            //As the payment is still to be made, there's no value or need to update it yet
            unitOfWork.OrderHeaderRepository.UpdateSessionID(dto.headerId, session.Id);
            unitOfWork.Save();
            
            return new StatusCodeResult(303);
        }

        private static SessionCreateOptions BuildStripeSessionOptions(StripeProcessDto dto, string domain = "https://localhost:7106")
        {
            return new SessionCreateOptions
            {
                SuccessUrl = $"{domain}/{dto.area}/{dto.page}/{dto.SucessFullAction}",
                CancelUrl = $"{domain}/{dto.area}/{dto.page}/{dto.FailFullAction}",
                LineItems = [.. dto.items.Select(item =>
                {
                    return new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(item.Product.Price * 100),
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Product.Title,
                            }
                        },
                        Quantity = item.Count,
                    };
                })],
                Mode = "payment",
            };
        }
    }
}