using FluentValidation;
using Projeto.Moope.API.DTOs.Clientes;
using Projeto.Moope.API.DTOs.Planos;
using Projeto.Moope.API.DTOs.Validators;
using Projeto.Moope.API.Utils;
using Projeto.Moope.Core.DTOs.Validators;
using Projeto.Moope.Core.DTOs.Vendas;
using Projeto.Moope.Core.Interfaces.Identity;
using Projeto.Moope.Core.Interfaces.Notifications;
using Projeto.Moope.Core.Interfaces.Repositories;
using Projeto.Moope.Core.Interfaces.Services;
using Projeto.Moope.Core.Interfaces.UnitOfWork;
using Projeto.Moope.Core.Interfaces.Pagamentos;
using Projeto.Moope.Core.Notifications;
using Projeto.Moope.Core.Services;
using Projeto.Moope.Infrastructure.Data;
using Projeto.Moope.Infrastructure.Data.UnitOfWork;
using Projeto.Moope.Infrastructure.Repositories;
using Projeto.Moope.Infrastructure.Services.Pagamentos;

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
            service.AddScoped<INotificador, Notificador>();
            service.Configure<JwtSettings>(configuration.GetSection("Jwt"));
            service.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            service.AddScoped<IUser, AspNetUser>();
        }

        private static void RegisterRepositories(IServiceCollection service)
        {
            service.AddScoped<IPlanoRepository, PlanoRepository>();
            service.AddScoped<IClienteRepository, ClienteRepository>();
            service.AddScoped<IVendedorRepository, VendedorRepository>();
            service.AddScoped<IEnderecoRepository, EnderecoRepository>();
            service.AddScoped<IUsuarioRepository, UsuarioRepository>();
            service.AddScoped<IPessoaFisicaRepository, PessoaFisicaRepository>();
            service.AddScoped<IPessoaJuridicaRepository, PessoaJuridicaRepository>();
            service.AddScoped<IPapelRepository, PapelRepository>();
            service.AddScoped<IPedidoRepository, PedidoRepository>();
            service.AddScoped<ITransacaoRepository, TransacaoRepository>();
            service.AddScoped<IUnitOfWork, UnitOfWork>();
        }

        private static void RegisterServices(IServiceCollection service)
        {
            service.AddScoped<IPlanoService, PlanoService>();
            service.AddScoped<IPapelService, PapelService>();
            service.AddScoped<IClienteService, ClienteService>();
            service.AddScoped<IVendedorService, VendedorService>();
            service.AddScoped<IEnderecoService, EnderecoService>();
            service.AddScoped<IUsuarioService, UsuarioService>();
            service.AddScoped<IIdentityUserService, IdentityUserService>();
            service.AddScoped<IVendaService, VendaService>();
            service.AddHttpClient<IGoogleRecaptchaService, GoogleRecaptchaService>();
        }

        private static void RegisterValidators(IServiceCollection service)
        {
            service.AddScoped<IValidator<PlanoDto>, PlanoDtoValidator>();
            service.AddScoped<IValidator<CreateClienteDto>, CreateClienteDtoValidator>();
            service.AddScoped<IValidator<UpdateClienteDto>, UpdateClienteDtoValidator>();
            service.AddScoped<IValidator<CreateVendaDto>, CreateVendaDtoValidator>();
        }
    }
}