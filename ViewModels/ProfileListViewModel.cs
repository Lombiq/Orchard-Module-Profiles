﻿using System.Collections.Generic;

namespace OrchardHUN.ModuleProfiles.ViewModels
{
    public class ProfileListViewModel
    {
        public Dictionary<int, string> ProfileNames { get; set; }
        public Dictionary<string, bool> CurrentProfileStates { get; set; }
        public ModuleProfileViewModel Current { get; set; }

        public ProfileListViewModel()
        {
            ProfileNames = new Dictionary<int, string>();
            CurrentProfileStates = new Dictionary<string, bool>();
        }
    }
}
