using Projeto.Moope.Core.DTOs.Pagamentos;
using Projeto.Moope.Core.DTOs.Vendas;
using Projeto.Moope.Core.Interfaces.Pagamentos;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Projeto.Moope.API.DTOs;

namespace Projeto.Moope.Infrastructure.Services.Pagamentos
{
    public class CelPayGatewayStrategy : IPaymentGatewayStrategy
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<CelPayGatewayStrategy> _logger;

        public string NomeGateway => "CelPay";

        public CelPayGatewayStrategy(
            HttpClient httpClient, 
            IConfiguration configuration,
            ILogger<CelPayGatewayStrategy> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
            
            // Configurar base URL e headers padrão
            _httpClient.BaseAddress = new Uri(_configuration["CelPay:BaseUrl"] ?? "https://api.celpayments.com.br");
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_configuration["CelPay:ApiKey"]}");
            // _httpClient.DefaultRequestHeaders.Add("Content-Type", "application/json");
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<CelPayResponseDto> ProcessarPagamentoAsync(VendaStoreDto vendaDto)
        {
            try
            {
                var requestDto = MapearParaCelPayRequest(vendaDto);
                var jsonContent = JsonSerializer.Serialize(requestDto);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                _logger.LogInformation("Processando pagamento via CelPay para venda: {VendaId}", requestDto.ExternalId);

                var response = await _httpClient.PostAsync("/v1/charges", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var celPayResponse = JsonSerializer.Deserialize<CelPayResponseDto>(responseContent);
                    _logger.LogInformation("Pagamento processado com sucesso via CelPay. TransactionId: {TransactionId}", celPayResponse?.Id);
                    return celPayResponse ?? new CelPayResponseDto { Status = "ERROR", ErrorMessage = "Resposta inválida do gateway" };
                }
                else
                {
                    _logger.LogError("Erro ao processar pagamento via CelPay. Status: {Status}, Response: {Response}", response.StatusCode, responseContent);
                    return new CelPayResponseDto 
                    { 
                        Status = "ERROR", 
                        ErrorMessage = $"Erro HTTP: {response.StatusCode}",
                        ErrorCode = response.StatusCode.ToString()
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao processar pagamento via CelPay");
                return new CelPayResponseDto 
                { 
                    Status = "ERROR", 
                    ErrorMessage = "Erro interno do sistema",
                    ErrorCode = "INTERNAL_ERROR"
                };
            }
        }

        public async Task<CelPayResponseDto> ConsultarTransacaoAsync(string transactionId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/v1/charges/{transactionId}");
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var celPayResponse = JsonSerializer.Deserialize<CelPayResponseDto>(responseContent);
                    return celPayResponse ?? new CelPayResponseDto { Status = "ERROR", ErrorMessage = "Resposta inválida do gateway" };
                }
                else
                {
                    _logger.LogError("Erro ao consultar transação via CelPay. Status: {Status}, Response: {Response}", response.StatusCode, responseContent);
                    return new CelPayResponseDto 
                    { 
                        Status = "ERROR", 
                        ErrorMessage = $"Erro HTTP: {response.StatusCode}",
                        ErrorCode = response.StatusCode.ToString()
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao consultar transação via CelPay");
                return new CelPayResponseDto 
                { 
                    Status = "ERROR", 
                    ErrorMessage = "Erro interno do sistema",
                    ErrorCode = "INTERNAL_ERROR"
                };
            }
        }

        private CelPayRequestDto MapearParaCelPayRequest(VendaStoreDto vendaDto)
        {
            var (mes, ano) = ExtrairMesAnoValidade(vendaDto.DataValidade);
            
            return new CelPayRequestDto
            {
                ExternalId = Guid.NewGuid().ToString(),
                Amount = vendaDto.Valor,
                Currency = "BRL",
                Card = new CardInfo
                {
                    Number = vendaDto.NumeroCartao,
                    ExpMonth = mes,
                    ExpYear = ano,
                    Cvv = vendaDto.Cvv,
                    HolderName = vendaDto.NomeCliente
                },
                Customer = new CustomerInfo
                {
                    Name = vendaDto.NomeCliente,
                    Email = vendaDto.Email,
                    Phone = vendaDto.Telefone
                },
                Description = vendaDto.Descricao ?? $"Venda para {vendaDto.NomeCliente}",
                Capture = "true",
                Installments = "1"
            };
        }

        private (string mes, string ano) ExtrairMesAnoValidade(string dataValidade)
        {
            var partes = dataValidade.Split('/');
            if (partes.Length == 2)
            {
                var mes = partes[0];
                var ano = "20" + partes[1]; // Assumindo formato MM/YY
                return (mes, ano);
            }
            
            throw new ArgumentException("Formato de data de validade inválido. Use MM/YY");
        }
    }
}
