﻿using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Travel_Website_System_API.Models;

namespace Travel_Website_System_API_.Repositories
{
    public class GenericRepository<TEntity> where TEntity : class
    {


        ApplicationDBContext db;
        private readonly DbSet<TEntity> _dbSet;

        public GenericRepository(ApplicationDBContext db)
        {
            this.db = db;
            _dbSet = db.Set<TEntity>();
        }

        public List<TEntity> GetAll()
        {
            return db.Set<TEntity>()
                     .ToList();
        }

        // This For Hotels Services Only , will be used in service Component in angular
        public List<TEntity> GetAllWithPagination(int pageNumber, int pageSize)
        {
            return db.Set<TEntity>()
                     .Where(e => EF.Property<bool>(e, "isDeleted") == false)
                     .Skip((pageNumber - 1) * pageSize)
                     .Take(pageSize)
                     .ToList();
        }
     
        public List<Service> GetAllHotelsWithPagination(int pageNumber, int pageSize)
{
            return db.Set<Service>()
                     .Where(s => s.isDeleted == false && s.category.Name == "Hotels")
                     .Skip((pageNumber - 1) * pageSize)
                     .Take(pageSize)
                     .ToList();
        }


        public int GetTotalCount()
        {
            return db.Set<TEntity>().Count();
        }


        public TEntity GetById(int id) {

            return db.Set<TEntity>().Find(id);
        }

        public void Add(TEntity entity)
        {
            db.Set<TEntity>().Add(entity);
        }
        public void Edit(TEntity entity)
        {
            db.Entry(entity).State = EntityState.Modified;
        }

        public void Remove(TEntity entity)
        {
            db.Set<TEntity>().Remove(entity);
        }
        public void Save()
        {
            db.SaveChanges();
        }



        public IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
        {
            return db.Set<TEntity>().Where(predicate);
        }



            public IQueryable<TEntity> FindWithInclude(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includeProperties)
            {
                IQueryable<TEntity> query = _dbSet;
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }
                return query.Where(predicate);
            }

            // دوال أخرى مثل Add، Remove، Save، الخ
        


        //

    }
}
