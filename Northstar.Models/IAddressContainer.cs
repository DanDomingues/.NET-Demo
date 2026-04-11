namespace Northstar.Models
{
    public interface IAddressContainer
    {
        public string? PhoneNumber { get; set; }
        public string? StreetAddress { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? PostalCode { get; set; }

        public void FetchDetails(IAddressContainer container)
        {
            PhoneNumber = container.PhoneNumber;
            StreetAddress = container.StreetAddress;
            City = container.City;
            State = container.State;
            PostalCode = container.PostalCode;
        }
    }

    public interface INamedAddressContainer : IAddressContainer
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public void FetchDetails(INamedAddressContainer container)
        {
            var super = this as IAddressContainer;
            super.FetchDetails(container);
            FirstName = container.FirstName;
            LastName = container.LastName;
        }
    }

    public interface IShippingContainer : INamedAddressContainer
    {
        public string? TrackingNumber { get; set; }
        public string? Carrier { get; set; }

        public void FetchDetails(IShippingContainer container)
        {
            var super = this as INamedAddressContainer;
            super.FetchDetails(container);
            TrackingNumber = container.TrackingNumber;
            Carrier = container.Carrier;
        }
    }
}