using System.ComponentModel.DataAnnotations;

namespace Projeto.Moope.API.DTOs.Auth
{
    public class LoginDto
    {
        [Required(ErrorMessage = "O E-mail � obrigat�rio.")]
        [EmailAddress(ErrorMessage = "E-mail inv�lido.")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "A {0} � obrigat�ria.")]
        public string? Senha { get; set; }

        [Required(ErrorMessage = "O captcha � obrigat�rio.")]
        public string? RecaptchaToken { get; set; }
    }
}