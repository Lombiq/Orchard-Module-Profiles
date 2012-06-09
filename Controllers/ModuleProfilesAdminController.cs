using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Orchard.Data;
using Orchard.Environment.Extensions;
using Orchard.Environment.Features;
using Orchard.Localization;
using Orchard.UI.Admin;
using Orchard.UI.Notify;
using OrchardHUN.ModuleProfiles.Models;
using OrchardHUN.ModuleProfiles.ViewModels;

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
        public void CreateProfile()
        {
            var model = new ModuleProfileViewModel();

            if (TryUpdateModel<ModuleProfileViewModel>(model))
            {
                if (_repository.Get(p => p.Name == model.Name) == null)
                {
                    _repository.Create(new ModuleProfileRecord() { Name = model.Name });
                    _repository.Flush();

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
        }

        [HttpPost]
        public void SaveConfiguration()
        {
            var model = new ModuleProfileViewModel();

            if (TryUpdateModel<ModuleProfileViewModel>(model))
            {
                var installedModules = _featureManager.GetAvailableFeatures().Where(f => f.Extension.ExtensionType == "Module");
                var enabledModules = _featureManager.GetEnabledFeatures().Where(f => f.Extension.ExtensionType == "Module");

                foreach (var item in installedModules)
                {
                    model.Modules.Add(new ModuleViewModel()
                    {
                        Name = item.Name,
                        Included = true,
                        Enabled = enabledModules.Contains(item)
                    });
                }

                var record = new ModuleProfileRecord();
                var serializer = new JavaScriptSerializer();

                record.Name = model.Name;
                record.Definition = serializer.Serialize(model.Modules);

                if (record.Definition.Length <= 4000)
                {
                    _repository.Create(record);
                    _repository.Flush();

                    _notifier.Add(NotifyType.Information, T("Successfully saved configuration to profile: {0}.", model.Name)); 
                }
                else
                {
                    _notifier.Add(NotifyType.Error, T("Saving configuration failed: too many modules."));
                }
            }
            else
            {
                _notifier.Add(NotifyType.Error, T("Saving configuration failed."));
            }
        }

        [HttpPost]
        public void ActivateProfile()
        {
            var model = new ModuleProfileViewModel();

            if (TryUpdateModel<ModuleProfileViewModel>(model))
            {
                var serializer = new JavaScriptSerializer();
                var modules = serializer.Deserialize<List<ModuleViewModel>>
                            (_repository.Fetch(p => p.Name == model.Name).FirstOrDefault().Definition);

                _featureManager.EnableFeatures(modules.Where(m => m.Enabled).Select(m => m.Name));
                _featureManager.DisableFeatures(modules.Where(m => !m.Enabled).Select(m => m.Name));

                _notifier.Add(NotifyType.Information, T("Successfully activated profile: {0}.", model.Name));
            }
            else
            {
                _notifier.Add(NotifyType.Error, T("Activating profile failed."));
            }
        }

        [HttpPost]
        public void InverseActivateProfile()
        {
            var model = new ModuleProfileViewModel();

            if (TryUpdateModel<ModuleProfileViewModel>(model))
            {
                var serializer = new JavaScriptSerializer();
                var modules = serializer.Deserialize<List<ModuleViewModel>>
                            (_repository.Fetch(p => p.Name == model.Name).FirstOrDefault().Definition);

                _featureManager.EnableFeatures(modules.Where(m => !m.Enabled).Select(m => m.Name));
                _featureManager.DisableFeatures(modules.Where(m => m.Enabled).Select(m => m.Name));

                _notifier.Add(NotifyType.Information, T("Successfully inverse-activated profile: {0}.", model.Name));
            }
            else
            {
                _notifier.Add(NotifyType.Error, T("Activating profile failed."));
            }
        }

        [HttpDelete]
        public void DeleteProfile()
        {
            var model = new ModuleProfileViewModel();

            if (TryUpdateModel<ModuleProfileViewModel>(model))
            {
                _repository.Delete(_repository.Fetch(p => p.Name == model.Name).FirstOrDefault());
                _repository.Flush();

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
            var model = new ProfileListViewModel();

            if (TryUpdateModel<ProfileListViewModel>(model))
            {
                var serializer = new JavaScriptSerializer();
                var profile = _repository.Fetch(p => p.Name == model.Current.Name).FirstOrDefault();
                var included = model.Current.Modules.Where(m => m.Included);
                profile.Definition = serializer.Serialize(included);

                _repository.Update(profile);
                _repository.Flush();

                _notifier.Add(NotifyType.Information, T("Successfully saved profile: {0}.", model.Current.Name));
            }
            else
            {
                _notifier.Add(NotifyType.Error, T("Saving profile failed.", model.Current.Name));
            }

            return RedirectToAction("Index", new { profileName = model.Current.Name });
        }
    }
}
