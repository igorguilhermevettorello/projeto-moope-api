using AutoMapper;

namespace Projeto.Moope.API.Configurations
{
    public class AutomapperConfig : Profile
    {
        public AutomapperConfig()
        {
            //CreateMap<Fornecedor, FornecedorDto>().ReverseMap();
            //CreateMap<Endereco, EnderecoDto>().ReverseMap();
            //CreateMap<Produto, ProdutoDto>().ReverseMap();
            //CreateMap<Produto, ProdutoImagemDto>().ReverseMap();
            //CreateMap<Categoria, CategoriaDto>().ReverseMap();

            //CreateMap<ProdutoImagemViewModel, Produto>().ReverseMap();

            //CreateMap<Produto, ProdutoViewModel>()
            //    .ForMember(dest => dest.NomeFornecedor, opt => opt.MapFrom(src => src.Fornecedor.Nome));
        }
    }
}
