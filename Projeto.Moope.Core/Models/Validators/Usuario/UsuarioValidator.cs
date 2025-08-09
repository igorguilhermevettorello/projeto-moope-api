using FluentValidation;
using UsuarioModel = Projeto.Moope.Core.Models.Usuario;

namespace Projeto.Moope.Core.Models.Validators.Usuario
{
    public class UsuarioValidator : AbstractValidator<UsuarioModel>
    {
        public UsuarioValidator()
        {
            RuleFor(c => c.Nome)
                .NotEmpty().WithMessage("O campo {PropertyName} precisa ser fornecido")
                .Length(2, 200).WithMessage("O campo {PropertyName} precisa ter entre {MinLength} e {MaxLength} caracteres")
                .OverridePropertyName("Nome");

            RuleFor(c => c.Email)
                .NotEmpty().WithMessage("O campo {PropertyName} precisa ser fornecido")
                .EmailAddress().WithMessage("O campo {PropertyName} é inválido")
                .OverridePropertyName("Email");

            RuleFor(c => c.Telefone)
                .NotEmpty().WithMessage("O campo {PropertyName} precisa ser fornecido")
                .OverridePropertyName("Telefone");

        }
    }
}