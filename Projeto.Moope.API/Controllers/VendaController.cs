using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Projeto.Moope.API.Controllers.Base;
using Projeto.Moope.Core.DTOs.Vendas;
using Projeto.Moope.Core.Interfaces.Identity;
using Projeto.Moope.Core.Interfaces.Notifications;
using Projeto.Moope.Core.Interfaces.Services;

namespace Projeto.Moope.API.Controllers
{
    [ApiController]
    [Route("api/venda")]
    [Authorize]
    public class VendaController : MainController
    {
        private readonly IVendaService _vendaService;
        private readonly IIdentityUserService _identityUserService;
        
        public VendaController(
            IVendaService vendaService,
            IIdentityUserService identityUserService,
            INotificador notificador,
            IUser user) : base(notificador, user)
        {
            _vendaService = vendaService;
            _identityUserService = identityUserService;
        }

        [AllowAnonymous]
        [HttpPost("processar")]
        //[Authorize(Roles = $"{nameof(TipoUsuario.Vendedor)},{nameof(TipoUsuario.Administrador)}")]
        [ProducesResponseType(typeof(CreateVendaDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> ProcessarVenda([FromBody] CreateVendaDto vendaDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var clinte = await _identityUserService.BuscarPorEmailAsync(vendaDto.Email);
                // if (clinte == null)
                // {
                //     throw new Exception("teste");
                // }
                vendaDto.ClienteId = clinte.Id;
                
                
                var resultado = await _vendaService.ProcessarVendaAsync(vendaDto);
                
                if (resultado.Sucesso)
                {
                    return Ok(resultado);
                }
                else
                {
                    return BadRequest(resultado);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Erro interno ao processar venda", details = ex.Message });
            }
        }

        
        [HttpGet("{vendaId:guid}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> ConsultarVenda(Guid vendaId)
        {
            try
            {
                var venda = await _vendaService.ConsultarVendaAsync(vendaId);
                
                if (venda.Id == Guid.Empty)
                {
                    return NotFound(new { error = "Venda não encontrada" });
                }

                return Ok(venda);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Erro interno ao consultar venda", details = ex.Message });
            }
        }

        [HttpGet("vendedor/{vendedorId:guid}")]
        public async Task<IActionResult> ListarVendasPorVendedor(Guid vendedorId)
        {
            try
            {
                var vendas = await _vendaService.ListarVendasPorVendedorAsync(vendedorId);
                return Ok(vendas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Erro interno ao listar vendas", details = ex.Message });
            }
        }

        [HttpGet("cliente/{email}")]
        public async Task<IActionResult> ListarVendasPorCliente(string email)
        {
            try
            {
                var vendas = await _vendaService.ListarVendasPorClienteAsync(email);
                return Ok(vendas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Erro interno ao listar vendas do cliente", details = ex.Message });
            }
        }
        
        [HttpGet("health")]
        [AllowAnonymous]
        public IActionResult HealthCheck()
        {
            return Ok(new { 
                status = "OK", 
                message = "API de Vendas funcionando normalmente",
                timestamp = DateTime.UtcNow,
                gateway = "CelPay"
            });
        }
    }
}
