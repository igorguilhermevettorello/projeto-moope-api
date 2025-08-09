using Microsoft.EntityFrameworkCore;
using Projeto.Moope.Core.Interfaces.Repositories;
using Projeto.Moope.Core.Models;
using Projeto.Moope.Infrastructure.Data;

namespace Projeto.Moope.Infrastructure.Repositories
{
    public class PessoaFisicaRepository : IPessoaFisicaRepository
    {
        private readonly AppDbContext _context;
        public PessoaFisicaRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PessoaFisica> BuscarPorIdAsync(Guid id)
        {
            return await _context.PessoasFisicas.Include(pf => pf.Cliente).FirstOrDefaultAsync(pf => pf.ClienteId == id);
        }

        public async Task<IEnumerable<PessoaFisica>> BuscarTodosAsync()
        {
            return await _context.PessoasFisicas.Include(pf => pf.Cliente).ToListAsync();
        }

        public async Task<PessoaFisica> SalvarAsync(PessoaFisica entity)
        {
            await _context.PessoasFisicas.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<PessoaFisica> AtualizarAsync(PessoaFisica entity)
        {
            _context.PessoasFisicas.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> RemoverAsync(Guid id)
        {
            var pessoaFisica = await _context.PessoasFisicas.FindAsync(id);
            if (pessoaFisica != null)
            {
                _context.PessoasFisicas.Remove(pessoaFisica);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
} 