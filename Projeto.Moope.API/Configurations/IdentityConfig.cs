using Microsoft.AspNetCore.Identity;
using Projeto.Moope.API.Utils;
using Projeto.Moope.Infrastructure.Data;

namespace Projeto.Moope.API.Configurations
{
    public static class IdentityConfig
    {
        public static IServiceCollection AddIdentityConfig(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddIdentity<IdentityUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddErrorDescriber<IdentityErrorDescriberPtBr>()
            .AddDefaultTokenProviders();

            return services;
        }
    }
}
