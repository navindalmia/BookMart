using BookMart.DataAccess.Data;
using BookMart.DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace BookMart.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        public ICategoryRepository CategoryRepository { private set; get; }
        public IProductRepository ProductRepository { private set; get; }
        public ICompanyRepository CompanyRepository { private set; get; }
        public IShoppingCartRepository ShoppingCartRepository { private set; get; }

        public IApplicationUserRepository ApplicationUserRepository { private set; get; }
        public IOrderHeaderRepository OrderHeaderRepository { private set; get; }
        public IOrderDetailRepository OrderDetailRepository { private set; get; }


        private ApplicationDbContext _db;
        public UnitOfWork(ApplicationDbContext db) 
        {
            _db = db;
            CategoryRepository = new CategoryRepository(_db);
            ProductRepository = new ProductRepository(_db);
            CompanyRepository = new CompanyRepository(_db);
            ShoppingCartRepository = new ShoppingCartRepository(_db);
            ApplicationUserRepository = new ApplicationUserRepository(_db);
            OrderHeaderRepository = new OrderHeaderRepository(_db);
            OrderDetailRepository = new OrderDetailRepository(_db);
        }
        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
