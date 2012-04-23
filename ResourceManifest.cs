using Orchard.Environment.Extensions;
using Orchard.UI.Resources;

namespace OrchardHUN.ModuleProfiles
{
    [OrchardFeature("OrchardHUN.ModuleProfiles")]
    public class ResourceManifest : IResourceManifestProvider
    {
        public void BuildManifests(ResourceManifestBuilder builder)
        {
            var manifest = builder.Add();

            manifest.DefineStyle("ModuleProfiles").SetUrl("orchardhun-moduleprofiles.css");
            manifest.DefineScript("jQuery").SetUrl("jquery-1.7.1.min.js", "jquery-1.7.1.js").SetVersion("1.7.1");
        }
    }
}
