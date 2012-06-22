using Orchard.Data.Migration;
using Orchard.Environment.Extensions;
using OrchardHUN.ModuleProfiles.Models;
using Orchard.Data;
using System.Collections.Generic;
using OrchardHUN.ModuleProfiles.ViewModels;
using System.Web.Script.Serialization;

namespace OrchardHUN.ModuleProfiles
{
    [OrchardFeature("OrchardHUN.ModuleProfiles")]
    public class Migrations : DataMigrationImpl
    {
        private readonly IRepository<ModuleProfileRecord> _repository;

        public Migrations(IRepository<ModuleProfileRecord> repository)
        {
            _repository = repository;
        }

        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(ModuleProfileRecord).Name,
                table => table
                    .Column<int>("Id", column => column.PrimaryKey().Identity())
                    .Column<string>("Name", column => column.NotNull().Unique())
                    .Column<string>("Definition", column => column.NotNull().Unlimited())
                );
            return 1;
        }

        public int UpdateFrom1()
        {
            if (_repository.Get(p => p.Name == "Developer") == null)
            {
                var modules = new List<ModuleViewModel>()
                {
                    new ModuleViewModel()
                    {
                        Name = "Orchard.CodeGeneration",
                        Included = true,
                        Enabled = true
                    },
                    new ModuleViewModel()
                    {
                        Name = "Vandelay.TranslationManager",
                        Included = true,
                        Enabled = true
                    },
                    new ModuleViewModel()
                    {
                        Name = "Gallery",
                        Included = true,
                        Enabled = true
                    },
                    new ModuleViewModel()
                    {
                        Name = "Orchard.Packaging",
                        Included = true,
                        Enabled = true
                    },
                    new ModuleViewModel()
                    {
                        Name = "Piedone.Combinator",
                        Included = true,
                        Enabled = false
                    }
                };

                var model = new ModuleProfileViewModel()
                {
                    Name = "Developer",
                    Modules = modules
                };

                var serializer = new JavaScriptSerializer();

                var profile = new ModuleProfileRecord()
                {
                    Name = model.Name,
                    Definition = serializer.Serialize(model.Modules)
                };

                _repository.Create(profile);
                _repository.Flush();
            }

            if (_repository.Get(p => p.Name == "Production") == null)
            {
                var modules = new List<ModuleViewModel>()
                {
                    new ModuleViewModel()
                    {
                        Name = "Orchard.CodeGeneration",
                        Included = true,
                        Enabled = false
                    },
                    new ModuleViewModel()
                    {
                        Name = "Vandelay.TranslationManager",
                        Included = true,
                        Enabled = false
                    },
                    new ModuleViewModel()
                    {
                        Name = "Gallery",
                        Included = true,
                        Enabled = false
                    },
                    new ModuleViewModel()
                    {
                        Name = "Orchard.Packaging",
                        Included = true,
                        Enabled = false
                    },
                    new ModuleViewModel()
                    {
                        Name = "Piedone.Combinator",
                        Included = true,
                        Enabled = true
                    },
                    new ModuleViewModel()
                    {
                        Name = "Orchard.DesignerTools",
                        Included = true,
                        Enabled = false
                    },
                    new ModuleViewModel()
                    {
                        Name = "Profiling",
                        Included = true,
                        Enabled = false
                    },
                    new ModuleViewModel()
                    {
                        Name = "Orchard.Experimental.WebCommandLine",
                        Included = true,
                        Enabled = false
                    }
                };

                var model = new ModuleProfileViewModel()
                {
                    Name = "Production",
                    Modules = modules
                };

                var serializer = new JavaScriptSerializer();

                var profile = new ModuleProfileRecord()
                {
                    Name = model.Name,
                    Definition = serializer.Serialize(model.Modules)
                };

                _repository.Create(profile);
                _repository.Flush();
            }

            return 2;
        }
    }
}