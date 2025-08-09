using System;

namespace Projeto.Moope.API.DTOs
{
    public class TransacaoDto
    {
        public Guid? Id { get; set; }
        public Guid PedidoId { get; set; }
        public decimal Valor { get; set; }
        public DateTime DataPagamento { get; set; }
        public string Status { get; set; }
        public string MetodoPagamento { get; set; }
    }
} 