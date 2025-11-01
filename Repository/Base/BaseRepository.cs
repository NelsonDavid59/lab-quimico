
using AgroLaboratorio.Data;
using Microsoft.EntityFrameworkCore;

namespace AgroLaboratorio.Repository.Base
{
    public class BaseRepository<TModel, PKType> : IRepository<TModel, PKType> where TModel : class
    {
        private AppDbContext _context;
        private DbSet<TModel> _set;

        public BaseRepository(AppDbContext context)
        {
            _context = context;
            _set = context.Set<TModel>();
        }

        public virtual void Add(TModel model)
        {
            _set.Add(model);
        }

        public virtual void Update(TModel model)
        {
            _set.Update(model);
        }

        public async Task<TModel?> FindById(PKType codigo)
        {
            return await _set.FindAsync(codigo);
        }

        public async Task<ICollection<TModel>> GetAllAsync()
        {
            return await _set.ToListAsync();
        }

        public DbSet<TModel> Set => _set;

        public AppDbContext Context => _context;
    }
}
