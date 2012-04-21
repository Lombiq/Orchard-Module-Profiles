using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OrchardHUN.ModuleProfiles.ViewModels
{
    public class ModuleProfileViewModel
    {
        [Required]
        public string Name { get; set; }

        public List<ModuleViewModel> Modules { get; set; }

        public ModuleProfileViewModel()
        {
            Modules = new List<ModuleViewModel>();
        }
    }
}