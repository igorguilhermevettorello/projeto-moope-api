using FluentValidation;

namespace Projeto.Moope.Core.Models.Validators
{
    //public class RevendedorValidator : AbstractValidator<Revendedor>
    //{
    //    public RevendedorValidator()
    //    {
    //        RuleFor(x => x.Cnpj).NotEmpty().Length(14);
    //        RuleFor(x => x.RazaoSocial).NotEmpty().MaximumLength(255);
    //        RuleFor(x => x.PercentualComissao).InclusiveBetween(0, 100);
    //    }
    //}

    public class RevendedorValidator : AbstractValidator<Revendedor>
    {
        public RevendedorValidator()
        {
            //RuleFor(f => f.RazaoSocial)
            //    .NotEmpty().WithMessage("O campo Raz�o Social precisa ser fornecido")
            //    .Length(2, 100)
            //    .WithMessage("O campo Raz�o Social precisa ter entre {MinLength} e {MaxLength} caracteres")
            //    .OverridePropertyName("RazaoSocial");

            //RuleFor(x => x.Codigo)
            //    .NotEmpty().WithMessage("O C�digo � obrigat�rio.")
            //    .OverridePropertyName("Codigo");

            //RuleFor(x => x.Descricao)
            //    .NotEmpty().WithMessage("A Descri��o � obrigat�ria.")
            //    .Length(2, 100).WithMessage("O campo Descri��o precisa ter entre {MinLength} e {MaxLength} caracteres")
            //    .OverridePropertyName("Descricao");

            //RuleFor(x => x.Valor)
            //    .GreaterThan(0).WithMessage("O valor deve ser maior que zero.")
            //    .OverridePropertyName("Valor");
        }
    }
}