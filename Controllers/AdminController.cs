using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Orchard.Data;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.UI.Notify;
using OrchardHUN.ModuleProfiles.Models;
using OrchardHUN.ModuleProfiles.ViewModels;
using Orchard.Environment.Features;

namespace OrchardHUN.ModuleProfiles.Controllers
{
    [OrchardFeature("OrchardHUN.ModuleProfiles")]
    public class AdminController : Controller
    {
        private readonly IRepository<ModuleProfileRecord> _repository;
        private readonly IFeatureManager _featureManager;
        private readonly INotifier _notifier;

        public Localizer T { get; set; }

        public AdminController(
            IRepository<ModuleProfileRecord> repository,
            IFeatureManager featureManager,
            INotifier notifier)
        {
            _repository = repository;
            _featureManager = featureManager;
            _notifier = notifier;

            T = NullLocalizer.Instance;
        }

        public ActionResult Index(string currentProfileName = "")
        {
            var profilesData = _repository.Table.ToList();
            var list = new ProfileListViewModel();

            if (profilesData != null)
            {
                foreach (var item in profilesData)
                {
                    list.ProfileNames.Add(item.Id, item.Name);
                }

                if (!string.IsNullOrEmpty(currentProfileName))
                {
                    var serializer = new JavaScriptSerializer();

                    list.Current = new ModuleProfileViewModel()
                    {
                        Name = currentProfileName,
                        ModuleStates = serializer.Deserialize<Dictionary<string, bool>>(profilesData.Find(p => p.Name == currentProfileName).Definition)
                    };

                    var features = _featureManager.GetAvailableFeatures();
                    foreach (var item in features)
                    {
                        list.Current.AvailableModules.Add(item.Name, list.Current.ModuleStates[item.Name]);
                    } 
                }
            }

            return View(list);
        }

        [HttpPost]
        public void CreateNewProfile()
        {
            var model = new ModuleProfileViewModel();

            if (TryUpdateModel<ModuleProfileViewModel>(model))
            {
                _repository.Create(new ModuleProfileRecord() { Name = model.Name });
            }
        }
    }
}
