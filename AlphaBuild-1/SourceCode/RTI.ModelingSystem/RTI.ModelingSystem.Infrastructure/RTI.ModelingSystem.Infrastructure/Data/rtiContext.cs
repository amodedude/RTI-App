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
using System.Linq;

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

        // This over ride for 'SaveChanges' allows the system to unwrap the actual error in order to display a useful error message the user/administrator
        public override int SaveChanges()  
        {
            try
            {
                return base.SaveChanges();
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                // Retrieve the error messages as a list of strings.
                var errorMessages = ex.EntityValidationErrors.SelectMany(x => x.ValidationErrors).Select(x => x.ErrorMessage);

                // Join the list to a single string.
                var fullErrorMessage = string.Join("; ", errorMessages);

                // Combine the original exception message with the new one.
                var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);

                // Throw a new DbEntityValidationException with the improved exception message.
                throw new System.Data.Entity.Validation.DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
            }
        }
    }
}
