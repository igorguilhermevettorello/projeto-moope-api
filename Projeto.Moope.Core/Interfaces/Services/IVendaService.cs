using Projeto.Moope.Core.DTOs.Vendas;

namespace Projeto.Moope.Core.Interfaces.Services
{
    public interface IVendaService
    {
        Task<VendaResponseDto> ProcessarVendaAsync(CreateVendaDto vendaDto);
        Task<VendaResponseDto> ConsultarVendaAsync(Guid vendaId);
        Task<IEnumerable<VendaResponseDto>> ListarVendasPorVendedorAsync(Guid vendedorId);
        Task<IEnumerable<VendaResponseDto>> ListarVendasPorClienteAsync(string email);
    }
}
