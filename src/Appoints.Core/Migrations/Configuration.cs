using Appoints.Core.Domain;

namespace Appoints.Core.Migrations
{
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<Appoints.Core.Data.AppointsDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(Appoints.Core.Data.AppointsDbContext context)
        {
            context.Roles.AddOrUpdate(r => r.Name, new Role {Name = RoleNames.Admin});
            context.Roles.AddOrUpdate(r => r.Name, new Role {Name = RoleNames.Customer});
        }
    }
}