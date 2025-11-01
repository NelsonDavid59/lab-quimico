using AgroLaboratorio.Repository.Base;
using AgroLaboratorio.Models;
using AgroLaboratorio.Data;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace AgroLaboratorio.Repository.Personas
{
    public class PersonaRep : BaseRepository<Persona, int>, IPersonaRep
    {
        public PersonaRep(AppDbContext context) : base(context)
        {

        }

        /**
         * Búsqueda de Personas.
         * Value: Documento
         * */
        public async Task<List<Persona>> SearchByDocumento(string documento, int searchCount)
        {
            var query = Set.Where(p => p.Documento != null && p.Documento.Contains(documento));

            var result = await query
                            .OrderBy(p => p.Nombre)
                            .Take(searchCount)
                            .Select(p => 
                                new Persona
                                {
                                    CodPersona = p.CodPersona,
                                    Nombre = p.Nombre,
                                    Apellido = p.Apellido
                                }
                             )
                            .ToListAsync();

            return result;
        }

        public async Task<List<Persona>> SearchByNombre(string nombre, int searchCount)
        {
            var query = Set.Where(p => EF.Functions.ILike(p.Nombre + " " + p.Apellido, $"%{nombre}%"));

            var result = await query
                .OrderBy(p => p.Nombre)
                .Take(searchCount)
                .Select(p => 
                    new Persona 
                    {
                        CodPersona = p.CodPersona,
                        Nombre = p.Nombre,
                        Apellido = p.Apellido
                    })
                .ToListAsync();

            return result;
        }

    }
}
