using AutoMapper;
using Projeto.Moope.API.DTOs;
using Projeto.Moope.API.DTOs.Clientes;
using Projeto.Moope.API.DTOs.Enderecos;
using Projeto.Moope.API.DTOs.Planos;
using Projeto.Moope.API.DTOs.Revendedor;
using Projeto.Moope.API.DTOs.Vendas;
using Projeto.Moope.Core.Models;

namespace Projeto.Moope.API.Configurations
{
    public class AutomapperConfig : Profile
    {
        public AutomapperConfig()
        {
            CreateMap<Plano, PlanoDto>().ReverseMap();
            
            // Mapeamentos do Cliente
            // CreateMap<Cliente, ClienteDto>()
            //     .ForMember(dest => dest.PapelNome, opt => opt.MapFrom(src => src.Papel.Nome.ToString()));

            // cliente
            
            CreateMap<Cliente, ListClienteDto>().ReverseMap();
            
            CreateMap<CreateClienteDto, Cliente>();
            CreateMap<CreateClienteDto, Usuario>();
            CreateMap<CreateClienteDto, PessoaFisica>()
                .ForMember(dest => dest.Cpf, opt => opt.MapFrom(src => src.CpfCnpj));
            
            CreateMap<CreateClienteDto, PessoaJuridica>()
                .ForMember(dest => dest.Cnpj, opt => opt.MapFrom(src => src.CpfCnpj))
                .ForMember(dest => dest.RazaoSocial, opt => opt.MapFrom(src => src.Nome));
            
            CreateMap<UpdateClienteDto, PessoaFisica>()
                .ForMember(dest => dest.Cpf, opt => opt.MapFrom(src => src.CpfCnpj));
            
            CreateMap<UpdateClienteDto, PessoaJuridica>()
                .ForMember(dest => dest.Cnpj, opt => opt.MapFrom(src => src.CpfCnpj))
                .ForMember(dest => dest.RazaoSocial, opt => opt.MapFrom(src => src.Nome));
            
            CreateMap<UpdateClienteDto, Cliente>();
            CreateMap<UpdateClienteDto, Usuario>();
            CreateMap<Endereco, CreateEnderecoDto>().ReverseMap();
            CreateMap<UpdateEnderecoDto, Endereco>();

            CreateMap<Usuario, Usuario>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Created, opt => opt.Ignore());
            
            // vendedor
            
            CreateMap<CreateVendedorDto, Vendedor>().ReverseMap();
            CreateMap<CreateVendedorDto, Usuario>();
            CreateMap<CreateVendedorDto, PessoaFisica>()
                .ForMember(dest => dest.Cpf, opt => opt.MapFrom(src => src.CpfCnpj));
            
            CreateMap<CreateVendedorDto, PessoaJuridica>()
                .ForMember(dest => dest.Cnpj, opt => opt.MapFrom(src => src.CpfCnpj))
                .ForMember(dest => dest.RazaoSocial, opt => opt.MapFrom(src => src.Nome));
            
            CreateMap<UpdateVendedorDto, Vendedor>();
            CreateMap<UpdateVendedorDto, Usuario>();
            CreateMap<UpdateVendedorDto, PessoaFisica>()
                .ForMember(dest => dest.Cpf, opt => opt.MapFrom(src => src.CpfCnpj));
            
            CreateMap<UpdateVendedorDto, PessoaJuridica>()
                .ForMember(dest => dest.Cnpj, opt => opt.MapFrom(src => src.CpfCnpj))
                .ForMember(dest => dest.RazaoSocial, opt => opt.MapFrom(src => src.Nome));
            
            CreateMap<CreateVendaDto, VendaStoreDto>().ReverseMap();
        }
    }
}
