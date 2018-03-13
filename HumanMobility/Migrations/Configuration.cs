using HumanMobility.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace HumanMobility.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<ApplicationDbContext>
    {
        internal const string AdminRole = "AdminRole";
        internal const string UserRole = "UserRole";

        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(ApplicationDbContext context)
        {
            if (!context.Users.Any(x => x.UserName == "tester"))
            {
                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

                var adminRole = new IdentityRole(AdminRole);
                var userRole = new IdentityRole(UserRole);

                roleManager.Create(adminRole);
                roleManager.Create(userRole);

                var testUser = new ApplicationUser { UserName = "tester" };
                var adminUser = new ApplicationUser { UserName = "adminit" };
                var iadminUser = new ApplicationUser { UserName = "iadmin" };

                userManager.Create(testUser, "Ab123456");
                userManager.Create(adminUser, "HumanAdmin 2718281");
                userManager.Create(iadminUser, "HumanAdmin 3141592");

                userManager.AddToRole(testUser.Id, UserRole);
                userManager.AddToRole(adminUser.Id, AdminRole);
                userManager.AddToRole(iadminUser.Id, AdminRole);
            }
        }
    }
}
