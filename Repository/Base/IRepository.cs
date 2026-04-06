using Microsoft.EntityFrameworkCore;

namespace AgroLaboratorio.Repository.Base
{
    public interface IRepository<TModel, PKType> where TModel : class
    {
        /* OPERACIONES BASE */
        public void Add(TModel model);

        public void Update(TModel model);

        public Task<TModel?> FindById(PKType codigo);

        public Task<ICollection<TModel>> GetAllAsync();

        public DbSet<TModel> Set { get; }
        public DbContext Context { get; }
    }
}
