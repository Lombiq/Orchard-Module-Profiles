using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrchardHUN.ModuleProfiles.ViewModels
{
    public class ModuleProfileViewModel
    {
        public string Name { get; set; }
        public Dictionary<string, bool> Definition { get; set; }
    }
}
