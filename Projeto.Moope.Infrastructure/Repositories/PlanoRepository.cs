using Microsoft.EntityFrameworkCore;
using Projeto.Moope.Core.Interfaces.Repositories;
using Projeto.Moope.Core.Models;
using Projeto.Moope.Infrastructure.Data;

namespace Projeto.Moope.Infrastructure.Repositories
{
    public class PlanoRepository : IPlanoRepository
    {
        private readonly AppDbContext _context;
        public PlanoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Plano> BuscarPorIdAsync(Guid id)
        {
            return await _context.Planos.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Plano>> BuscarTodosAsync()
        {
            return await _context.Planos.ToListAsync();
        }

        public async Task<Plano> SalvarAsync(Plano entity)
        {
            await _context.Planos.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<Plano> AtualizarAsync(Plano entity)
        {
            _context.Planos.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> RemoverAsync(Guid id)
        {
            var plano = await _context.Planos.FindAsync(id);
            if (plano != null)
            {
                _context.Planos.Remove(plano);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
} 