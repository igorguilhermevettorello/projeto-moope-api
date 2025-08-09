using System;

namespace Projeto.Moope.API.DTOs
{
    public class RevendedorDto
    {
        public Guid Id { get; set; }
        public Guid PapelId { get; set; }
        public string PapelNome { get; set; }
        public decimal PercentualComissao { get; set; }
        public Guid? RevendedorId { get; set; }
        public Guid EnderecoId { get; set; }
        // Adicione outros campos relevantes se necessário
    }
} 