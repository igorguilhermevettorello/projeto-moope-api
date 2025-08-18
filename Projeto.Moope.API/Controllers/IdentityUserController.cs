using Microsoft.AspNetCore.Mvc;
using Projeto.Moope.API.Controllers.Base;
using Projeto.Moope.API.DTOs.Usuarios;
using Projeto.Moope.Core.Interfaces.Identity;
using Projeto.Moope.Core.Interfaces.Notifications;
using Projeto.Moope.Core.Interfaces.Services;

namespace Projeto.Moope.API.Controllers
{
    [ApiController]
    [Route("api/identity-user")]
    public class IdentityUserController : MainController
    {
        private readonly IIdentityUserService _identityUserService;
        private readonly INotificador _notificador;

        public IdentityUserController(
            IIdentityUserService identityUserService,
            INotificador notificador,
            IUser user) : base(notificador, user)
        {
            _identityUserService = identityUserService;
            _notificador = notificador;
        }

        [HttpPost("criar")]
        [ProducesResponseType(typeof(IdentityUserResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> CriarUsuarioAsync([FromBody] CreateIdentityUserDto createDto)
        {
            // if (!ModelState.IsValid)
            //     return CustomResponse(ModelState);
            //
            // var resultado = await _identityUserService.CriarUsuarioAsync(
            //     createDto.Email, 
            //     createDto.Senha, 
            //     createDto.Telefone);
            //
            // if (!resultado.Succeeded)
            // {
            //     var response = new IdentityUserResponseDto
            //     {
            //         Sucesso = false,
            //         Erros = resultado.Errors.Select(e => e.Description).ToList()
            //     };
            //     return BadRequest(response);
            // }
            //
            // var usuario = await _identityUserService.BuscarPorEmailAsync(createDto.Email);
            // string tokenConfirmacao = null;
            //
            // if (createDto.RequerConfirmacaoEmail)
            // {
            //     tokenConfirmacao = await _identityUserService.GerarTokenConfirmacaoEmailAsync(usuario.Id);
            // }
            //
            // var response = new IdentityUserResponseDto
            // {
            //     Id = usuario.Id,
            //     Email = usuario.Email,
            //     Telefone = usuario.PhoneNumber,
            //     EmailConfirmed = usuario.EmailConfirmed,
            //     PhoneNumberConfirmed = usuario.PhoneNumberConfirmed,
            //     TwoFactorEnabled = usuario.TwoFactorEnabled,
            //     LockoutEnabled = usuario.LockoutEnabled,
            //     AccessFailedCount = usuario.AccessFailedCount,
            //     Sucesso = true,
            //     TokenConfirmacaoEmail = tokenConfirmacao
            // };

            return Ok();
        }

        // [HttpPost("alterar-senha")]
        // [ProducesResponseType(StatusCodes.Status200OK)]
        // [ProducesResponseType(StatusCodes.Status400BadRequest)]
        // [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        // [ProducesResponseType(StatusCodes.Status403Forbidden)]
        // public async Task<IActionResult> AlterarSenhaAsync([FromBody] AlterarSenhaDto alterarSenhaDto)
        // {
        //     if (!ModelState.IsValid)
        //         return CustomResponse(ModelState);
        //
        //     var resultado = await _identityUserService.AlterarSenhaAsync(
        //         alterarSenhaDto.UserId,
        //         alterarSenhaDto.SenhaAtual,
        //         alterarSenhaDto.NovaSenha);
        //
        //     if (!resultado.Succeeded)
        //     {
        //         var erros = resultado.Errors.Select(e => e.Description).ToList();
        //         return BadRequest(new { Sucesso = false, Erros = erros });
        //     }
        //
        //     return Ok(new { Sucesso = true, Mensagem = "Senha alterada com sucesso" });
        // }
        //
        // [HttpPost("resetar-senha")]
        // [ProducesResponseType(StatusCodes.Status200OK)]
        // [ProducesResponseType(StatusCodes.Status400BadRequest)]
        // public async Task<IActionResult> ResetarSenhaAsync([FromBody] ResetarSenhaDto resetarSenhaDto)
        // {
        //     if (!ModelState.IsValid)
        //         return CustomResponse(ModelState);
        //
        //     var resultado = await _identityUserService.ResetarSenhaAsync(
        //         resetarSenhaDto.Email,
        //         resetarSenhaDto.Token,
        //         resetarSenhaDto.NovaSenha);
        //
        //     if (!resultado.Succeeded)
        //     {
        //         var erros = resultado.Errors.Select(e => e.Description).ToList();
        //         return BadRequest(new { Sucesso = false, Erros = erros });
        //     }
        //
        //     return Ok(new { Sucesso = true, Mensagem = "Senha resetada com sucesso" });
        // }
        //
        // [HttpPost("gerar-token-reset")]
        // [ProducesResponseType(StatusCodes.Status200OK)]
        // [ProducesResponseType(StatusCodes.Status400BadRequest)]
        // public async Task<IActionResult> GerarTokenResetAsync([FromBody] GerarTokenResetDto gerarTokenDto)
        // {
        //     if (!ModelState.IsValid)
        //         return CustomResponse(ModelState);
        //
        //     var token = await _identityUserService.GerarTokenResetSenhaAsync(gerarTokenDto.Email);
        //
        //     if (token == null)
        //     {
        //         return BadRequest(new { Sucesso = false, Mensagem = "Não foi possível gerar o token" });
        //     }
        //
        //     return Ok(new { Sucesso = true, Token = token });
        // }
        //
        // [HttpPost("confirmar-email")]
        // [ProducesResponseType(StatusCodes.Status200OK)]
        // [ProducesResponseType(StatusCodes.Status400BadRequest)]
        // public async Task<IActionResult> ConfirmarEmailAsync([FromBody] ConfirmarEmailDto confirmarEmailDto)
        // {
        //     if (!ModelState.IsValid)
        //         return CustomResponse(ModelState);
        //
        //     var sucesso = await _identityUserService.ConfirmarEmailAsync(
        //         confirmarEmailDto.UserId,
        //         confirmarEmailDto.Token);
        //
        //     if (!sucesso)
        //     {
        //         return BadRequest(new { Sucesso = false, Mensagem = "Não foi possível confirmar o email" });
        //     }
        //
        //     return Ok(new { Sucesso = true, Mensagem = "Email confirmado com sucesso" });
        // }
        //
        // [HttpPost("gerar-token-confirmacao")]
        // [ProducesResponseType(StatusCodes.Status200OK)]
        // [ProducesResponseType(StatusCodes.Status400BadRequest)]
        // public async Task<IActionResult> GerarTokenConfirmacaoAsync([FromBody] GerarTokenConfirmacaoDto gerarTokenDto)
        // {
        //     if (!ModelState.IsValid)
        //         return CustomResponse(ModelState);
        //
        //     var token = await _identityUserService.GerarTokenConfirmacaoEmailAsync(gerarTokenDto.UserId);
        //
        //     if (token == null)
        //     {
        //         return BadRequest(new { Sucesso = false, Mensagem = "Não foi possível gerar o token" });
        //     }
        //
        //     return Ok(new { Sucesso = true, Token = token });
        // }
        //
        // [HttpGet("buscar-por-email/{email}")]
        // [ProducesResponseType(typeof(IdentityUserResponseDto), StatusCodes.Status200OK)]
        // [ProducesResponseType(StatusCodes.Status404NotFound)]
        // public async Task<IActionResult> BuscarPorEmailAsync(string email)
        // {
        //     var usuario = await _identityUserService.BuscarPorEmailAsync(email);
        //
        //     if (usuario == null)
        //         return NotFound("Usuário não encontrado");
        //
        //     var response = new IdentityUserResponseDto
        //     {
        //         Id = usuario.Id,
        //         Email = usuario.Email,
        //         Telefone = usuario.PhoneNumber,
        //         EmailConfirmed = usuario.EmailConfirmed,
        //         PhoneNumberConfirmed = usuario.PhoneNumberConfirmed,
        //         TwoFactorEnabled = usuario.TwoFactorEnabled,
        //         LockoutEnabled = usuario.LockoutEnabled,
        //         AccessFailedCount = usuario.AccessFailedCount,
        //         Sucesso = true
        //     };
        //
        //     return Ok(response);
        // }
        //
        // [HttpPost("verificar-senha")]
        // [ProducesResponseType(StatusCodes.Status200OK)]
        // [ProducesResponseType(StatusCodes.Status400BadRequest)]
        // public async Task<IActionResult> VerificarSenhaAsync([FromBody] VerificarSenhaDto verificarSenhaDto)
        // {
        //     if (!ModelState.IsValid)
        //         return CustomResponse(ModelState);
        //
        //     var senhaValida = await _identityUserService.VerificarSenhaAsync(
        //         verificarSenhaDto.UserId,
        //         verificarSenhaDto.Senha);
        //
        //     return Ok(new { Sucesso = true, SenhaValida = senhaValida });
        // }
        //
        // [HttpPost("bloquear/{userId}")]
        // [ProducesResponseType(StatusCodes.Status200OK)]
        // [ProducesResponseType(StatusCodes.Status400BadRequest)]
        // public async Task<IActionResult> BloquearUsuarioAsync(string userId)
        // {
        //     var resultado = await _identityUserService.BloquearUsuarioAsync(userId);
        //
        //     if (!resultado.Succeeded)
        //     {
        //         var erros = resultado.Errors.Select(e => e.Description).ToList();
        //         return BadRequest(new { Sucesso = false, Erros = erros });
        //     }
        //
        //     return Ok(new { Sucesso = true, Mensagem = "Usuário bloqueado com sucesso" });
        // }
        //
        // [HttpPost("desbloquear/{userId}")]
        // [ProducesResponseType(StatusCodes.Status200OK)]
        // [ProducesResponseType(StatusCodes.Status400BadRequest)]
        // public async Task<IActionResult> DesbloquearUsuarioAsync(string userId)
        // {
        //     var resultado = await _identityUserService.DesbloquearUsuarioAsync(userId);
        //
        //     if (!resultado.Succeeded)
        //     {
        //         var erros = resultado.Errors.Select(e => e.Description).ToList();
        //         return BadRequest(new { Sucesso = false, Erros = erros });
        //     }
        //
        //     return Ok(new { Sucesso = true, Mensagem = "Usuário desbloqueado com sucesso" });
        // }
        //
        // [HttpPost("ativar/{userId}")]
        // [ProducesResponseType(StatusCodes.Status200OK)]
        // [ProducesResponseType(StatusCodes.Status400BadRequest)]
        // public async Task<IActionResult> AtivarUsuarioAsync(string userId)
        // {
        //     var resultado = await _identityUserService.AtivarUsuarioAsync(userId);
        //
        //     if (!resultado.Succeeded)
        //     {
        //         var erros = resultado.Errors.Select(e => e.Description).ToList();
        //         return BadRequest(new { Sucesso = false, Erros = erros });
        //     }
        //
        //     return Ok(new { Sucesso = true, Mensagem = "Usuário ativado com sucesso" });
        // }
        //
        // [HttpPost("desativar/{userId}")]
        // [ProducesResponseType(StatusCodes.Status200OK)]
        // [ProducesResponseType(StatusCodes.Status400BadRequest)]
        // public async Task<IActionResult> DesativarUsuarioAsync(string userId)
        // {
        //     var resultado = await _identityUserService.DesativarUsuarioAsync(userId);
        //
        //     if (!resultado.Succeeded)
        //     {
        //         var erros = resultado.Errors.Select(e => e.Description).ToList();
        //         return BadRequest(new { Sucesso = false, Erros = erros });
        //     }
        //
        //     return Ok(new { Sucesso = true, Mensagem = "Usuário desativado com sucesso" });
        // }
    }

    // DTOs auxiliares
    public class AlterarSenhaDto
    {
        public string UserId { get; set; }
        public string SenhaAtual { get; set; }
        public string NovaSenha { get; set; }
    }

    public class ResetarSenhaDto
    {
        public string Email { get; set; }
        public string Token { get; set; }
        public string NovaSenha { get; set; }
    }

    public class GerarTokenResetDto
    {
        public string Email { get; set; }
    }

    public class ConfirmarEmailDto
    {
        public string UserId { get; set; }
        public string Token { get; set; }
    }

    public class GerarTokenConfirmacaoDto
    {
        public string UserId { get; set; }
    }

    public class VerificarSenhaDto
    {
        public string UserId { get; set; }
        public string Senha { get; set; }
    }
}
