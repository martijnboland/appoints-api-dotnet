using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using Appoints.Core.Domain;
using Appoints.Core.Migrations;

namespace Appoints.Core.Data
{
    public class AppointsDbContext : DbContext
    {
        public AppointsDbContext() : this("AppointsDb")
        {
        }

        public AppointsDbContext(string connectionStringName)
            : base(connectionStringName)
        {
        }

        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Appointment> Appointments { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Now we're here we might as well initialize our database.
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<AppointsDbContext, Configuration>());

            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            // Remove cascade delete convention because it causes trouble when generating the DB.
            // (Introducing FOREIGN KEY constraint on table may cause cycles or multiple cascade paths.)
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            base.OnModelCreating(modelBuilder);
        }
    }
}