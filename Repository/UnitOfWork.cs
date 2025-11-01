using AgroLaboratorio.Data;
using AgroLaboratorio.Repository.Personas;
using AgroLaboratorio.Repository.SolsAnalisis;

namespace AgroLaboratorio.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private readonly IElemQuimicoRep _elemQuimicoRep;
        private readonly IPersonaRep _personaRep;
        private readonly ISolAnalisisRep _solAnalisisRep;

        public UnitOfWork(AppDbContext context,
            IElemQuimicoRep elemQuimicoRep,
            IPersonaRep personaRep,
            ISolAnalisisRep solAnalisisRep)
        {
            _elemQuimicoRep = elemQuimicoRep;
            _personaRep = personaRep;
            _solAnalisisRep = solAnalisisRep;

            _context = context;
        }

        public IElemQuimicoRep ElemQuimico => _elemQuimicoRep;

        public IPersonaRep Persona => _personaRep;

        public ISolAnalisisRep SolAnalisis => _solAnalisisRep;

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
