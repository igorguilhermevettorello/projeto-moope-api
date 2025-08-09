using Projeto.Moope.Core.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Projeto.Moope.Core.Models
{

    [Table("Papel")]
    public class Papel
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public TipoPapel Nome { get; set; } 
        [Required]
        public Guid PessoaJuridicaId { get; set; }
        public PessoaJuridica PessoaJuridica { get; set; }
    }
} 