using System.Web.Mvc;
using Orchard;
using Orchard.Environment.Extensions;
using Orchard.Localization;

namespace OrchardHUN.ModuleProfiles.Controllers
{
    [OrchardFeature("OrchardHUN.ModuleProfiles")]
    public class AdminController : Controller
    {
        private readonly IOrchardServices _orchardServices;

        public Localizer T { get; set; }

        public AdminController(
            IOrchardServices orchardServices)
        {
            _orchardServices = orchardServices;

            T = NullLocalizer.Instance;
        }

        public ActionResult Index()
        {
            return View();
        }
    }
}
