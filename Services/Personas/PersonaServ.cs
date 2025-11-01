using AgroLaboratorio.Repository;
using AgroLaboratorio.Repository.Personas;
using AgroLaboratorio.ViewModels.Personas;
using AgroLaboratorio.Models;
using System.Security.Claims;
using AgroLaboratorio.Utils;
using System.Text.RegularExpressions;

namespace AgroLaboratorio.Services.Personas
{
    public class PersonaServ : IPersonaServ
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPersonaRep _personaRep;
        private readonly IHttpContextAccessor _contextAccessor;

        public PersonaServ(IUnitOfWork unitOfWork, IHttpContextAccessor contextAccessor)
        {
            _unitOfWork = unitOfWork;
            _personaRep = unitOfWork.Persona;
            _contextAccessor = contextAccessor;
        }

        public async Task AddPersona(PersonaVM vm)
        {
            var user = _contextAccessor.HttpContext?.User;
            var model = new Persona
            {
                Nombre = vm.Nombre,
                Apellido = vm.Apellido,
                Documento = vm.Documento,
                TipoPersona = TipoPersona.Cliente,
                CodUserAlta = user.FindFirstValue(ClaimTypes.NameIdentifier)
            };

            _personaRep.Add(model);

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<DisplayPersonaVM?> FindByCod(int codPersona)
        {
            var model = await _personaRep.FindById(codPersona);

            if (model == null) return null;

            return ParseModelToDisplayVM(model);
        }

        public async Task<PersonaVM?> FindForEditByCod(int codPersona)
        {
            var model = await _personaRep.FindById(codPersona);

            if (model == null) return null;

            var vm = new PersonaVM
            {
                CodPersona = model.CodPersona,
                Nombre = model.Nombre,
                Apellido = model.Apellido
            };

            return vm;
        }

        public async Task<PersonaListVM> GetAllPersona()
        {
            var models = await _personaRep.GetAllAsync();

            PersonaListVM result = new PersonaListVM();

            result.Personas = models.Select(p => ParseModelToDisplayVM(p))
                                    .ToList();

            return result;
        }

        public async Task UpdPersona(PersonaVM vm)
        {
            var user = _contextAccessor.HttpContext?.User;
            var model = await _personaRep.FindById(vm.CodPersona.Value);

            if (model == null) throw new Exception(ErrorMessages.ResourceNotFound);

            model.Nombre = vm.Nombre;
            model.Apellido = vm.Apellido;
            model.Documento = vm.Documento;
            model.CodUserModif = user.FindFirstValue(ClaimTypes.NameIdentifier);
            //model.FechaModif = 

        }

        public async Task<PersonaListVM> SearchPersonas(string value, int count)
        {
            //Evaluamos si el valor es un Documento o un Nombre
            List<Persona> lista = EsDocumento(value) ? 
                                    await _personaRep.SearchByDocumento(value, count) :
                                    await _personaRep.SearchByNombre(value, count);



            return
                new PersonaListVM
                {
                    Personas = lista.Select(ParseModelToDisplayVM).ToList()
                };
        }

        private DisplayPersonaVM ParseModelToDisplayVM (Persona model)
        {
            return new DisplayPersonaVM
            {
                CodPersona = model.CodPersona,
                Nombre = model.Nombre,
                Apellido = model.Apellido,
                Documento = model.Documento,
                CodUserAlta = model.CodUserAlta,
                FechaAlta = model.FechaAlta,
                CodUseModif = model.CodUserModif,
                FechaModif = model.FechaModif
            };
        }

        /* EVALUA SI UN STRING ES UN DOCUMENTO */
        private bool EsDocumento(string value)
        {
            //Pattern: CI (secuencia de digitos).
            var ciPattern = @"^\d+$";

            return Regex.IsMatch(value, ciPattern);
        }
    }
}
