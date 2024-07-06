using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using BookMart.DataAccess.Data;
using BookMart.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace BookMart.DataAccess.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        internal DbSet<T> db_Set;

        public Repository(ApplicationDbContext context)
        {
            _context = context;
            this.db_Set = _context.Set<T>();
            _context.Products.Include(u => u.Category);
        }

        public void Add(T entity)
        {
            db_Set.Add(entity);
        }

        public T Get(Expression<Func<T, bool>> predicate, string? includeProperties = null)
        {
            IQueryable<T> query = db_Set;
            query = query.Where(predicate);
            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var property in includeProperties
                    .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(property);

                }
            }
            return query.FirstOrDefault();
        }

        public IEnumerable<T> GetAll(string? includeProperties = null)
        {
            IQueryable<T> query = db_Set;
            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var property in includeProperties
                    .Split(',' , StringSplitOptions.RemoveEmptyEntries)) {
                    query = query.Include(property);

                }
            }
            return query.ToList();
        }

        public void Remove(T entity)
        {
            db_Set.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            db_Set.RemoveRange(entities);
        }
    }
}
