using Projeto.Moope.API.Utils;
using Projeto.Moope.Core.Interfaces.Notifications;
using Projeto.Moope.Core.Notifications;
using Projeto.Moope.Infrastructure.Data;


namespace Projeto.Moope.API.Configurations
{
    public static class DependencyInjectionConfig
    {
        public static IServiceCollection AddDependencyInjectionConfig(this IServiceCollection services, IConfiguration configuration)
        {
            RegisterApplicationDependencies(services, configuration);
            RegisterRepositories(services);
            RegisterServices(services);
            return services;
        }

        private static void RegisterApplicationDependencies(IServiceCollection service, IConfiguration configuration)
        {
            service.AddScoped<ApplicationDbContext>();
            //service.AddScoped<IAppEnvironment, AppEnvironment>();
            service.AddScoped<INotificador, Notificador>();
            service.Configure<JwtSettings>(configuration.GetSection("Jwt"));
            service.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            //service.AddScoped<IUser, AspNetUser>();


            //service.AddScoped<IUserContext, UserContext>();
            //service.AddScoped<MarketplaceContext>();
            //service.AddScoped<INotifier, Notifier>();
        }

        private static void RegisterRepositories(IServiceCollection service)
        {
            //service.AddScoped<IVendedorRepository, VendedorRepository>();
            //service.AddScoped<IProdutoRepository, ProdutoRepository>();
            //service.AddScoped<ICategoriaRepository, CategoriaRepository>();
            //service.AddScoped<IFornecedorRepository, FornecedorRepository>();
            //service.AddScoped<IEnderecoRepository, EnderecoRepository>();
            //service.AddScoped<IUserRepository<ApplicationUser>, UserRepository>();
        }

        private static void RegisterServices(IServiceCollection service)
        {
            //service.AddScoped<IContaService, ContaService>();
            //service.AddScoped<ICategoriaService, CategoriaService>();
            //service.AddScoped<IProdutoService, ProdutoService>();
            //service.AddScoped<IFornecedorService, FornecedorService>();
        }
    }
}
