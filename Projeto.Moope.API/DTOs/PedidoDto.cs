using System;

namespace Projeto.Moope.API.DTOs
{
    public class PedidoDto
    {
        public Guid? Id { get; set; }
        public Guid ClienteId { get; set; }
        public Guid RevendedorId { get; set; }
        public Guid PrepostoId { get; set; }
        public decimal Total { get; set; }
        public string Status { get; set; }
    }
} 