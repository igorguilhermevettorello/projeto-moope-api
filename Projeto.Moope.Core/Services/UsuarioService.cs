using Projeto.Moope.Core.Interfaces.Notifications;
using Projeto.Moope.Core.Interfaces.Repositories;
using Projeto.Moope.Core.Interfaces.Services;
using Projeto.Moope.Core.Models;
using Projeto.Moope.Core.Models.Validators;
using Projeto.Moope.Core.Models.Validators.Base;
using Projeto.Moope.Core.Models.Validators.Usuario;
using Projeto.Moope.Core.Services.Base;

namespace Projeto.Moope.Core.Services
{
    public class UsuarioService : BaseService, IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        public UsuarioService(
            IUsuarioRepository usuarioRepository,
            INotificador notificador) : base(notificador)
        {
            _usuarioRepository = usuarioRepository;
        }

        public async Task<Usuario> BuscarPorIdAsync(Guid id)
        {
            return await _usuarioRepository.BuscarPorIdAsync(id);
        }

        public async Task<IEnumerable<Usuario>> BuscarTodosAsync()
        {
            return await _usuarioRepository.BuscarTodosAsync();
        }

        public async Task<Result<Usuario>> SalvarAsync(Usuario usuario)
        {
            if (!ExecutarValidacao(new UsuarioValidator(), usuario))
            {
                return new Result<Usuario>()
                {
                    Status = false
                };
            }

            var entity = await _usuarioRepository.SalvarAsync(usuario);
            return new Result<Usuario>()
            {
                Status = false,
                Dados = entity
            };
        }

        public async Task<Result<Usuario>> AtualizarAsync(Usuario usuario)
        {
            if(!ExecutarValidacao(new UsuarioValidator(), usuario))
            {
                return new Result<Usuario>()
                {
                    Status = false
                };
            }

            var entity = await _usuarioRepository.AtualizarAsync(usuario);
            return new Result<Usuario>()
            {
                Status = false,
                Dados = entity
            };
        }

        public async Task<bool> RemoverAsync(Guid id)
        {
            await _usuarioRepository.RemoverAsync(id);
            return true;
        }
    }
} 