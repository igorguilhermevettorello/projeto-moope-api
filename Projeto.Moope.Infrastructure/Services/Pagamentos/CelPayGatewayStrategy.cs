using Projeto.Moope.Core.DTOs.Pagamentos;
using Projeto.Moope.Core.DTOs.Vendas;
using Projeto.Moope.Core.Interfaces.Pagamentos;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Projeto.Moope.API.DTOs;
using System.Net.Http.Headers;

namespace Projeto.Moope.Infrastructure.Services.Pagamentos
{
    public class CelPayGatewayStrategy : IPaymentGatewayStrategy
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<CelPayGatewayStrategy> _logger;
        private CelPayAuthResponseDto? _cachedToken;
        private readonly object _tokenLock = new object();

        public string NomeGateway => "CelPay";

        public CelPayGatewayStrategy(
            HttpClient httpClient, 
            IConfiguration configuration,
            ILogger<CelPayGatewayStrategy> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
            
            // Configurar base URL
            var baseUrl = _configuration["CelPay:IsProduction"] == "true" 
                ? _configuration["CelPay:BaseUrl"] 
                : _configuration["CelPay:BaseUrlSandbox"];
            
            _httpClient.BaseAddress = new Uri(baseUrl ?? "https://api.sandbox.cel.cash/v2");
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            
            var timeoutSeconds = int.Parse(_configuration["CelPay:TimeoutSeconds"] ?? "30");
            _httpClient.Timeout = TimeSpan.FromSeconds(timeoutSeconds);
        }

        /// <summary>
        /// Autentica na API do CelPay e obtém o token de acesso
        /// </summary>
        private async Task<CelPayAuthResponseDto> ObterTokenAsync()
        {
            lock (_tokenLock)
            {
                // Verifica se já temos um token válido em cache
                if (_cachedToken?.IsTokenValido == true)
                {
                    return _cachedToken;
                }
            }

            try
            {
                var authConfig = ObterConfiguracaoAuth();
                var credentials = $"{authConfig.GalaxId}:{authConfig.GalaxHash}";
                var base64Credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(credentials));

                // Configurar headers de autenticação
                _httpClient.DefaultRequestHeaders.Authorization = 
                    new AuthenticationHeaderValue("Basic", base64Credentials);

                // Adicionar header de parceiro se configurado
                if (!string.IsNullOrEmpty(authConfig.GalaxIdPartner) && 
                    !string.IsNullOrEmpty(authConfig.GalaxHashPartner))
                {
                    var partnerCredentials = $"{authConfig.GalaxIdPartner}:{authConfig.GalaxHashPartner}";
                    var base64PartnerCredentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(partnerCredentials));
                    _httpClient.DefaultRequestHeaders.Remove("AuthorizationPartner");
                    _httpClient.DefaultRequestHeaders.Add("AuthorizationPartner", base64PartnerCredentials);
                }

