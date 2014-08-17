namespace Appoints.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Appointment",
                c => new
                     {
                         Id = c.Int(nullable: false, identity: true),
                         Title = c.String(nullable: false),
                         StartDateAndTime = c.DateTime(nullable: false),
                         EndDateAndTime = c.DateTime(nullable: false),
                         Remarks = c.String(),
                         UserId = c.Int(nullable: false),
                     })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.User", t => t.UserId)
                .Index(t => t.UserId);

            CreateTable(
                "dbo.User",
                c => new
                     {
                         Id = c.Int(nullable: false, identity: true),
                         Provider = c.String(nullable: false),
                         ProviderUserId = c.String(nullable: false),
                         Email = c.String(nullable: false),
                         DisplayName = c.String(nullable: false),
                         ProviderAccessToken = c.String(),
                         ProviderRefreshToken = c.String(),
                         Created = c.DateTime(nullable: false),
                         LastAuthenticated = c.DateTime(),
                     })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.UserRole",
                c => new
                     {
                         UserId = c.Int(nullable: false),
                         RoleId = c.Int(nullable: false),
                     })
                .PrimaryKey(t => new {t.UserId, t.RoleId})
                .ForeignKey("dbo.Role", t => t.RoleId)
                .ForeignKey("dbo.User", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);

            CreateTable(
                "dbo.Role",
                c => new
                     {
                         Id = c.Int(nullable: false, identity: true),
                         Name = c.String(nullable: false),
                     })
                .PrimaryKey(t => t.Id);
        }

        public override void Down()
        {
            DropForeignKey("dbo.Appointment", "UserId", "dbo.User");
            DropForeignKey("dbo.UserRole", "UserId", "dbo.User");
            DropForeignKey("dbo.UserRole", "RoleId", "dbo.Role");
            DropIndex("dbo.UserRole", new[] {"RoleId"});
            DropIndex("dbo.UserRole", new[] {"UserId"});
            DropIndex("dbo.Appointment", new[] {"UserId"});
            DropTable("dbo.Role");
            DropTable("dbo.UserRole");
            DropTable("dbo.User");
            DropTable("dbo.Appointment");
        }
    }
}