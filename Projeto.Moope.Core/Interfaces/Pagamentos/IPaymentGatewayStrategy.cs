using Projeto.Moope.API.DTOs;
using Projeto.Moope.Core.DTOs.Pagamentos;
using Projeto.Moope.Core.DTOs.Vendas;

namespace Projeto.Moope.Core.Interfaces.Pagamentos
{
    public interface IPaymentGatewayStrategy
    {
        Task<CelPayResponseDto> ProcessarPagamentoAsync(VendaStoreDto vendaDto);
        Task<CelPayResponseDto> ConsultarTransacaoAsync(string transactionId);
        string NomeGateway { get; }
    }
}
