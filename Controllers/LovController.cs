using AgroLaboratorio.Services.Lovs;
using AgroLaboratorio.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AgroLaboratorio.Controllers
{
    [Route("lov")]
    public class LovController : BaseController
    {
        private readonly LovServ _service;

        public LovController(LovServ service,
            IOptions<PageConfig> config) : base(config)
        {
            _service = service;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Route("{rscName}")]
        public async Task<IActionResult> GetLov([FromRoute] string rscName, [FromBody] LovRequest lovRequest)
        {
            lovRequest.RscName = rscName;
            lovRequest.PageSz = PageConfig.LovPgSize;

            var result = await _service.GetLovResult(lovRequest);

            return Ok(result);
        }
    }
}
