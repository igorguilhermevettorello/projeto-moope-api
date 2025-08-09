using AutoMapper;
using Projeto.Moope.API.DTOs;
using Projeto.Moope.API.DTOs.Clientes;
using Projeto.Moope.API.DTOs.Enderecos;
using Projeto.Moope.API.DTOs.Planos;
using Projeto.Moope.Core.Models;

namespace Projeto.Moope.API.Configurations
{
    public class AutomapperConfig : Profile
    {
        public AutomapperConfig()
        {
            //CreateMap<Fornecedor, FornecedorDto>().ReverseMap();
            //CreateMap<Endereco, EnderecoDto>().ReverseMap();
            CreateMap<Plano, PlanoDto>().ReverseMap();
            //CreateMap<Categoria, CategoriaDto>().ReverseMap();
            
            // Mapeamentos do Cliente
            CreateMap<Cliente, ClienteDto>()
                .ForMember(dest => dest.PapelNome, opt => opt.MapFrom(src => src.Papel.Nome.ToString()));
            CreateMap<CreateClienteDto, Cliente>();
            CreateMap<UpdateClienteDto, Cliente>();
            CreateMap<Endereco, CreateEnderecoDto>().ReverseMap();
        }
    }
}
