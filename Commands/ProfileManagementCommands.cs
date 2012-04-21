﻿using System.Collections.Generic;
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

        [CommandName("moduleprofiles inverse activate")]
        [CommandHelp(@"moduleprofiles inverse activate <ProfileName>")]
        public void InverseActivateProfile(string profileName)
        {
            Activate(profileName, true);
        }

        private void Activate(string profileName, bool inverse)
        {
            var profile = _repository.Fetch(p => p.Name == profileName).FirstOrDefault();

            if (profile != null)
            {
                var serializer = new JavaScriptSerializer();
                var modules = serializer.Deserialize<List<ModuleViewModel>>(profile.Definition);

                if (inverse)
                {
                    _featureManager.EnableFeatures(modules.Where(m => !m.Enabled).Select(m => m.Name));
                    _featureManager.DisableFeatures(modules.Where(m => m.Enabled).Select(m => m.Name));
                    Context.Output.WriteLine(T("\nSuccessfully inverse-activated profile: {0}.", profileName));
                }
                else
                {
                    _featureManager.EnableFeatures(modules.Where(m => m.Enabled).Select(m => m.Name));
                    _featureManager.DisableFeatures(modules.Where(m => !m.Enabled).Select(m => m.Name));
                    Context.Output.WriteLine(T("\nSuccessfully activated profile: {0}.", profileName));
                }
            }
            else
            {
                Context.Output.WriteLine(T("\nProfile not found. The available profiles are:"));
                Context.Output.WriteLine(string.Join(", ", _repository.Table.ToList().Select(p => p.Name)));
            }
        }
    }
}
