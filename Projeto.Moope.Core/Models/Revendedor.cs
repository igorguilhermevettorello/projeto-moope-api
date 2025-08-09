using Projeto.Moope.Core.Models.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Projeto.Moope.Core.Models
{
    [Table("Revendedor")]
    public class Revendedor : Entity
    {
        public Guid PapelId { get; set; }
        public Papel Papel { get; set; }
        public string Cnpj { get; set; }
        public string RazaoSocial { get; set; }
        public decimal PercentualComissao { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public Guid? RevendedorId { get; set; }
        public Revendedor RevendedorPai { get; set; }
        public ICollection<Revendedor> RevendedoresFilhos { get; set; }
        public Guid EnderecoId { get; set; }
        public Endereco Endereco { get; set; }
        public Usuario Usuario { get; set; }
    }
} 