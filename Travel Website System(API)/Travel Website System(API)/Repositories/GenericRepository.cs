using Microsoft.EntityFrameworkCore;
using Travel_Website_System_API.Models;

namespace Travel_Website_System_API_.Repositories
{
    public class GenericRepository<TEntity> where TEntity : class
    {


        ApplicationDBContext db;
        public GenericRepository(ApplicationDBContext db)
        {
            this.db = db;
        }

        public List<TEntity> GetAll()
        {
            return db.Set<TEntity>()
                     .ToList();
        }


        public List<TEntity> GetAllWithPagination(int pageNumber, int pageSize)
        {
            return db.Set<TEntity>()
                     .Where(e => EF.Property<bool>(e, "isDeleted") == false)
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

}
}
