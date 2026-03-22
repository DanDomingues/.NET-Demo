using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demo.DataAccess;

namespace Demo.DataAccess.IRepository
{
    public interface IUnitOfWork
    {
        IApplicationUserRepository ApplicationUserRepository { get; }
        ICategoryRepository CategoryRepository { get; }
        IProductRepository ProductRepository { get; }
        IProductImageRepository ProductImagesRepository { get; }
        ICompanyRepository CompanyRepository { get; }
        IShoppingCartItemRepository ShoppingCarts { get; }
        IOrderItemDetailsRepository OrderItemDetailsRepository { get; }
        IOrderHeaderRepository OrderHeaderRepository { get; }

        void Save();
    }
}
