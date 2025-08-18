using Microsoft.EntityFrameworkCore;
using Projeto.Moope.Core.Interfaces.Repositories;
using Projeto.Moope.Core.Models;
using Projeto.Moope.Infrastructure.Data;

namespace Projeto.Moope.Infrastructure.Repositories
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly AppDbContext _context;
        public ClienteRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Cliente> BuscarPorIdAsNotrackingAsync(Guid id)
        {
            return await _context.Clientes.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
        }
            
        public async Task<Cliente> BuscarPorIdAsync(Guid id)
        {
            return await _context.Clientes.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<Cliente>> BuscarTodosAsync()
        {
            return await _context.Clientes.ToListAsync();
        }

        public async Task<Cliente> SalvarAsync(Cliente entity)
        {
            await _context.Clientes.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<Cliente> AtualizarAsync(Cliente entity)
        {
            _context.Clientes.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> RemoverAsync(Guid id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente != null)
            {
                _context.Clientes.Remove(cliente);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
} 