using Projeto.Moope.Core.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Projeto.Moope.Core.Models
{
    [Table("Cliente")]
    public class Cliente
    {
        [Key]
        public Guid Id { get; set; }
        public Guid? PapelId { get; set; }
        public Papel? Papel { get; set; }
        public TipoPessoa TipoPessoa { get; set; }
        public bool Ativo { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public Usuario Usuario { get; set; }
    }
} 