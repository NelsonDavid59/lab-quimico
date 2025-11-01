using AgroLaboratorio.Repository.Base;
using AgroLaboratorio.Models;

namespace AgroLaboratorio.Repository.Personas
{
    public interface IPersonaRep : IRepository<Persona, int>
    {
        //Metodos adicionales

        Task<List<Persona>> SearchByDocumento(string documento, int searchCount);
        Task<List<Persona>> SearchByNombre(string nombre, int searchCount);
    }
}
