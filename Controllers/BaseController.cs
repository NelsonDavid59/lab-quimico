using AgroLaboratorio.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AgroLaboratorio.Controllers
{
    public abstract class BaseController : Controller
    {
        protected PageConfig PageConfig;

        protected BaseController(IOptions<PageConfig> pageConfig)
        {
            PageConfig = pageConfig.Value;
        }
    }
}
