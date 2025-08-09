using System;

namespace Projeto.Moope.API.DTOs
{
    public class PessoaJuridicaDto
    {
        public Guid ClienteId { get; set; }
        public string Cnpj { get; set; }
        public string RazaoSocial { get; set; }
        public string NomeFantasia { get; set; }
        public string InscricaoEstadual { get; set; }
    }
} 