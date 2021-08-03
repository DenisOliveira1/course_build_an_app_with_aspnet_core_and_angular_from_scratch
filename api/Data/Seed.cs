using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using api.Context;
using api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace api.Data
{
    public class Seed
    {
        public static async Task SeedUsers(UserManager<UserModel> userManager, RoleManager<RoleModel> roleManager)
        {
            // Sai caso a tabela Users tenha alguma inserção
            if(await userManager.Users.AnyAsync()) return;

            var userData = await System.IO.File.ReadAllTextAsync("Data/UserSeedData.json");
            var users = JsonSerializer.Deserialize<List<UserModel>>(userData);
            if (users == null) return;

            var roles = new List<RoleModel>{
                new RoleModel{Name = "Member"},
                new RoleModel{Name = "Admin"},
                new RoleModel{Name = "Moderator"},
            };

            foreach (var role in roles){
                await roleManager.CreateAsync(role);
            }

            foreach (var user in users){
                user.UserName = user.UserName.ToLower();
                await userManager.CreateAsync(user, "password");
                await userManager.AddToRoleAsync(user, "Member");
                // Quando se adiciona role ao usuário o userManager exige que o nome o userName do usuário seja único
            }

            CreateAdmin(userManager, roleManager);

        }

        public static async void CreateAdmin(UserManager<UserModel> userManager, RoleManager<RoleModel> roleManager){

            var admin = new UserModel{
                UserName = "admin",
                KnownAs = "Admin"
            };

            await userManager.CreateAsync(admin, "password");
            await userManager.AddToRolesAsync(admin, new[] {"Admin","Moderator"});
        }
    }
}