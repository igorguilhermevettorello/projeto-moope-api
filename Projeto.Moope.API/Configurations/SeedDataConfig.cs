using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Projeto.Moope.Core.Enums;
using Projeto.Moope.Core.Models;
using Projeto.Moope.Infrastructure.Data;

namespace Projeto.Moope.API.Configurations
{
    public static class SeedDataConfig
    {
        public static async void UseSeedData(this IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<AppDbContext>();
                var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
                var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                await context.Database.MigrateAsync();
                await SeedRolesAsync(roleManager);
                await SeedUsersAsync(userManager, context);
            }
        }

        private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            foreach (var roleName in Enum.GetNames(typeof(TipoUsuario)))
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }

        private static async Task SeedUsersAsync(UserManager<IdentityUser> userManager, AppDbContext context)
        {
            var emailAdmin = "admin@moope.com.br";
            if (await userManager.FindByEmailAsync(emailAdmin) == null)
            {
                var user = new IdentityUser
                {
                    UserName = emailAdmin,
                    Email = emailAdmin,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, "Admin@123");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, TipoUsuario.Administrador.ToString());

                    var novoUsuario = new Usuario
                    {
                        Nome = "Administrador do Sistema",
                        Email = emailAdmin,
                        Ativo = true,
                        Tipo = TipoUsuario.Administrador,
                        Telefone = "(99)99999-9999",
                        IdentityUserId = user.Id,
                        Created = DateTime.UtcNow,
                        Updated = DateTime.UtcNow
                    };
                    await context.Usuarios.AddAsync(novoUsuario);
                    await context.SaveChangesAsync();
                }
            }
        }
    }
} 