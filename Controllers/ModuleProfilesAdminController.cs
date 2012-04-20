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
using Orchard.UI.Admin;

namespace OrchardHUN.ModuleProfiles.Controllers
{
    [Admin]
    [OrchardFeature("OrchardHUN.ModuleProfiles")]
    public class ModuleProfilesAdminController : Controller
    {
        private readonly IRepository<ModuleProfileRecord> _repository;
        private readonly IFeatureManager _featureManager;
        private readonly INotifier _notifier;

        public Localizer T { get; set; }

        public ModuleProfilesAdminController(
            IRepository<ModuleProfileRecord> repository,
            IFeatureManager featureManager,
            INotifier notifier)
        {
            _repository = repository;
            _featureManager = featureManager;
            _notifier = notifier;

            T = NullLocalizer.Instance;
        }

        public ActionResult Index(string profileName = "")
        {
            var profilesData = _repository.Table.ToList();
            var model = new ProfileListViewModel();

            if (profilesData != null || profilesData.Count > 0)
            {
                foreach (var item in profilesData)
                {
                    model.ProfileNames.Add(item.Id, item.Name);
                }

                if (!string.IsNullOrEmpty(profileName) && model.ProfileNames.ContainsValue(profileName))
                {
                    var serializer = new JavaScriptSerializer();
                    model.Current = new ModuleProfileViewModel()
                    {
                        Name = profileName,
                        Modules = serializer.Deserialize<List<ModuleViewModel>>
                            (profilesData.Find(p => p.Name == profileName).Definition)
                            ?? new List<ModuleViewModel>()
                    };

                    var installedModules = _featureManager.GetAvailableFeatures().Where(f => f.Extension.ExtensionType == "Module");
                    foreach (var item in installedModules)
                    {
                        if (model.Current.Modules.Find(m => m.Name == item.Id) == null)
                        {
                            model.Current.Modules.Add(new ModuleViewModel() { Name = item.Id });
                        }
                    }
                }
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult CreateProfile()
        {
            var model = new ModuleProfileViewModel();

            if (TryUpdateModel<ModuleProfileViewModel>(model))
            {
                if (_repository.Get(p => p.Name == model.Name) == null)
                {
                    _repository.Create(new ModuleProfileRecord() { Name = model.Name });
                    _notifier.Add(NotifyType.Information, T("Successfully created profile: {0}.", model.Name));
                }
                else
                {
                    _notifier.Add(NotifyType.Error, T("A profile with this name already exists."));
                }
            }
            else
            {
                _notifier.Add(NotifyType.Error, T("Creating profile failed: {0}.", model.Name));
            }

            return RedirectToAction("Index", new { profileName = model.Name });
        }

        [HttpDelete]
        public void DeleteProfile()
        {
            var model = new ModuleProfileViewModel();

            if (TryUpdateModel<ModuleProfileViewModel>(model))
            {
                _repository.Delete(_repository.Fetch(p => p.Name == model.Name).FirstOrDefault());
                _notifier.Add(NotifyType.Information, T("Successfully deleted profile: {0}.", model.Name));
            }
            else
            {
                if (string.IsNullOrEmpty(model.Name))
                {
                    _notifier.Add(NotifyType.Error, T("No profile selected."));
                }
                else
                {
                    _notifier.Add(NotifyType.Error, T("Deleting profile failed: {0}.", model.Name));
                }
            }
        }

        [HttpPost]
        public ActionResult SaveProfile()
        {
            var model = new ModuleProfileViewModel();

            if (TryUpdateModel<ModuleProfileViewModel>(model))
            {
                _notifier.Add(NotifyType.Information, T("Successfully saved profile: {0}.", model.Name));
            }
            else
            {
                _notifier.Add(NotifyType.Error, T("Saving profile failed.", model.Name));
            }

            return RedirectToAction("Index", new { profileName = model.Name });
        }
    }
}
