using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrchardHUN.ModuleProfiles.ViewModels
{
    public class ProfileListViewModel
    {
        public Dictionary<int, string> ProfileNames { get; set; }
        public ModuleProfileViewModel Current { get; set; }

        public ProfileListViewModel()
        {
            ProfileNames = new Dictionary<int, string>();
        }
    }
}
