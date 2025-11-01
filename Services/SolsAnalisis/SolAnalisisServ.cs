using System.Security.Claims;
using AgroLaboratorio.Exceptions;
using AgroLaboratorio.Models;
using AgroLaboratorio.Repository;
using AgroLaboratorio.Repository.SolsAnalisis;
using AgroLaboratorio.Utils;
using AgroLaboratorio.ViewModels.SolsAnalisis;

namespace AgroLaboratorio.Services.SolsAnalisis
{
    public class SolAnalisisServ : ISolAnalisisServ
    {
        private readonly ISolAnalisisRep _solAnalisisRep;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _contextAccessor;

        public SolAnalisisServ(ISolAnalisisRep solAnalisisRep, IUnitOfWork unitOfWork, 
            IHttpContextAccessor contextAccessor)
        {
            _solAnalisisRep = solAnalisisRep;
            _unitOfWork = unitOfWork;
            _contextAccessor = contextAccessor;
        }

        public async Task AddSolicitud(SolAnalisisVM vm)
        {
            var user = _contextAccessor.HttpContext?.User;
            var model = ParseVMToModel(vm);

            model.Fecha = AppTime.NowLocal();
            model.Estado = Estados.Pendiente;

            model.FechaAlta = AppTime.NowLocal();
            model.CodUserAlta = user.FindFirstValue(ClaimTypes.NameIdentifier);

            //Asignamos el numero de linea a cada detalle
            foreach (var item in model.SolAnalisisDets.Select((m, i) => new { model = m, index = i}))
            {
                item.model.NroLinea = item.index + 1; //Índice + 1
            }

            _solAnalisisRep.Add(model);

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdSolicitud(SolAnalisisVM vm)
        {
            var user = _contextAccessor.HttpContext?.User;
            var model = ParseVMToModel(vm);

            model.FechaModif = AppTime.NowLocal();
            model.CodUserModif = user.FindFirstValue(ClaimTypes.NameIdentifier);

            await _solAnalisisRep.UpdateAsync(model);

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<SolAnalisisListVM> GetAllSols()
        {
            var list = await _solAnalisisRep.GetAllAsync();

            return new SolAnalisisListVM
            {
                Solicitudes = list.Select(ParseModelToDisplayVM).ToList()
            };
        }

        public async Task<DisplaySolAnalisisVM?> FindByCod(int id)
        {
            var model = await _solAnalisisRep.FindById(id);

            if (model == null) return null;

            return ParseModelToDisplayVM(model);
        }

    public async Task<SolAnalisisVM?> FindForEditByCod(int id)
        {
            var model = await _solAnalisisRep.FindByCodWithDetails(id);

            if (model == null) return null;

            var vm = new SolAnalisisVM
            {
                CodAnalisis = model.CodAnalisis,
                CodCliente = model.CodCliente,
                NombreCliente = model.NombCliente,
                Descripcion = model.Descripcion,
                Estado = model.Estado,
                Fecha = model.Fecha,
                Detalles = 
                    model.SolAnalisisDets.Select(m => new SolAnalisisDetVM
                    {
                        CodAnalisis = m.CodAnalisis,
                        CodElemento = m.CodElemento,
                        DescElemento = m.Descripcion,
                        CodSoluble = m.CodSoluble,
                        DescSolubilidad = m.CodSolubleNavigation?.Descripcion,
                        GarantiaVal = m.GarantiaVal,
                        NroLinea = m.NroLinea
                    }).ToList()
            };

            return vm;
        }

        

        private DisplaySolAnalisisVM ParseModelToDisplayVM(SolAnalisisCab model)
        {
            return new DisplaySolAnalisisVM {
                CodAnalisis = model.CodAnalisis,
                Fecha = model.Fecha,
                NombreCliente = model.NombCliente ?? "",
                Descripcion = model.Descripcion,
                Estado = model.Estado,
                CodUserAlta = model.CodUserAlta
            };
        }

        private SolAnalisisCab ParseVMToModel(SolAnalisisVM vm)
        {
            var details = vm.Detalles
                .Select(vm => new SolAnalisisDet { 
                    NroLinea = vm.NroLinea.GetValueOrDefault(),
                    CodElemento = vm.CodElemento,
                    Descripcion = vm.DescElemento,
                    GarantiaVal = vm.GarantiaVal.GetValueOrDefault(),
                    CodSoluble = vm.CodSoluble
                });

            return new SolAnalisisCab {
                CodAnalisis = vm.CodAnalisis.GetValueOrDefault(),
                CodCliente = vm.CodCliente,
                NombCliente = vm.NombreCliente,
                Descripcion = vm.Descripcion,
                Estado = vm.Estado,
                SolAnalisisDets = details.ToList()
            };
        }
    }
}
