using System.Linq;
using System.Web.Mvc;
using System.Collections.Generic;
using Orchard.Data;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using OrchardHUN.ModuleProfiles.Models;

namespace OrchardHUN.ModuleProfiles.Controllers
{
    [OrchardFeature("OrchardHUN.ModuleProfiles")]
    public class AdminController : Controller
    {
        private readonly IRepository<ModuleProfileRecord> _repository;

        public Localizer T { get; set; }

        public AdminController(
            IRepository<ModuleProfileRecord> repository)
        {
            _repository = repository;

            T = NullLocalizer.Instance;
        }

        public ActionResult Index()
        {
            var profilesData = _repository.Table.ToList();
            var profiles = new Dictionary<int, string>();
            foreach (var item in profilesData) profiles.Add(item.Id, item.Name);
            return View(profiles);
        }
    }
}
