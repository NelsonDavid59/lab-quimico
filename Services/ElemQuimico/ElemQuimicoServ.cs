using System.Security.Claims;
using AgroLaboratorio.Models;
using AgroLaboratorio.Repository;
using AgroLaboratorio.Utils;
using AgroLaboratorio.ViewModels.ElemQuimicos;
using Microsoft.AspNetCore.Http.HttpResults;

namespace AgroLaboratorio.Services
{
    public class ElemQuimicoServ : IElemQuimicoServ
    {
        private readonly IElemQuimicoRep _elemQuimRep;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _contextAccessor;

        public ElemQuimicoServ(IUnitOfWork unitOfWork, IHttpContextAccessor contextAccessor)
        {
            _unitOfWork = unitOfWork;
            _elemQuimRep = unitOfWork.ElemQuimico;
            _contextAccessor = contextAccessor;
        }

        public async Task AddElemQuimico(ElemQuimicoVM viewModel)
        {
            var user = _contextAccessor.HttpContext?.User;
            var model = new ElemQuimico();
            model.CodElemento = viewModel.CodElemento;
            model.Descripcion = viewModel.Descripcion;
            model.CodUserAlta = user.FindFirstValue(ClaimTypes.NameIdentifier);
            
            _elemQuimRep.Add(model);

            //Save Changes
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<ElemQuimicoListVM> GetAllElemQuimico()
        {
            var models = await _elemQuimRep.GetAllAsync();

            ElemQuimicoListVM result = new ElemQuimicoListVM();
            
            result.ElemQuimicos = models.Select(ParseModelToDisplayVM)
            .ToList();

            return result;
        }

        public async Task<ElemQuimicoVM?> FindForEditByCod(string codElemento)
        {
            var model = await _elemQuimRep.FindById(codElemento);

            if (model == null) return null;

            var vm = new ElemQuimicoVM
            {
                CodElemento = model.CodElemento,
                Descripcion = model.Descripcion
            };

            return vm;
        }

        public async Task<DisplayElemQuimicoVM?> FindByCod(string codElemento)
        {
            if (string.IsNullOrWhiteSpace(codElemento)) return null;

            var model = await _elemQuimRep.FindById(codElemento);

            if (model == null) return null;

            return ParseModelToDisplayVM(model);
        }

        public async Task UpdElemQuimico(ElemQuimicoVM vm)
        {
            var model = await _elemQuimRep.FindById(vm.CodElemento);

            if (model == null) throw new Exception(ErrorMessages.ResourceNotFound);

            model.Descripcion = vm.Descripcion;

            //EF Core detecta el cambio en el elemento que fue buscado
            await _unitOfWork.SaveChangesAsync();
        }

        private DisplayElemQuimicoVM ParseModelToDisplayVM(ElemQuimico model)
        {
            return new DisplayElemQuimicoVM
            {
                CodElemento = model.CodElemento,
                Descripcion = model.Descripcion,
                CodUserAlta = model.CodUserAlta,
                FechaAlta = model.FechaAlta,
                CodUserModif = model.CodUserModif,
                FechaModif = model.FechaModif
            };
        }
    }
}
