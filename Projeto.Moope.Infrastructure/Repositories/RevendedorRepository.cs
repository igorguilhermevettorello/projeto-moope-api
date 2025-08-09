using Microsoft.EntityFrameworkCore;
using Projeto.Moope.Core.Interfaces.Repositories;
using Projeto.Moope.Core.Models;
using Projeto.Moope.Infrastructure.Data;

namespace Projeto.Moope.Infrastructure.Repositories
{
    public class RevendedorRepository : IRevendedorRepository
    {
        private readonly AppDbContext _context;
        public RevendedorRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Revendedor> BuscarPorIdAsync(Guid id)
        {
            return await _context.Revendedores
                .Include(r => r.Papel)
                .Include(r => r.Usuario)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<IEnumerable<Revendedor>> BuscarTodosAsync()
        {
            return await _context.Revendedores
                .Include(r => r.Papel)
                .Include(r => r.Usuario)
                .ToListAsync();
        }

        public async Task<Revendedor> SalvarAsync(Revendedor entity)
        {
            await _context.Revendedores.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<Revendedor> AtualizarAsync(Revendedor entity)
        {
            _context.Revendedores.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> RemoverAsync(Guid id)
        {
            var revendedor = await _context.Revendedores.FindAsync(id);
            if (revendedor != null)
            {
                _context.Revendedores.Remove(revendedor);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
} 