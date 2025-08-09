using System;

namespace Projeto.Moope.API.DTOs
{
    public class PessoaFisicaDto
    {
        public Guid ClienteId { get; set; }
        public string Nome { get; set; }
        public string Cpf { get; set; }
    }
} 