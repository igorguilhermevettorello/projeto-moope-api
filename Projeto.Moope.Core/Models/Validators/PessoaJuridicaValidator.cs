using FluentValidation;

namespace Projeto.Moope.Core.Models.Validators
{
    public class PessoaJuridicaValidator : AbstractValidator<PessoaJuridica>
    {
        public PessoaJuridicaValidator()
        {
            RuleFor(x => x.Cnpj).NotEmpty().Length(14);
            RuleFor(x => x.RazaoSocial).NotEmpty().MaximumLength(255);
            RuleFor(x => x.NomeFantasia).MaximumLength(255);
            RuleFor(x => x.InscricaoEstadual).MaximumLength(20);
        }
    }
}