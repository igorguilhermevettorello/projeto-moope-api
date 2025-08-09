using System;

namespace Projeto.Moope.API.DTOs
{
    public class ClienteDto
    {
        public Guid Id { get; set; }
        public Guid PapelId { get; set; }
        public string PapelNome { get; set; }
        public bool Ativo { get; set; }
        // Adicione outros campos relevantes se necessário
    }
} 