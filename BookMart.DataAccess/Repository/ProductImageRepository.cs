using BookMart.DataAccess.Data;
using BookMart.DataAccess.Repository.IRepository;
using BookMart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BookMart.DataAccess.Repository
{
    public class ProductImageRepository : Repository<ProductImage>,IProductImageRepository
    {
        private ApplicationDbContext _db;
        public ProductImageRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        //public void Save()
        //{
        //    _db.SaveChanges();
        //}

        public void Update(ProductImage ProductImage)
        {
            _db.ProductImages.Update(ProductImage);
        }
    }
}
