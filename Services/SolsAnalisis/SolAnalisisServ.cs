using System.Security.Claims;
using AgroLaboratorio.Exceptions;
using AgroLaboratorio.Models;
using AgroLaboratorio.Repository;
using AgroLaboratorio.Repository.SolsAnalisis;
using AgroLaboratorio.Utils;
using AgroLaboratorio.ViewModels.SolsAnalisis;
using AgroLaboratorio.Common.Results;
using AgroLaboratorio.Data;

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

        public async Task<Result> UpdSolicitudAsync(SolAnalisisVM vm)
        {
            try
            {
                var user = _contextAccessor.HttpContext?.User ?? 
                    throw new InvalidOperationException("Usuario no autenticado. Endpoint debe tener [Authorize]");
                var codUser = user?.FindFirstValue(ClaimTypes.NameIdentifier);

                var incomingModel = ParseVMToModel(vm);

                incomingModel.FechaModif = AppTime.NowLocal();
                incomingModel.CodUserModif = codUser;

                var dbModel = await _solAnalisisRep.FindByCodWithDetails(incomingModel.CodAnalisis);

                //Verificar si existe el modelo en DB
                if (dbModel is null) return Result.Failure(Errors.RscNotFound);

                //Seteamos los valores para la parte Master
                SetMasterEntryValues(dbModel, incomingModel);

                SetDetailsEntryValues(dbModel, incomingModel);

                return Result.Success();
            }
            catch(Exception ex)
            {
                Console.Write(ex.StackTrace);
                return Result.Failure(Errors.InternalServerError);
            }
        }

        private void SetMasterEntryValues(SolAnalisisCab dbModel, SolAnalisisCab incomingModel)
        {
            //Obtenemos el acceso al change tracking del objeto
            var entry = _solAnalisisRep.Set.Entry(dbModel);

            //Reemplazamos los valores actuales por los nuevos
            entry.CurrentValues.SetValues(incomingModel);

            //Marcamos los valores que no requieren actualizarse para que EF Core no los modifique
            entry.Member(nameof(dbModel.FechaAlta)).IsModified = false;
            entry.Member(nameof(dbModel.CodUserAlta)).IsModified = false;
            entry.Member(nameof(dbModel.Fecha)).IsModified = false;

            //Aseguramos de que el estado de actualizacion sea false para los detalles
            entry.Collection(m => m.SolAnalisisDets).IsModified = false;
        }

        private void SetDetailsEntryValues(SolAnalisisCab dbModel, SolAnalisisCab incomingModel)
        {
            var dbDets = dbModel.SolAnalisisDets.ToList();
            var incomingDets = incomingModel.SolAnalisisDets.ToList();

            var dbSet = _solAnalisisRep.DetsSet;

            //Calculamos el número de línea
            int currentNroLinea = dbDets.Count != 0 ? dbDets.Max(d => d.NroLinea) : 1;

            foreach(var incomingDet in incomingDets)
            {
                var existing = dbDets.FirstOrDefault(m => m.NroLinea == incomingDet.NroLinea
                                                            && m.CodElemento == incomingDet.CodElemento);
                if (existing != null)
                {
                    //Asignamos la PK existente al objeto entrante
                    incomingDet.NroLinea = existing.NroLinea;
                    incomingDet.CodAnalisis = existing.CodAnalisis;

                    dbSet.Entry(existing).CurrentValues.SetValues(incomingDet);

                    continue; //Seguimos el bucle
                }
                
                incomingDet.NroLinea = ++currentNroLinea; //Sumamos 1 antes de asignar
                incomingDet.CodAnalisis = dbModel.CodAnalisis;

                dbSet.Add(incomingDet); //Se agrega un nuevo detalle
            }

            //Eliminamos los que no existan
            var incomingDetIds = incomingDets
                .Where(x => x.NroLinea != 0) //Solo los que tengan numero de linea se consideran existentes
                .Select(x => (x.NroLinea, x.CodElemento));

            //Lo que exista en la DB y no exista en la lista entrante debe ser eliminado
            var detsToRemove = dbDets
                .Where(d => !incomingDetIds.Contains((d.NroLinea, d.CodElemento)))
                .ToList();

            dbSet.RemoveRange(detsToRemove);
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
