using AgroLaboratorio.ViewModels.Personas;
using AgroLaboratorio.ViewModels.SolsAnalisis;

namespace AgroLaboratorio.Services.Personas
{
    public interface IPersonaServ
    {
        Task AddPersona(PersonaVM vm);

        Task UpdPersona(PersonaVM vm);

        Task<PersonaListVM> GetAllPersona();

        Task<PersonaVM?> FindForEditByCod(int codPersona);

        Task<DisplayPersonaVM?> FindByCod(int codPersona);

        public Task<PersonaListVM> SearchPersonas(string value, int count);
    }
}
