using FluentValidation;
using Projeto.Moope.API.DTOs.Clientes;
using Projeto.Moope.API.DTOs.Planos;
using Projeto.Moope.API.DTOs.Validators;
using Projeto.Moope.API.Utils;
using Projeto.Moope.Core.Interfaces.Identity;
using Projeto.Moope.Core.Interfaces.Notifications;
using Projeto.Moope.Core.Interfaces.Repositories;
using Projeto.Moope.Core.Interfaces.Services;
using Projeto.Moope.Core.Interfaces.UnitOfWork;
using Projeto.Moope.Core.Notifications;
using Projeto.Moope.Core.Services;
using Projeto.Moope.Infrastructure.Data;
using Projeto.Moope.Infrastructure.Data.UnitOfWork;
using Projeto.Moope.Infrastructure.Repositories;

namespace Projeto.Moope.API.Configurations
{
    public static class DependencyInjectionConfig
    {
        public static IServiceCollection AddDependencyInjectionConfig(this IServiceCollection services, IConfiguration configuration)
        {
            RegisterApplicationDependencies(services, configuration);
            RegisterRepositories(services);
            RegisterServices(services);
            RegisterValidators(services);
            return services;
        }

        private static void RegisterApplicationDependencies(IServiceCollection service, IConfiguration configuration)
        {
            service.AddScoped<AppDbContext>();
            //service.AddScoped<IAppEnvironment, AppEnvironment>();
            service.AddScoped<INotificador, Notificador>();
            service.Configure<JwtSettings>(configuration.GetSection("Jwt"));
            service.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            service.AddScoped<IUser, AspNetUser>();





            //service.AddScoped<IUserContext, UserContext>();
            //service.AddScoped<MarketplaceContext>();
            //service.AddScoped<INotifier, Notifier>();
        }

        private static void RegisterRepositories(IServiceCollection service)
        {
            //service.AddScoped<IVendedorRepository, VendedorRepository>();
            service.AddScoped<IPlanoRepository, PlanoRepository>();
            service.AddScoped<IClienteRepository, ClienteRepository>();
            //service.AddScoped<ICategoriaRepository, CategoriaRepository>();
            //service.AddScoped<IFornecedorRepository, FornecedorRepository>();
            service.AddScoped<IEnderecoRepository, EnderecoRepository>();
            //service.AddScoped<IUserRepository<ApplicationUser>, UserRepository>();
            service.AddScoped<IUsuarioRepository, UsuarioRepository>();
            service.AddScoped<IPessoaFisicaRepository, PessoaFisicaRepository>();
            service.AddScoped<IUnitOfWork, UnitOfWork>();
        }

        private static void RegisterServices(IServiceCollection service)
        {
            //service.AddScoped<IContaService, ContaService>();
            //service.AddScoped<ICategoriaService, CategoriaService>();
            service.AddScoped<IPlanoService, PlanoService>();
            service.AddScoped<IClienteService, ClienteService>();
            //service.AddScoped<IFornecedorService, FornecedorService>();
            service.AddScoped<IUsuarioService, UsuarioService>();
            service.AddHttpClient<IGoogleRecaptchaService, GoogleRecaptchaService>();
            //service.AddScoped<IValidator<UsuarioDto>, UsuarioValidator>();
        }

        private static void RegisterValidators(IServiceCollection service)
        {
            service.AddScoped<IValidator<PlanoDto>, PlanoDtoValidator>();
            service.AddScoped<IValidator<CreateClienteDto>, CreateClienteDtoValidator>();
            service.AddScoped<IValidator<UpdateClienteDto>, UpdateClienteDtoValidator>();
        }
    }
}
