using AgroLaboratorio.Repository.Personas;
using AgroLaboratorio.Repository.SolsAnalisis;

namespace AgroLaboratorio.Repository
{
    public interface IUnitOfWork : IDisposable
    {
        IElemQuimicoRep ElemQuimico { get; }
        IPersonaRep Persona { get; }
        ISolAnalisisRep SolAnalisis { get; }

        Task<int> SaveChangesAsync();
    }
}
