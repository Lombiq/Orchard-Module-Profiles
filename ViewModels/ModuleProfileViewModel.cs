using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrchardHUN.ModuleProfiles.ViewModels
{
    class ModuleProfileViewModel
    {
        public string Name { get; set; }
        public List<string> ModuleNames { get; set; }
        public List<bool> ModuleStates { get; set; }
    }
}
