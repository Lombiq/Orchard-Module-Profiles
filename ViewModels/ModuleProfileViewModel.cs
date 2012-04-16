using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace OrchardHUN.ModuleProfiles.ViewModels
{
    public class ModuleProfileViewModel
    {
        [Required]
        public string Name { get; set; }

        public Dictionary<string, bool> AvailableModules { get; set; }
        public Dictionary<string, bool> ModuleStates { get; set; }

        public ModuleProfileViewModel()
        {
            Name = string.Empty;
            AvailableModules = new Dictionary<string, bool>();
            ModuleStates = new Dictionary<string, bool>();
        }
    }
}
