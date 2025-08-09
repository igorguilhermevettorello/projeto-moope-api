using FluentValidation;

namespace Projeto.Moope.Core.Models.Validators
{
    public class PessoaFisicaValidator : AbstractValidator<PessoaFisica>
    {
        public PessoaFisicaValidator()
        {
            RuleFor(x => x.ClienteId).NotEmpty();
            RuleFor(x => x.Nome).NotEmpty().MaximumLength(255);
            RuleFor(x => x.Cpf).NotEmpty().Length(11);
        }
    }
}