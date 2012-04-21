namespace OrchardHUN.ModuleProfiles.Models
{
    public class ModuleProfileRecord
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string Definition { get; set; }

        public ModuleProfileRecord()
        {
            Definition = string.Empty;
        }
    }
}
