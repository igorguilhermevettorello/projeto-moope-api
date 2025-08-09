using System.ComponentModel.DataAnnotations;

namespace Projeto.Moope.API.DTOs.Auth
{
    public class AuthRegistroDto
    {
        [Required(ErrorMessage = "O campo {0} � obrigat�rio")]
        [StringLength(100, ErrorMessage = "O campo {0} precisa ter entre {2} e {1} caracteres", MinimumLength = 2)]
        public string Nome { get; set; }
        [Required(ErrorMessage = "O campo {0} � obrigat�rio")]
        [EmailAddress(ErrorMessage = "O {0} � inv�lido")]
        public string Email { get; set; }
        [Required(ErrorMessage = "O campo {0} � obrigat�rio")]
        [StringLength(14, ErrorMessage = "O campo {0} precisa ter entre {2} e {1} caracteres", MinimumLength = 13)]
        public string Telefone { get; set; }
        [Required(ErrorMessage = "O campo {0} � obrigat�rio")]
        public string Cpf { get; set; }
    }
}
