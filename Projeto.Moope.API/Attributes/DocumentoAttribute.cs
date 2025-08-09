using Microsoft.Extensions.Options;
using Projeto.Moope.Core.Enums;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Projeto.Moope.API.Attributes
{
    public class DocumentoAttribute : ValidationAttribute
    {
        private readonly string _tipoPessoaCampo;

        public DocumentoAttribute(string tipoPessoaCampo)
        {
            _tipoPessoaCampo = tipoPessoaCampo;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string documento = Convert.ToString(value)?.Trim().Replace(".", "").Replace("-", "").Replace("/", "");

            var tipoPessoaProp = validationContext.ObjectType.GetProperty(_tipoPessoaCampo);
            if (tipoPessoaProp == null)
                return new ValidationResult($"Campo '{_tipoPessoaCampo}' não encontrado.");

            var tipoPessoa = (TipoPessoa)tipoPessoaProp.GetValue(validationContext.ObjectInstance);
            if (tipoPessoa == null)
                return new ValidationResult("Tipo de pessoa não informado.");

            bool valido = tipoPessoa == TipoPessoa.FISICA
                ? ValidarCPF(documento)
                : ValidarCNPJ(documento);

            return valido
                ? ValidationResult.Success
                : new ValidationResult(ErrorMessage ?? "Documento inválido.");
        }

        private bool ValidarCPF(string cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf) || cpf.Length != 11 || Regex.IsMatch(cpf, @"^(\d)\1+$"))
                return false;

            var soma = 0;
            for (int i = 0; i < 9; i++)
                soma += (cpf[i] - '0') * (10 - i);

            var resto = (soma * 10) % 11;
            if (resto == 10) resto = 0;
            if (resto != (cpf[9] - '0')) return false;

            soma = 0;
            for (int i = 0; i < 10; i++)
                soma += (cpf[i] - '0') * (11 - i);

            resto = (soma * 10) % 11;
            if (resto == 10) resto = 0;

            return resto == (cpf[10] - '0');
        }

        private bool ValidarCNPJ(string cnpj)
        {
            if (string.IsNullOrWhiteSpace(cnpj) || cnpj.Length != 14 || Regex.IsMatch(cnpj, @"^(\d)\1+$"))
                return false;

            int[] multiplicador1 = { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

            string temp = cnpj.Substring(0, 12);
            int soma = 0;

            for (int i = 0; i < 12; i++)
                soma += (temp[i] - '0') * multiplicador1[i];

            int resto = soma % 11;
            resto = resto < 2 ? 0 : 11 - resto;

            temp += resto;
            soma = 0;

            for (int i = 0; i < 13; i++)
                soma += (temp[i] - '0') * multiplicador2[i];

            resto = soma % 11;
            resto = resto < 2 ? 0 : 11 - resto;

            return cnpj.EndsWith(resto.ToString());
        }
    }
}
