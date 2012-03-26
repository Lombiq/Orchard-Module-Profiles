using Orchard.Commands;
using Orchard.Environment.Extensions;
using Orchard.Environment.Features;
using Orchard.Localization;
using Orchard.Modules.Services;
using System.Collections.Generic;

namespace OrchardHUN.ModuleProfiles.Commands
{
    [OrchardFeature("OrchardHUN.ModuleProfiles")]
    public class ProfileManagementCommands : DefaultOrchardCommandHandler
    {
        private class ModuleSwitcher
        {
            public string ModuleName { get; set; }
            public bool Enable { get; set; }

            public ModuleSwitcher(string moduleName, bool enable)
            {
                ModuleName = moduleName;
                Enable = enable;
            }
        }

        private readonly IFeatureManager _featureManager;

        public ProfileManagementCommands(
            IFeatureManager featureManager)
        {

            _featureManager = featureManager;

            T = NullLocalizer.Instance;
        }

        [CommandName("moduleprofiles activate")]
        [CommandHelp(@"moduleprofiles activate <ProfileName>")]
        public void ActivateProfile(string profileName)
        {
            List<ModuleSwitcher> switcher = new List<ModuleSwitcher>();
            switch (profileName)
            {
                case "Developer":
                    _featureManager.EnableFeatures(new List<string>()
                        {
                            "Vandelay.TranslationManager",
                            "Orchard.CodeGeneration",
                        }, true);
                    _featureManager.DisableFeatures(new List<string>()
                        {
                            "Piedone.Combinator"
                        }, true);
                    Context.Output.WriteLine(T("Profile \"{0}\" activated.", profileName));
                    break;
                case "Production":
                    _featureManager.EnableFeatures(new List<string>()
                        {
                            "Piedone.Combinator"
                        }, true);
                    _featureManager.DisableFeatures(new List<string>()
                        {
                            "Vandelay.TranslationManager",
                            "Orchard.DesignerTools",
                            "Orchard.CodeGeneration",
                            "Profiling",
                            "Orchard.Experimental.WebCommandLine",
                            "Orchard.Packaging",
                            "Four2n.MiniProfiler"
                        }, true);
                    Context.Output.WriteLine(T("Profile \"{0}\" activated.", profileName));
                    break;
                default:
                    Context.Output.WriteLine(T("Profile \"{0}\" not found.", profileName));
                    break;
            }
        }
    }
}
