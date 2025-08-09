using Projeto.Moope.Core.Interfaces.Notifications;
using Projeto.Moope.Core.Interfaces.Repositories;
using Projeto.Moope.Core.Interfaces.Services;
using Projeto.Moope.Core.Models;
using Projeto.Moope.Core.Models.Validators;
using Projeto.Moope.Core.Models.Validators.Base;
using Projeto.Moope.Core.Notifications;
using Projeto.Moope.Core.Services.Base;

namespace Projeto.Moope.Core.Services
{
    public class RevendedorService : BaseService, IRevendedorService
    {
        private readonly IRevendedorRepository _revendedorRepository;
        public RevendedorService(
            IRevendedorRepository revendedorRepository,
            INotificador notificador) : base(notificador)
        {
            _revendedorRepository = revendedorRepository;
        }

        public async Task<Revendedor> BuscarPorIdAsync(Guid id)
        {
            return await _revendedorRepository.BuscarPorIdAsync(id);
        }

        public async Task<IEnumerable<Revendedor>> BuscarTodosAsync()
        {
            return await _revendedorRepository.BuscarTodosAsync();
        }

        public async Task<Result<Revendedor>> SalvarAsync(Revendedor revendedor)
        {
            if (!ExecutarValidacao(new RevendedorValidator(), revendedor))
            {
                return new Result<Revendedor>()
                {
                    Status = false
                };
            }

            var entity = await _revendedorRepository.SalvarAsync(revendedor);
            return new Result<Revendedor>()
            {
                Status = false,
                Dados = entity
            };
        }

        public async Task<Result<Revendedor>> AtualizarAsync(Revendedor revendedor)
        {
            if (!ExecutarValidacao(new RevendedorValidator(), revendedor))
            {
                return new Result<Revendedor>()
                {
                    Status = false
                };
            }

            var entity = await _revendedorRepository.SalvarAsync(revendedor);
            return new Result<Revendedor>()
            {
                Status = false,
                Dados = entity
            };
        }

        public async Task<bool> RemoverAsync(Guid id)
        {
            // Aqui você pode adicionar lógica para remover o Papel, se necessário
            await _revendedorRepository.RemoverAsync(id);
            return true;
        }
    }
} 