using Orchard.Localization;
using Orchard.UI.Navigation;

namespace OrchardHUN.ModuleProfiles
{
    public class AdminMenu : INavigationProvider
    {
        public Localizer T { get; set; }
        public string MenuName { get { return "admin"; } }

        public void GetNavigation(NavigationBuilder builder)
        {
            builder.Add(T("Modules"),
                menu => menu.Add(T("Module Profiles"), "4", item => item.Action("Index", "Admin", new { area = "OrchardHUN.ModuleProfiles" }).LocalNav())
            );
        }
    }
}