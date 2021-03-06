﻿using System;
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
using OrchardHUN.ModuleProfiles.Services;

namespace OrchardHUN.ModuleProfiles.Controllers
{
    [Admin]
    [OrchardFeature("OrchardHUN.ModuleProfiles")]
    public class ModuleProfilesAdminController : Controller
    {
        private readonly IRepository<ModuleProfileRecord> _repository;
        private readonly IFeatureManager _featureManager;
        private readonly IModuleProfilesService _moduleProfilesService;
        private readonly INotifier _notifier;

        public Localizer T { get; set; }

        public ModuleProfilesAdminController(
            IRepository<ModuleProfileRecord> repository,
            IFeatureManager featureManager,
            IModuleProfilesService moduleProfilesService,
            INotifier notifier)
        {
            _repository = repository;
            _featureManager = featureManager;
            _moduleProfilesService = moduleProfilesService;
            _notifier = notifier;

            T = NullLocalizer.Instance;
        }

        public ActionResult Index(string profileName = "")
        {
            var profilesData = _repository.Table.ToList();
            var model = new ProfileListViewModel();

            if (profilesData != null || profilesData.Count > 0)
            {
                foreach (var item in profilesData) model.ProfileNames.Add(item.Id, item.Name);

                if (!string.IsNullOrEmpty(profileName) && model.ProfileNames.ContainsValue(profileName))
                {
                    model.Current = new ModuleProfileViewModel()
                    {
                        Name = profileName,
                        Modules = (new JavaScriptSerializer().Deserialize<List<ModuleViewModel>>
                        (profilesData.Find(p => p.Name == profileName).Definition)
                            ?? new List<ModuleViewModel>()).OrderBy(m => m.Enabled).ThenBy(m => m.Name).ToList()
                    };

                    var installedModulesIds = _featureManager.GetAvailableFeatures().Where(f => f.Extension.ExtensionType == "Module").OrderBy(m => m.Id).Select(m => m.Id);
                    model.Current.Modules.RemoveAll(m => !installedModulesIds.Contains(m.Name));
                    foreach (var item in installedModulesIds)
                    {
                        if (model.Current.Modules.Find(m => m.Name == item) == null)
                        {
                            model.Current.Modules.Add(new ModuleViewModel() { Name = item });
                        }
                    }

                    var enabledModulesIds = _featureManager.GetEnabledFeatures().Where(f => f.Extension.ExtensionType == "Module").Select(m => m.Id);
                    foreach (var item in model.Current.Modules) item.State = enabledModulesIds.Contains(item.Name);
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
                else _notifier.Add(NotifyType.Error, T("A profile with this name already exists."));
            }
            else _notifier.Add(NotifyType.Error, T("Creating profile failed: {0}.", model.Name));
        }

        [HttpPost]
        public void SaveConfiguration()
        {
            var model = new ModuleProfileViewModel();

            if (TryUpdateModel<ModuleProfileViewModel>(model))
            {
                if (_repository.Get(p => p.Name == model.Name) == null)
                {
                    var installedModules = _featureManager.GetAvailableFeatures().Where(f => f.Extension.ExtensionType == "Module");
                    var enabledModules = _featureManager.GetEnabledFeatures().Where(f => f.Extension.ExtensionType == "Module");
                    foreach (var item in installedModules)
                    {
                        model.Modules.Add(new ModuleViewModel()
                        {
                            Name = item.Id,
                            Included = true,
                            Enabled = enabledModules.Contains(item)
                        });
                    }

                    try
                    {
                        _repository.Create(new ModuleProfileRecord() { Name = model.Name, Definition = new JavaScriptSerializer().Serialize(model.Modules) });
                        _repository.Flush();

                        _notifier.Add(NotifyType.Information, T("Successfully saved configuration to profile: {0}.", model.Name));
                    }
                    catch (InvalidOperationException)
                    {
                        _notifier.Add(NotifyType.Error, T("Saving configuration failed: invalid database operation."));
                    }
                }
                else _notifier.Add(NotifyType.Error, T("A profile with this name already exists."));
            }
            else _notifier.Add(NotifyType.Error, T("Saving configuration failed."));
        }

        [HttpPost]
        public void ActivateProfile()
        {
            var model = new ModuleProfileViewModel();

            if (TryUpdateModel<ModuleProfileViewModel>(model))
            {
                Activate(model, false);

                _notifier.Add(NotifyType.Information, T("Successfully activated profile: {0}.", model.Name));
            }
            else _notifier.Add(NotifyType.Error, T("Activating profile failed."));
        }

        [HttpPost]
        public void InverseActivateProfile()
        {
            var model = new ModuleProfileViewModel();

            if (TryUpdateModel<ModuleProfileViewModel>(model))
            {
                Activate(model, true);

                _notifier.Add(NotifyType.Information, T("Successfully inverse-activated profile: {0}.", model.Name));
            }
            else _notifier.Add(NotifyType.Error, T("Activating profile failed."));
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
                if (string.IsNullOrEmpty(model.Name)) _notifier.Add(NotifyType.Error, T("No profile selected."));
                else _notifier.Add(NotifyType.Error, T("Deleting profile failed: {0}.", model.Name));
            }
        }

        [HttpPost]
        public ActionResult SaveProfile()
        {
            var model = new ProfileListViewModel();

            if (TryUpdateModel<ProfileListViewModel>(model))
            {
                var profile = _repository.Fetch(p => p.Name == model.Current.Name).FirstOrDefault();
                var included = model.Current.Modules.Where(m => m.Included);
                profile.Definition = new JavaScriptSerializer().Serialize(included);

                try
                {
                    _repository.Create(profile);
                    _repository.Flush();

                    _notifier.Add(NotifyType.Information, T("Successfully saved profile: {0}.", model.Current.Name));
                }
                catch (InvalidOperationException)
                {
                    _notifier.Add(NotifyType.Error, T("Saving profile failed: invalid operation."));
                }
            }
            else _notifier.Add(NotifyType.Error, T("Saving profile failed.", model.Current.Name));

            return RedirectToAction("Index", new { profileName = model.Current.Name });
        }

        private void Activate(ModuleProfileViewModel profile, bool inverse)
        {
            var modules = new JavaScriptSerializer().Deserialize<List<ModuleViewModel>>
                    (_repository.Fetch(p => p.Name == profile.Name).FirstOrDefault().Definition);

            if (inverse) _moduleProfilesService.InverseActivateProfile(modules);
            else _moduleProfilesService.ActivateProfile(modules);
        }
    }
}