using AgroLaboratorio.Services.Personas;
using AgroLaboratorio.Utils;
using AgroLaboratorio.ViewModels.Personas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AgroLaboratorio.Controllers
{
    public class PersonasController : BaseController
    {
        private readonly IPersonaServ _service;

        public PersonasController(IPersonaServ personaServ,
            IOptions<PageConfig> pageConfig) : base(pageConfig) 
        {
            _service = personaServ;
        }

        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Index()
        {
            var personas = await _service.GetAllPersona();

            return View(personas);
        }

        [Authorize(Roles = Roles.Admin)]
        public IActionResult CreateForm()
        {
            return PartialView(FormNames.CreatePartial);
        }

        [HttpPost]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Create(PersonaVM vm)
        {
            if (!ModelState.IsValid)
            {
                return PartialView(FormNames.CreatePartial, vm);
            }

            try
            {
                await _service.AddPersona(vm);

                //Flag de éxito
                return Json(new { success = true });
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ErrorMessages.Save);
                return PartialView(FormNames.CreatePartial, vm);
            }
        }

        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> EditForm(int id)
        {
            var vm = await _service.FindForEditByCod(id);

            if (vm is null) return NotFound();

            return PartialView(FormNames.EditPartial, vm);
        }

        [HttpPost]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Edit(PersonaVM vm)
        {
            if (!ModelState.IsValid) 
            {
                return PartialView(FormNames.EditPartial, vm);   
            }

            try
            {
                await _service.UpdPersona(vm);

                return Json(new { success = true });
            }
            catch (Exception ex) 
            {
                ModelState.AddModelError("", ErrorMessages.Update);
                return PartialView(FormNames.EditPartial, vm);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Search([FromBody]SearchReq req)
        {
            try
            {
                int s = PageConfig.SearchSize;

                PersonaListVM result = await _service.SearchPersonas(req.SearchValue ?? "", PageConfig.SearchSize);
                var json = Json(new
                {
                    results = result.Personas.Select(p => new {
                        id = p.CodPersona,
                        text = $"{p.Nombre} {p.Apellido}"
                    }).ToArray()
                });

                return json;
            }
            catch (Exception ex) 
            { 
                return Json(
                    new
                    {
                        results = new []{
                            new {id = "0", text = "Error en la búsqueda."}
                        }
                    });
            }
        }

        public class SearchReq
        {
            public string? SearchValue { get; set; }
        }
    }
}
