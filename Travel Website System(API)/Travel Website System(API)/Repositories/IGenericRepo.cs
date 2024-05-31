namespace Travel_Website_System_API_.Repositories
{
    // generic repo contains all shared crud for all repos 
    public interface IGenericRepo <TEntity> where TEntity : class
    {
        public List<TEntity> GetAll();
        public TEntity GetById(int id);
        public void Update(TEntity entity);
        public void Delete(int id);
        public void Add(TEntity entity);
        public void Save();
    }
}