                // Preparar dados da requisição de autenticação
                var authRequest = new CelPayAuthRequestDto
                {
                    Scope = _configuration["CelPay:Scope"] ?? "customers.read customers.write plans.read plans.write transactions.read transactions.write webhooks.write cards.read cards.write card-brands.read subscriptions.read subscriptions.write charges.read charges.write boletos.read"
                };
                var jsonContent = JsonSerializer.Serialize(authRequest, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
                });
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                _logger.LogInformation("Obtendo token de autenticação do CelPay com scope: {Scope}", authRequest.Scope);

                var response = await _httpClient.PostAsync("/token", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var authResponse = JsonSerializer.Deserialize<CelPayAuthResponseDto>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
                    });

                    if (authResponse != null)
                    {
                        authResponse.ObtidoEm = DateTime.UtcNow;
                        
                        lock (_tokenLock)
                        {
                            _cachedToken = authResponse;
                        }

                        _logger.LogInformation("Token obtido com sucesso. Expira em {ExpiresIn} segundos", authResponse.ExpiresIn);
                        return authResponse;
                    }
                }

                _logger.LogError("Erro ao obter token. Status: {Status}, Response: {Response}", response.StatusCode, responseContent);
                throw new InvalidOperationException($"Falha na autenticação CelPay: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao obter token do CelPay");
                throw;
            }
        }

        /// <summary>
        /// Configura o HttpClient com o token de autorização Bearer
        /// </summary>
        private async Task ConfigurarAutorizacaoAsync()
        {
            var token = await ObterTokenAsync();
            _httpClient.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Bearer", token.AccessToken);
        }

        /// <summary>
        /// Obtém configuração de autenticação das configurações
        /// </summary>
        private CelPayAuthConfigDto ObterConfiguracaoAuth()
        {
            return new CelPayAuthConfigDto
            {
                GalaxId = _configuration["CelPay:GalaxId"] ?? throw new InvalidOperationException("CelPay:GalaxId não configurado"),
                GalaxHash = _configuration["CelPay:GalaxHash"] ?? throw new InvalidOperationException("CelPay:GalaxHash não configurado"),
                GalaxIdPartner = _configuration["CelPay:GalaxIdPartner"],
                GalaxHashPartner = _configuration["CelPay:GalaxHashPartner"],
                BaseUrl = _httpClient.BaseAddress?.ToString() ?? "",
                IsProduction = bool.Parse(_configuration["CelPay:IsProduction"] ?? "false")
            };
        }

        public async Task<CelPayResponseDto> ProcessarPagamentoAsync(VendaStoreDto vendaDto)
        {
            try
            {
                // Configurar autenticação antes de fazer a requisição
                await ConfigurarAutorizacaoAsync();

                var requestDto = MapearParaCelPayRequest(vendaDto);
                var jsonContent = JsonSerializer.Serialize(requestDto);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                _logger.LogInformation("Processando pagamento via CelPay para venda: {VendaId}", requestDto.ExternalId);

                var response = await _httpClient.PostAsync("/charges", content);
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
                // Configurar autenticação antes de fazer a requisição
                await ConfigurarAutorizacaoAsync();

                var response = await _httpClient.GetAsync($"/charges/{transactionId}");
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

        /// <summary>
        /// Cria uma subscription com plano no CelPay
        /// </summary>
        public async Task<CelPaySubscriptionResponseDto> CriarSubscriptionComPlanoAsync(CelPaySubscriptionRequestDto subscriptionDto)
        {
            try
            {
                // Configurar autenticação antes de fazer a requisição
                await ConfigurarAutorizacaoAsync();

                var jsonContent = JsonSerializer.Serialize(subscriptionDto, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
                });
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                _logger.LogInformation("Criando subscription via CelPay para plano: {PlanId}", subscriptionDto.PlanId);

                var response = await _httpClient.PostAsync("/subscriptions", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var subscriptionResponse = JsonSerializer.Deserialize<CelPaySubscriptionResponseDto>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
                    });
                    
                    _logger.LogInformation("Subscription criada com sucesso via CelPay. SubscriptionId: {SubscriptionId}", subscriptionResponse?.Id);
                    return subscriptionResponse ?? new CelPaySubscriptionResponseDto { Status = "ERROR", ErrorMessage = "Resposta inválida do gateway" };
                }
                else
                {
                    _logger.LogError("Erro ao criar subscription via CelPay. Status: {Status}, Response: {Response}", response.StatusCode, responseContent);
                    return new CelPaySubscriptionResponseDto 
                    { 
                        Status = "ERROR", 
                        ErrorMessage = $"Erro HTTP: {response.StatusCode}",
                        ErrorCode = response.StatusCode.ToString()
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao criar subscription via CelPay");
                return new CelPaySubscriptionResponseDto 
                { 
                    Status = "ERROR", 
                    ErrorMessage = "Erro interno do sistema",
                    ErrorCode = "INTERNAL_ERROR"
                };
            }
        }

        /// <summary>
        /// Consulta uma subscription no CelPay
        /// </summary>
        public async Task<CelPaySubscriptionResponseDto> ConsultarSubscriptionAsync(string subscriptionId)
        {
            try
            {
                // Configurar autenticação antes de fazer a requisição
                await ConfigurarAutorizacaoAsync();

                var response = await _httpClient.GetAsync($"/subscriptions/{subscriptionId}");
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var subscriptionResponse = JsonSerializer.Deserialize<CelPaySubscriptionResponseDto>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
                    });
                    return subscriptionResponse ?? new CelPaySubscriptionResponseDto { Status = "ERROR", ErrorMessage = "Resposta inválida do gateway" };
                }
                else
                {
                    _logger.LogError("Erro ao consultar subscription via CelPay. Status: {Status}, Response: {Response}", response.StatusCode, responseContent);
                    return new CelPaySubscriptionResponseDto 
                    { 
                        Status = "ERROR", 
                        ErrorMessage = $"Erro HTTP: {response.StatusCode}",
                        ErrorCode = response.StatusCode.ToString()
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao consultar subscription via CelPay");
                return new CelPaySubscriptionResponseDto 
                { 
                    Status = "ERROR", 
                    ErrorMessage = "Erro interno do sistema",
                    ErrorCode = "INTERNAL_ERROR"
                };
            }
        }

        /// <summary>
        /// Cancela uma subscription no CelPay
        /// </summary>
        public async Task<CelPaySubscriptionResponseDto> CancelarSubscriptionAsync(CelPayCancelSubscriptionDto cancelDto)
        {
            try
            {
                // Configurar autenticação antes de fazer a requisição
                await ConfigurarAutorizacaoAsync();

                var jsonContent = JsonSerializer.Serialize(cancelDto, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
                });
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                _logger.LogInformation("Cancelando subscription via CelPay: {SubscriptionId}", cancelDto.SubscriptionId);

                var response = await _httpClient.PostAsync($"/subscriptions/{cancelDto.SubscriptionId}/cancel", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var subscriptionResponse = JsonSerializer.Deserialize<CelPaySubscriptionResponseDto>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
                    });
                    
                    _logger.LogInformation("Subscription cancelada com sucesso via CelPay: {SubscriptionId}", cancelDto.SubscriptionId);
                    return subscriptionResponse ?? new CelPaySubscriptionResponseDto { Status = "CANCELLED", ErrorMessage = "Resposta inválida do gateway" };
                }
                else
                {
                    _logger.LogError("Erro ao cancelar subscription via CelPay. Status: {Status}, Response: {Response}", response.StatusCode, responseContent);
                    return new CelPaySubscriptionResponseDto 
                    { 
                        Status = "ERROR", 
                        ErrorMessage = $"Erro HTTP: {response.StatusCode}",
                        ErrorCode = response.StatusCode.ToString()
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao cancelar subscription via CelPay");
                return new CelPaySubscriptionResponseDto 
                { 
                    Status = "ERROR", 
                    ErrorMessage = "Erro interno do sistema",
                    ErrorCode = "INTERNAL_ERROR"
                };
            }
        }

        /// <summary>
        /// Atualiza uma subscription no CelPay
        /// </summary>
        public async Task<CelPaySubscriptionResponseDto> AtualizarSubscriptionAsync(CelPayUpdateSubscriptionDto updateDto)
        {
            try
            {
                // Configurar autenticação antes de fazer a requisição
                await ConfigurarAutorizacaoAsync();

                var jsonContent = JsonSerializer.Serialize(updateDto, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
                });
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                _logger.LogInformation("Atualizando subscription via CelPay: {SubscriptionId}", updateDto.SubscriptionId);

                var response = await _httpClient.PutAsync($"/subscriptions/{updateDto.SubscriptionId}", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var subscriptionResponse = JsonSerializer.Deserialize<CelPaySubscriptionResponseDto>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
                    });
                    
                    _logger.LogInformation("Subscription atualizada com sucesso via CelPay: {SubscriptionId}", updateDto.SubscriptionId);
                    return subscriptionResponse ?? new CelPaySubscriptionResponseDto { Status = "UPDATED", ErrorMessage = "Resposta inválida do gateway" };
                }
                else
                {
                    _logger.LogError("Erro ao atualizar subscription via CelPay. Status: {Status}, Response: {Response}", response.StatusCode, responseContent);
                    return new CelPaySubscriptionResponseDto 
                    { 
                        Status = "ERROR", 
                        ErrorMessage = $"Erro HTTP: {response.StatusCode}",
                        ErrorCode = response.StatusCode.ToString()
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao atualizar subscription via CelPay");
                return new CelPaySubscriptionResponseDto 
                { 
                    Status = "ERROR", 
                    ErrorMessage = "Erro interno do sistema",
                    ErrorCode = "INTERNAL_ERROR"
                };
            }
        }
    }
}
