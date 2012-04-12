using Orchard.Data.Migration;
using Orchard.Environment.Extensions;
using OrchardHUN.ModuleProfiles.Models;

namespace OrchardHUN.ModuleProfiles
{
    [OrchardFeature("OrchardHUN.ModuleProfiles")]
    public class Migrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(ModuleProfileRecord).Name,
                table => table
                    .Column<int>("Id", column => column.PrimaryKey().Identity())
                    .Column<string>("Name", column => column.NotNull())
                    .Column<string>("Definition", column => column.NotNull())
                );
            return 1;
        }
    }
}
