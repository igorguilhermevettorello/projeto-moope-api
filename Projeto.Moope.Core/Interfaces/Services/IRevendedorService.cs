using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Projeto.Moope.Core.Models;
using Projeto.Moope.Core.Models.Validators.Base;

namespace Projeto.Moope.Core.Interfaces.Services
{
    public interface IRevendedorService
    {
        Task<Revendedor> BuscarPorIdAsync(Guid id);
        Task<IEnumerable<Revendedor>> BuscarTodosAsync();
        Task<Result<Revendedor>> SalvarAsync(Revendedor revendedor);
        Task<Result<Revendedor>> AtualizarAsync(Revendedor revendedor);
        Task<bool> RemoverAsync(Guid id);
    }
}