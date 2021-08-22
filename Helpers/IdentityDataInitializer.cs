// Modified code from: http://www.binaryintellect.net/articles/5e180dfa-4438-45d8-ac78-c7cc11735791.aspx

using BugTracker.Areas.Identity.Data;
using BugTracker.Data;
using BugTracker.Models;
using Dapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;

namespace BugTracker.Helpers
{
    public static class IdentityDataInitializer
    {
        public static void SeedDb()
        {
            string createTablesQuery = File.ReadAllText(@"F:\BugTracker\CreateTables.sql");
            string populateTablesQuery = File.ReadAllText(@"F:\BugTracker\PopulateTables.sql");

            // Create myDb.
            SqlConnection myDb = DbHelper.GetConnection();
            myDb.Execute(createTablesQuery);

            // Create and populate the demoDb.
            SqlConnection demoDb = DbHelper.GetDemoConnection();
            demoDb.Execute(createTablesQuery);
            demoDb.Execute(populateTablesQuery);

        }

        public static void SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            bool adminRoleExists = roleManager.RoleExistsAsync("Administrator").Result;
            bool demoAdminRoleExists = roleManager.RoleExistsAsync("DemoAdministrator").Result;
            bool devRoleExists = roleManager.RoleExistsAsync("Developer").Result;
            bool demoDevRoleExists = roleManager.RoleExistsAsync("DemoDeveloper").Result;
            bool submitterRoleExists = roleManager.RoleExistsAsync("Submitter").Result;
            bool demoSubmitterRoleExists = roleManager.RoleExistsAsync("DemoSubmitter").Result;
            bool noRoleRoleExists = roleManager.RoleExistsAsync("NoRole").Result;

            if (!adminRoleExists)
            {
                // Create Admin role.
                IdentityRole role = new IdentityRole()
                {
                    Name = "Administrator",
                    NormalizedName = "ADMINISTRATOR"

                };
                roleManager.CreateAsync(role);
            }

            if (!demoAdminRoleExists)
            {
                // Create Admin role.
                IdentityRole role = new IdentityRole()
                {
                    Name = "DemoAdministrator",
                    NormalizedName = "DEMOADMINISTRATOR"

                };
                roleManager.CreateAsync(role);
            }

            if (!devRoleExists)
            {
                // Create Dev role.
                IdentityRole role = new IdentityRole()
                {
                    Name = "Developer",
                    NormalizedName = "DEVELOPER"

                };
                roleManager.CreateAsync(role);
            }

            if (!demoDevRoleExists)
            {
                // Create Dev role.
                IdentityRole role = new IdentityRole()
                {
                    Name = "DemoDeveloper",
                    NormalizedName = "DEMODEVELOPER"

                };
                roleManager.CreateAsync(role);
            }

            if (!submitterRoleExists)
            {
                // Create Submitter role.
                IdentityRole role = new IdentityRole()
                {
                    Name = "Submitter",
                    NormalizedName = "SUBMITTER"

                };
                roleManager.CreateAsync(role);
            }

            if (!demoSubmitterRoleExists)
            {
                // Create Submitter role.
                IdentityRole role = new IdentityRole()
                {
                    Name = "DemoSubmitter",
                    NormalizedName = "DEMOSUBMITTER"

                };
                roleManager.CreateAsync(role);
            }

            if (!noRoleRoleExists)
            {
                // Create NoRole role.
                IdentityRole role = new IdentityRole()
                {
                    Name = "NoRole",
                    NormalizedName = "NOROLE"

                };
                roleManager.CreateAsync(role);
            }
        }


        public static async void SeedUsers(UserManager<BugTrackerUser> userManager)
        {
            bool demoAdminExits = userManager.FindByNameAsync("DemoAdministrator").Result != null;
            bool demoDevExits = userManager.FindByNameAsync("DemoDeveloper").Result != null;
            bool demoSubmitterExits = userManager.FindByNameAsync("DemoSubmitter").Result != null;
            // This will be different on deployment.
            bool myAccount = userManager.FindByNameAsync("DefaultAdmin").Result != null;

            await MakeAccountIfDoesNotExist(
                DbHelper.GetDemoConnection(),
                demoAdminExits, 
                "DemoAdministrator", "Demo", "Administrator", 
                userManager, 
                "DemoAdministrator", AllBTUserRoles.DemoAdministrator
            );

            await MakeAccountIfDoesNotExist(
                DbHelper.GetDemoConnection(),
                demoDevExits,
                "DemoDeveloper", "Demo", "Developer",
                userManager,
                "DemoDeveloper", AllBTUserRoles.DemoDeveloper
            );

            await MakeAccountIfDoesNotExist(
                DbHelper.GetDemoConnection(),
                demoSubmitterExits,
                "DemoSubmitter", "Demo", "Submitter",
                userManager,
                "DemoSubmitter", AllBTUserRoles.DemoSubmitter
            );

            await MakeAccountIfDoesNotExist(
                DbHelper.GetConnection(),
                myAccount,
                "DefaultAdmin", "Default", "Admin",
                userManager,
                "Administrator", AllBTUserRoles.Administrator
            );

        }


        public static async Task MakeAccountIfDoesNotExist(
            SqlConnection connection,
            bool doesAccountExist, 
            string username, string firstname, string lastname,
            UserManager<BugTrackerUser> userManager, 
            string roleName, AllBTUserRoles roleForMyDb
        )
        {
            if (!doesAccountExist)
            {
                // Create the model for Identity Account.
                BugTrackerUser user = new BugTrackerUser
                {
                    UserName = username,
                    Email = "demo@account.com",
                    FirstName = firstname,
                    LastName = lastname
                };
                // Add user to Identity and it's role.
                await userManager.CreateAsync(user, "WWSSadadqeeq1!");
                await userManager.AddToRoleAsync(user, roleName);

                // Add user to myDb.
                SqlHelper sqlHelper = new SqlHelper( connection );
                BTUser btUser = new BTUser(
                    sqlHelper.GenerateUserId(), user.Id,
                    user.UserName, user.FirstName, user.LastName,
                    0
                );
                // Add user to Identity and it's role to myDb.
                btUser.Role = (int)roleForMyDb;
                sqlHelper.InsertUser(btUser);
            }
        }

    }
}
