using AgroLaboratorio.Services.SolsAnalisis;
using AgroLaboratorio.ViewModels.SolsAnalisis;
using Microsoft.AspNetCore.Mvc;
using AgroLaboratorio.Utils;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Filters;
using AgroLaboratorio.Controllers.ActionFilters;
using Microsoft.AspNetCore.Authorization;

namespace AgroLaboratorio.Controllers
{
    public class SolsAnalisisController : Controller
    {
        private readonly ISolAnalisisServ _service;

        public SolsAnalisisController(ISolAnalisisServ service)
        {
            _service = service;
        }

        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Index()
        {
            var solicitudes = await _service.GetAllSols();
            return View(solicitudes);
        }

        [Authorize(Roles = Roles.Admin)]
        public IActionResult Create()
        {
            var vm = new SolAnalisisVM
            {
                Fecha = AppTime.NowLocal()
            };

            return View(vm);
        }

        [HttpPost]
        [Authorize(Roles = Roles.Admin)]
        [ServiceFilter(typeof(ValidateSolAnalisisFilter))]
        public async Task<IActionResult> Create(SolAnalisisVM vm)
        {
            if (!ModelState.IsValid)
            {
                ApplyDetailsError();

                //Fecha siempre es asignada
                vm.Fecha = AppTime.NowLocal();
                return View(vm);
            }

            try
            {
                await _service.AddSolicitud(vm);

                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ErrorMessages.Save);

                //Fecha siempre es asignada
                vm.Fecha = AppTime.NowLocal();
                return View(vm);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            var vm = await _service.FindForEditByCod(id);

            if (vm == null) 
            {
                return NotFound();
            }

            return View(vm);
        }

        [HttpPost]
        [Authorize(Roles = Roles.Admin)]
        [ServiceFilter(typeof(ValidateSolAnalisisFilter))]
        public async Task<IActionResult> Edit(SolAnalisisVM vm)
        {
            if (!ModelState.IsValid)
            {
                ApplyDetailsError();

                return View(vm);
            }

            try
            {
                vm.Estado = Estados.Pendiente; //Temporal

                await _service.UpdSolicitud(vm);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ErrorMessages.Update);
                return View(vm);
            }
        }

        private void ApplyDetailsError()
        {
            ModelStateEntry? errorDetallesEntry = ModelState["Detalles"];

            if (errorDetallesEntry != null)
            {
                var msg = string.Join("<br>", errorDetallesEntry.Errors.Select(v => v.ErrorMessage));

                ViewBag.DetailsErrorScript = $"showDetailsError('{msg}')";
            }
        }
    }
}
