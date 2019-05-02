using CoachApp.Models;
using Microsoft.AspNetCore.Identity;
using System.Linq;

namespace CoachApp.Data
{
    public static class DbInitializer
    {
        public static void Initialize(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            string[] roles = { "Coach", "Athlete" };
            foreach (var role in roles)
                if (!roleManager.Roles.Any(r => r.NormalizedName == role.ToUpper()))
                    roleManager.CreateAsync(new IdentityRole { Name = role }).Wait();

            if (!userManager.Users.Any())
            {
                var coach = new ApplicationUser
                {
                    FirstName = "Mitchel",
                    LastName = "Fausto",
                    UserName = "email@email.com",
                    Email = "email@email.com"
                };
                if (userManager.CreateAsync(coach, "_Aa123456").Result.Succeeded)
                    userManager.AddToRoleAsync(coach, roles[0].ToUpper()).Wait();

                string[] athleteNames = { "Queen Jacobi",
                                          "Magen Faye",
                                          "Delicia Ledonne",
                                          "Camille Grantham",
                                          "Marc Voth",
                                          "Randy Rondon",
                                          "Delora Saville",
                                          "Rosario Reuben",
                                          "Lula Uhlman" };
                ApplicationUser athlete;
                for (int i = 0; i < 9; i++)
                {
                    athlete = new ApplicationUser
                    {
                        UserName = athleteNames[i].Split(' ').First(),
                        FirstName = athleteNames[i].Split(' ').First(),
                        LastName = athleteNames[i].Split(' ').Last()
                    };
                    if (userManager.CreateAsync(athlete).Result.Succeeded)
                        userManager.AddToRoleAsync(athlete, roles[1].ToUpper()).Wait();
                }
            }
        }
    }
}
