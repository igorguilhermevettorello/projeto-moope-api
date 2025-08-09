using Projeto.Moope.Core.Models.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Projeto.Moope.Core.Models
{
    [Table("Pedido")]
    public class Pedido : Entity
    {
        public Guid ClienteId { get; set; }
        public Guid RevendedorId { get; set; }
        public decimal Total { get; set; }
        public string Status { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public Cliente Cliente { get; set; }
        public Revendedor Revendedor { get; set; }
    }
} 