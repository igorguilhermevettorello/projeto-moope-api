using Projeto.Moope.Core.Models.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Projeto.Moope.Core.Models
{
    [Table("PessoaFisica")]
    public class PessoaFisica : Entity
    {
        public Guid ClienteId { get; set; }
        public string Nome { get; set; }
        public string Cpf { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public Cliente Cliente { get; set; }
    }
} 