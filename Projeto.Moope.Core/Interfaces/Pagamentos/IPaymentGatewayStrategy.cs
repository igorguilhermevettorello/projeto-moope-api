using Projeto.Moope.Core.DTOs.Pagamentos;
using Projeto.Moope.Core.DTOs.Vendas;

namespace Projeto.Moope.Core.Interfaces.Pagamentos
{
    public interface IPaymentGatewayStrategy
    {
        Task<CelPayResponseDto> ProcessarPagamentoAsync(CreateVendaDto vendaDto);
        Task<CelPayResponseDto> ConsultarTransacaoAsync(string transactionId);
        string NomeGateway { get; }
    }
}
