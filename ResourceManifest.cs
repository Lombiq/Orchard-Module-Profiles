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
        }
    }
}
