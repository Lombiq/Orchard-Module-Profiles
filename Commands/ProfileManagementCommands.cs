using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;
using Orchard.Commands;
using Orchard.Data;
using Orchard.Environment.Extensions;
using Orchard.Environment.Features;
using Orchard.Localization;
using OrchardHUN.ModuleProfiles.Models;
using OrchardHUN.ModuleProfiles.ViewModels;

namespace OrchardHUN.ModuleProfiles.Commands
{
    [OrchardFeature("OrchardHUN.ModuleProfiles")]
    public class ProfileManagementCommands : DefaultOrchardCommandHandler
    {
        private readonly IRepository<ModuleProfileRecord> _repository;
        private readonly IFeatureManager _featureManager;

        public ProfileManagementCommands(
            IRepository<ModuleProfileRecord> repository,
            IFeatureManager featureManager)
        {
            _repository = repository;
            _featureManager = featureManager;

            T = NullLocalizer.Instance;
        }

        [CommandName("moduleprofiles activate")]
        [CommandHelp(@"moduleprofiles activate <ProfileName>")]
        public void ActivateProfile(string profileName)
        {
            Activate(profileName, false);
        }

        [CommandName("modprofs act")]
        [CommandHelp(@"modprofs act <ProfileName>")]
        public void ActivateProfileShort(string profileName)
        {
            Activate(profileName, false);
        }

        [CommandName("moduleprofiles inverse activate")]
        [CommandHelp(@"moduleprofiles inverse activate <ProfileName>")]
        public void InverseActivateProfile(string profileName)
        {
            Activate(profileName, true);
        }

        [CommandName("modprofs inv")]
        [CommandHelp(@"modprofs inv <ProfileName>")]
        public void InverseActivateProfileShort(string profileName)
        {
            Activate(profileName, true);
        }

        [CommandName("moduleprofiles save configuration")]
        [CommandHelp(@"moduleprofiles save configuration <ProfileName>")]
        public void SaveConfiguration(string profileName)
        {
            Save(profileName);
        }

        [CommandName("modprofs save")]
        [CommandHelp(@"modprofs save <ProfileName>")]
        public void SaveConfigurationShort(string profileName)
        {
            Save(profileName);
        }

        [CommandName("moduleprofiles delete")]
        [CommandHelp(@"moduleprofiles delete <ProfileName>")]
        public void DeleteProfile(string profileName)
        {
            Delete(profileName);
        }

        [CommandName("modprofs del")]
        [CommandHelp(@"modprofs del <ProfileName>")]
        public void DeleteProfileShort(string profileName)
        {
            Delete(profileName);
        }

        [CommandName("moduleprofiles list")]
        [CommandHelp(@"moduleprofiles list")]
        public void ListAvailableProfiles()
        {
            ListProfiles();
        }

        [CommandName("modprofs lst")]
        [CommandHelp(@"modprofs lst")]
        public void ListAvailableProfilesShort()
        {
            ListProfiles();
        }

        private void Activate(string profileName, bool inverse)
        {
            var profile = _repository.Fetch(p => p.Name == profileName).FirstOrDefault();

            if (profile != null)
            {
                var modules = new JavaScriptSerializer().Deserialize<List<ModuleViewModel>>(profile.Definition);

                if (inverse)
                {
                    _featureManager.EnableFeatures(modules.Where(m => !m.Enabled).Select(m => m.Name));
                    _featureManager.DisableFeatures(modules.Where(m => m.Enabled).Select(m => m.Name));
                    Context.Output.WriteLine(T("Successfully inverse-activated profile: {0}.", profileName));
                }
                else
                {
                    _featureManager.EnableFeatures(modules.Where(m => m.Enabled).Select(m => m.Name));
                    _featureManager.DisableFeatures(modules.Where(m => !m.Enabled).Select(m => m.Name));
                    Context.Output.WriteLine(T("Successfully activated profile: {0}.", profileName));
                }
            }
            else
            {
                Context.Output.WriteLine(T("Profile {0} not found. The available profiles are:", profileName));
                PrintProfiles();
            }
        }

        private void Save(string profileName)
        {
            if (_repository.Get(p => p.Name == profileName) == null)
            {
                var installedModules = _featureManager.GetAvailableFeatures().Where(f => f.Extension.ExtensionType == "Module");
                var enabledModules = _featureManager.GetEnabledFeatures().Where(f => f.Extension.ExtensionType == "Module");

                var model = new ModuleProfileViewModel() { Name = profileName };
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

                    Context.Output.WriteLine(T("Successfully saved configuration to profile: {0}.", model.Name));
                }
                catch (InvalidOperationException)
                {
                    Context.Output.WriteLine(T("Saving configuration failed: invalid database operation."));
                }
            }
            else Context.Output.WriteLine(T("A profile with this name already exists."));
        }

        private void Delete(string profileName)
        {
            var profile = _repository.Fetch(p => p.Name == profileName).FirstOrDefault();

            if (profile == null)
            {
                Context.Output.WriteLine(T("Profile {0} not found. The available profiles are:", profileName));
                PrintProfiles();
            }
            else
            {
                _repository.Delete(profile);
                _repository.Flush();

                Context.Output.WriteLine(T("Successfully deleted profile: {0}.", profileName));
            }
        }

        private void ListProfiles()
        {
            Context.Output.WriteLine(T("The available profiles are:"));
            PrintProfiles();
        }

        private void PrintProfiles()
        {
            Context.Output.WriteLine(string.Join(", ", _repository.Table.ToList().Select(p => p.Name)));
        }
    }
}
