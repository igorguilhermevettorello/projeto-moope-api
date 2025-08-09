using FluentValidation;

namespace Projeto.Moope.Core.Models.Validators
{
    public class PedidoValidator : AbstractValidator<Pedido>
    {
        public PedidoValidator()
        {
            RuleFor(x => x.ClienteId).NotEmpty();
            RuleFor(x => x.RevendedorId).NotEmpty();
            RuleFor(x => x.Total).GreaterThanOrEqualTo(0);
            RuleFor(x => x.Status).NotEmpty().MaximumLength(50);
        }
    }
}