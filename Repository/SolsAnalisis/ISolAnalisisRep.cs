using AgroLaboratorio.Repository.Base;
using AgroLaboratorio.Models;
using AgroLaboratorio.Data;
using Microsoft.EntityFrameworkCore;

namespace AgroLaboratorio.Repository.SolsAnalisis
{
    public interface ISolAnalisisRep : IRepository<SolAnalisisCab, int>
    {
        public Task<SolAnalisisCab?> FindByCodWithDetails(int codigo);
        public Task UpdateAsync(SolAnalisisCab model);

        public DbSet<SolAnalisisDet> DetsSet { get; } 
    }
}
