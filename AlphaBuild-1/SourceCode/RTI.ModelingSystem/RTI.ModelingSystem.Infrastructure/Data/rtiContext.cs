// -----------------------------------------------------------------------
// <copyright file="RtiContext.cs" company="RTI">
// RTI
// </copyright>
// <summary>Rti Context</summary>
// -----------------------------------------------------------------------

using RTI.ModelingSystem.Core.DBModels;
using RTI.ModelingSystem.Core.Models;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace RTI.ModelingSystem.Infrastructure.Data
{
    public partial class RtiContext : DbContext
    {
        static RtiContext()
        {
            Database.SetInitializer<RtiContext>(null);
         
        }

        public RtiContext()
            : base("Name=rtiContext")
        {
            var adapter = (IObjectContextAdapter)this;

            var objectContext = adapter.ObjectContext;

            objectContext.CommandTimeout = 120;
        }

        public DbSet<cond_archive> cond_archive { get; set; }
        public DbSet<customer> customers { get; set; }
        public DbSet<customer_water> customer_water { get; set; }
        public DbSet<resin_products> resin_products { get; set; }
        public DbSet<source> sources { get; set; }
        public DbSet<train> trains { get; set; }
        public DbSet<vessel> vessels { get; set; }
        public DbSet<vessel_historical> vessel_historical { get; set; }
        public DbSet<water_data> water_data { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
          modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

        public System.Data.Entity.DbSet<RTI.ModelingSystem.Core.Models.SystemSettings> SystemSettings { get; set; }
    }
}
