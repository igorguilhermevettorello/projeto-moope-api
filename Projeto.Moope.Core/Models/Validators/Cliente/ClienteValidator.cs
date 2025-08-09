using FluentValidation;
using ClienteModel = Projeto.Moope.Core.Models.Cliente;

namespace Projeto.Moope.Core.Models.Validators.Cliente
{
    public class ClienteValidator : AbstractValidator<ClienteModel>
    {
        public ClienteValidator()
        {
            //RuleFor(x => x.UsuarioId).NotEmpty();
            RuleFor(x => x.TipoPessoa).NotEmpty();
            RuleFor(x => x.Ativo).NotNull();
        }
    }
}