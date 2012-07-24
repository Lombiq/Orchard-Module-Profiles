using System.Collections.Generic;
using Orchard;
using OrchardHUN.ModuleProfiles.ViewModels;

namespace OrchardHUN.ModuleProfiles.Services
{
    public interface IModuleProfilesService : IDependency
    {
        void ActivateProfile(List<ModuleViewModel> modules);
        void InverseActivateProfile(List<ModuleViewModel> modules);
    }
}
