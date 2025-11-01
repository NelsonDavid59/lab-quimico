using AgroLaboratorio.Services;
using AgroLaboratorio.Utils;
using AgroLaboratorio.ViewModels.ElemQuimicos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgroLaboratorio.Controllers
{
    public class ElemQuimicosController : Controller
    {
        private readonly IElemQuimicoServ _service;

        public ElemQuimicosController(IElemQuimicoServ service)
        {
            _service = service;
        }

        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Index()
        {
            var result = await _service.GetAllElemQuimico();
            return View(result);
        }

        [Authorize(Roles = Roles.Admin)]
        public IActionResult CreateForm()
        {
            return PartialView(FormNames.CreatePartial);
        }

        [HttpPost]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Create(ElemQuimicoVM model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView(FormNames.CreatePartial, model);
            }

            try
            {
                await _service.AddElemQuimico(model);

                //Flag de éxito
                return Json(new { success = true });
            }
            catch (Exception ex) 
            {
                ModelState.AddModelError("", ErrorMessages.Save);
                return PartialView(FormNames.CreatePartial, model);
            }
        }

        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> EditForm(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return NotFound();

            var vm = await _service.FindForEditByCod(id);

            if (vm is null) return NotFound();

            return PartialView(FormNames.EditPartial, vm);
        }

        [HttpPost]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Edit(ElemQuimicoVM model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView(FormNames.EditPartial, model);
            }

            try
            {
                await _service.UpdElemQuimico(model);

                return Json(new { success = true });
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ErrorMessages.Update);
                return PartialView(FormNames.EditPartial, model);
            }
        }
    }
}
