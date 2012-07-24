using System;
using System.Collections.Generic;
using System.Linq;
using Orchard.Environment.Features;
using OrchardHUN.ModuleProfiles.ViewModels;
using JetBrains.Annotations;

namespace OrchardHUN.ModuleProfiles.Services
{
    [UsedImplicitly]
    public class ModuleProfilesService : IModuleProfilesService
    {
        private readonly IFeatureManager _featureManager;

        public ModuleProfilesService(IFeatureManager featureManager)
        {
            _featureManager = featureManager;
        }

        public void ActivateProfile(List<ModuleViewModel> modules)
        {
            modules = FilterUnavailableModules(modules);
            _featureManager.EnableFeatures(modules.Where(m => m.Enabled).Select(m => m.Name));
            _featureManager.DisableFeatures(modules.Where(m => !m.Enabled).Select(m => m.Name));
        }

        public void InverseActivateProfile(List<ModuleViewModel> modules)
        {
            modules = FilterUnavailableModules(modules);
            _featureManager.EnableFeatures(modules.Where(m => !m.Enabled).Select(m => m.Name));
            _featureManager.DisableFeatures(modules.Where(m => m.Enabled).Select(m => m.Name));
        }

        private List<ModuleViewModel> FilterUnavailableModules(List<ModuleViewModel> modules)
        {
            var installedModulesIds = _featureManager.GetAvailableFeatures().Where(f => f.Extension.ExtensionType == "Module").OrderBy(m => m.Id).Select(m => m.Id);
            modules.RemoveAll(m => !installedModulesIds.Contains(m.Name));

            return modules;
        }
    }
}