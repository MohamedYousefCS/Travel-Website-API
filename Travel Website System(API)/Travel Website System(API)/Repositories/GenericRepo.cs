using Travel_Website_System_API.Models;

namespace Travel_Website_System_API_.Repositories
{
    public class GenericRepo <TEntity> : IGenericRepo<TEntity> where TEntity : class
    {
        ApplicationDBContext db;
        public GenericRepo(ApplicationDBContext db)
        {
            this.db = db;
        }
        // TEntity : is replaced with each Model class which represents Dbset in DBContext
        public List<TEntity> GetAll()
        {
            return db.Set<TEntity>().ToList(); // db.set<Student>().ToList;, Student : Model Class
        }
        public TEntity GetById(int id)
        {
            return db.Set<TEntity>().Find(id);
        }

        public void Update(TEntity entity)
        {
            //db.Set<TEntity>().Update(entity);ex: === db.students.Update(student)
            db.Entry(entity).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
        }
        public void Add(TEntity entity)
        {
            db.Set<TEntity>().Add(entity);
        }
        // this is hard delete
        public void Delete(int id)
        {
            db.Set<TEntity>().Remove(GetById(id));// take entity
        }

        public void Save()
        {
            db.SaveChanges();
        }

    }
}

