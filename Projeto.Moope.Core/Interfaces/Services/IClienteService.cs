using Projeto.Moope.Core.DTOs.Clientes;
using Projeto.Moope.Core.Models;
using Projeto.Moope.Core.Models.Validators.Base;

namespace Projeto.Moope.Core.Interfaces.Services
{
    public interface IClienteService
    {
        Task<Cliente> BuscarPorIdAsync(Guid id);
        Task<IEnumerable<Cliente>> BuscarTodosAsync();
        Task<Result<Cliente>> SalvarAsync(Cliente cliente, Endereco endereco, Usuario usuario, ClienteStoreDto auxiliar);
        Task<Result<Cliente>> AtualizarAsync(Cliente cliente, Endereco endereco, Usuario usuario, ClienteStoreDto auxiliar);
        Task<Result<Cliente>> AtualizarAsync(Cliente cliente);
        Task<bool> RemoverAsync(Guid id);
    }
}