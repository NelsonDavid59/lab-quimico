using AgroLaboratorio.Repository.Base;
using AgroLaboratorio.Models;

namespace AgroLaboratorio.Repository.SolsAnalisis
{
    public interface ISolAnalisisRep : IRepository<SolAnalisisCab, int>
    {
        public Task<SolAnalisisCab?> FindByCodWithDetails(int codigo);
        public Task UpdateAsync(SolAnalisisCab model);
    }
}
