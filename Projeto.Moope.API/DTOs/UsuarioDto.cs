using System;

namespace Projeto.Moope.API.DTOs
{
    public class UsuarioDto
    {
        public Guid? Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Telefone { get; set; }
        public string Tipo { get; set; }
        public bool Ativo { get; set; }
        public Guid? EnderecoId { get; set; }
    }
} 