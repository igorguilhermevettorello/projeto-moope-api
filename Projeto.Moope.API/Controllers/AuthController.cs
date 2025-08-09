using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Projeto.Moope.API.Controllers.Base;
using Projeto.Moope.API.DTOs;
using Projeto.Moope.API.DTOs.Auth;
using Projeto.Moope.API.DTOs.Usuarios;
using Projeto.Moope.API.Utils;
using Projeto.Moope.Core.Enums;
using Projeto.Moope.Core.Interfaces.Identity;
using Projeto.Moope.Core.Interfaces.Notifications;
using Projeto.Moope.Core.Interfaces.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Projeto.Moope.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : MainController
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly JwtSettings _jwtSettings;
        private readonly ILogger _logger;
        private readonly IGoogleRecaptchaService _recaptchaService;

        private string[] ErrorPassowrd = { "PasswordTooShort", "PasswordRequiresNonAlphanumeric", "PasswordRequiresLower", "PasswordRequiresUpper", "PasswordRequiresDigit" };
        private string[] ErrorEmail = { "DuplicateUserName" };

        public AuthController(
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            IOptions<JwtSettings> config,
            ILogger<AuthController> logger,
            IGoogleRecaptchaService recaptchaService,
            INotificador notificador,
            IUser user) : base(notificador, user)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _jwtSettings = config.Value;
            _logger = logger;
            _recaptchaService = recaptchaService;
        }

        [HttpPost("registrar")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Register([FromBody] AuthRegistroDto registerDto)
        {
            // Lógica de registro de usuário aqui

            if (!ModelState.IsValid) return CustomResponse(ModelState);

            //var fornecedor = _mapper.Map<Fornecedor>(fornecedorDto);

            //var result = await _fornecedorService.Adicionar(fornecedor);

            //if (!result.Status) return CustomResponse();

            //fornecedorDto = _mapper.Map<FornecedorDto>(result.Dados);

            return CustomResponse(registerDto);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {

            //_logger.LogInformation("teste");
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            var isValid = await _recaptchaService.VerifyTokenAsync(loginDto.RecaptchaToken);
            if (!isValid)
            {
                ModelState.AddModelError("Senha", "O captcha está inválido.");
                return CustomResponse(ModelState);
            }

            var result = await _signInManager.PasswordSignInAsync(loginDto.Email, loginDto.Senha, false, true);

            if (result.Succeeded)
            {
                var token = await GerarJwt(loginDto.Email);
                return CustomResponse(new { data = token });
            }
            else if (result.IsLockedOut)
            {
                ModelState.AddModelError("Senha", "Usuário temporariamente bloqueado por tentativas inválidas.");
                return CustomResponse(ModelState);
            }
            else if (!result.Succeeded)
            {
                ModelState.AddModelError("Senha", "E-mail ou senha inválidos.");
                return CustomResponse(ModelState);
            }
            else
            {
                ModelState.AddModelError("Senha", "E-mail ou senha inválidos.");
                return CustomResponse(ModelState);
            }
        }
        private async Task<LoginResponseDto> GerarJwt(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            var claims = await _userManager.GetClaimsAsync(user);
            var userRoles = await _userManager.GetRolesAsync(user);

            // Pegue a primeira role (ou adapte para múltiplas)
            var role = userRoles.FirstOrDefault();
            TipoUsuario? tipoUsuario = null;
            if (Enum.TryParse<TipoUsuario>(role, out var parsedTipo))
            {
                tipoUsuario = parsedTipo;
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddHours(_jwtSettings.ExpiracaoHoras);

            claims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.Id));
            claims.Add(new Claim(JwtRegisteredClaimNames.Email, user.Email));
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Nbf, ToUnixEpochDate(DateTime.UtcNow).ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(DateTime.UtcNow).ToString(), ClaimValueTypes.Integer64));
            foreach (var userRole in userRoles)
            {
                claims.Add(new Claim("role", userRole));
            }

            if (tipoUsuario != null)
            {
                claims.Add(new Claim("perfil", tipoUsuario.ToString().ToLower()));
            }

            var identityClaims = new ClaimsIdentity();
            identityClaims.AddClaims(claims);
            var token = new SecurityTokenDescriptor
            {
                Subject = identityClaims,
                Expires = expires,
                SigningCredentials = creds,
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(token);
            var tokenString = tokenHandler.WriteToken(securityToken);

            var loginResponseDto = new LoginResponseDto
            {
                AccessToken = tokenString,
                ExpiresIn = TimeSpan.FromHours(_jwtSettings.ExpiracaoHoras).TotalSeconds,
                User = new LoginUsuarioDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    Claims = claims.Select(c => new ClaimDto { Type = c.Type, Value = c.Value }),
                    Perfil = tipoUsuario?.ToString().ToLower()
                }
            };

            return loginResponseDto;
        }

        private static long ToUnixEpochDate(DateTime date)
            => (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);
    }
} 