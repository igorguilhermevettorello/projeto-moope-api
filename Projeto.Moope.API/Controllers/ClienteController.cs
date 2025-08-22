using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Projeto.Moope.API.Controllers.Base;
using Projeto.Moope.API.DTOs.Clientes;
using Projeto.Moope.Core.Enums;
using Projeto.Moope.Core.Interfaces.Identity;
using Projeto.Moope.Core.Interfaces.Notifications;
using Projeto.Moope.Core.Interfaces.Services;
using Projeto.Moope.Core.Interfaces.UnitOfWork;
using Projeto.Moope.Core.Models;
using Projeto.Moope.Infrastructure.Helpers;

namespace Projeto.Moope.API.Controllers
{
    [ApiController]
    [Route("api/cliente")]
    [Authorize]
    public class ClienteController : MainController
    {
        private readonly IClienteService _clienteService;
        private readonly IUsuarioService _usuarioService;
        private readonly IEnderecoService _enderecoService;
        private readonly IIdentityUserService _identityUserService;
        private readonly IPapelService _papelService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public ClienteController(
            IClienteService clienteService,
            IUsuarioService usuarioService,
            IEnderecoService enderecoService,
            IIdentityUserService identityUserService,
            IPapelService papelService,
            IMapper mapper,
            IUnitOfWork unitOfWork,
            INotificador notificador,
            IUser user) : base(notificador, user)
        {
            _clienteService = clienteService;
            _usuarioService = usuarioService;
            _enderecoService = enderecoService;
            _identityUserService = identityUserService;
            _papelService = papelService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [Authorize(Roles = nameof(TipoUsuario.Administrador))]
        [ProducesResponseType(typeof(List<ListClienteDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> BuscarTodosAsync()
        {
            var clientes = await _clienteService.BuscarTodosAsync();
            return Ok(_mapper.Map<IEnumerable<ListClienteDto>>(clientes));
        }

        [HttpGet("{id}")]
        [Authorize(Roles = nameof(TipoUsuario.Administrador))]
        [ProducesResponseType(typeof(ListClienteDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> BuscarPorIdAsync(Guid id)
        {
            var cliente = await _clienteService.BuscarPorIdAsync(id);
            if (cliente == null) 
                return NotFound("Cliente não encontrado");
            
            return Ok(_mapper.Map<ListClienteDto>(cliente));
        }

        [HttpPost]
        [Authorize(Roles = $"{nameof(TipoUsuario.Vendedor)},{nameof(TipoUsuario.Administrador)}")]
        [ProducesResponseType(typeof(CreateClienteDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> CriarAsync([FromBody] CreateClienteDto createClienteDto)
        {
            var clienteId = Guid.NewGuid();
            var usuarioExistente = false;
            if (createClienteDto == null)
            {
                NotificarErro("Mensagem", "As informações do cliente não foram carregadas. Tente novamente.");
                return CustomResponse();
            }
            
            if (!ModelState.IsValid)
                return CustomResponse(ModelState);

            var identityUser = new IdentityUser<Guid>();
            
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var cliente = _mapper.Map<Cliente>(createClienteDto);
                var endereco = _mapper.Map<Endereco>(createClienteDto.Endereco);
                var usuario =  _mapper.Map<Usuario>(createClienteDto);
                var pessoaFisica = _mapper.Map<PessoaFisica>(createClienteDto);
                var pessoaJuridica = _mapper.Map<PessoaJuridica>(createClienteDto);
                
                var rsIdentity = await _identityUserService.CriarUsuarioAsync(
                    createClienteDto.Email, 
                    createClienteDto.Senha, 
                    telefone: createClienteDto.Telefone, 
                    tipoUsuario: TipoUsuario.Cliente);

                usuarioExistente = rsIdentity.UsuarioExiste; 
                
                if (!rsIdentity.Status) 
                    throw new Exception(rsIdentity.Mensagem);

                identityUser = (IdentityUser<Guid>)rsIdentity.Dados;
                
                if (rsIdentity.UsuarioExiste)
                {
                    cliente.Id = identityUser.Id;
                    clienteId = identityUser.Id;
                    var rsPapel = await _papelService.SalvarAsync(new Papel()
                    {
                        UsuarioId = identityUser.Id,
                        TipoUsuario = TipoUsuario.Cliente,
                        Created = DateTime.UtcNow
                    });
                    
                    if (!rsPapel.Status) 
                        throw new Exception(rsPapel.Mensagem);
                    
                    var rsCliente = await _clienteService.SalvarAsync(cliente);
                    if (!rsCliente.Status) 
                        throw new Exception(rsCliente.Mensagem);
                }
                else
                {
                    var rsEndereco = await _enderecoService.SalvarAsync(endereco);
                    if (!rsEndereco.Status) 
                        throw new Exception(rsEndereco.Mensagem);
                
                    usuario.Id =  rsIdentity.Dados.Id;
                    usuario.EnderecoId = rsEndereco.Dados.Id;
                    usuario.TipoUsuario = TipoUsuario.Cliente;
                
                    var rsUsuario = await _usuarioService.SalvarAsync(usuario);
                    if (!rsUsuario.Status) 
                        throw new Exception(rsUsuario.Mensagem);
                
                    clienteId = rsUsuario.Dados.Id;
                    cliente.Id = rsUsuario.Dados.Id;
                    pessoaFisica.Id = rsUsuario.Dados.Id;
                    pessoaJuridica.Id = rsUsuario.Dados.Id;
                    
                    if (!await IsAdmin())
                    {
                        cliente.VendedorId = UsuarioId;
                    }
                    
                    var rsCliente = await _clienteService.SalvarAsync(cliente, pessoaFisica, pessoaJuridica);
                    if (!rsCliente.Status) 
                        throw new Exception(rsCliente.Mensagem);    
                }

                await _unitOfWork.CommitAsync();
                
                return Created(string.Empty, new { id = clienteId });
            }
            catch (Exception ex)
            {
                if (!usuarioExistente) 
                    _identityUserService.RemoverAoFalharAsync(identityUser);
                
                NotificarErro("Mensagem",  ex.Message);
                await _unitOfWork.RollbackAsync();
                return CustomResponse();
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = nameof(TipoUsuario.Administrador))]
        [ProducesResponseType(typeof(UpdateClienteDto), StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> AtualizarAsync(Guid id, [FromBody] UpdateClienteDto updateClienteDto)
        {
            if (id == Guid.Empty || updateClienteDto.Id == Guid.Empty)
            {
                ModelState.AddModelError("Id", "Campo Id está inválido.");
                return CustomResponse(ModelState);
            }

            if (id != updateClienteDto.Id)
            {
                ModelState.AddModelError("Id", "Campo Id do parâmetro não confere com o Id solicitado.");
                return CustomResponse(ModelState);
            }

            if (!ModelState.IsValid) 
                return CustomResponse(ModelState);

            var clienteExistente = await _clienteService.BuscarPorIdAsNotrackingAsync(id);
            if (clienteExistente == null)
            {
                ModelState.AddModelError("Mensagem", "Cliente não encontrado.");
                return CustomResponse(ModelState);
            }
            
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var cliente = _mapper.Map<Cliente>(updateClienteDto);
                var endereco = _mapper.Map<Endereco>(updateClienteDto.Endereco);
                var usuario =  _mapper.Map<Usuario>(updateClienteDto);
                var pessoaFisica = _mapper.Map<PessoaFisica>(updateClienteDto);
                var pessoaJuridica = _mapper.Map<PessoaJuridica>(updateClienteDto);

                var rsIdentity = await _identityUserService.AlterarUsuarioAsync(
                    clienteExistente.Id,
                    updateClienteDto.Email, 
                    telefone: updateClienteDto.Telefone);
                
                if (!rsIdentity.Status) 
                    throw new Exception(rsIdentity.Mensagem);

                var usuarioAuxiliar = await _usuarioService.BuscarPorIdAsNotrackingAsync(id);
                endereco.Id = (Guid) usuarioAuxiliar.EnderecoId;
                var rsEndereco = await _enderecoService.AtualizarAsync(endereco);
                if (!rsEndereco.Status) 
                    throw new Exception(rsEndereco.Mensagem);

                usuario.Id = clienteExistente.Id;
                usuario.TipoUsuario = TipoUsuario.Cliente;
                var rsUsuario = await _usuarioService.AtualizarAsync(usuario);
                if (!rsUsuario.Status) 
                    throw new Exception(rsUsuario.Mensagem);
                
                cliente.Id = clienteExistente.Id;
                var rsCliente = await _clienteService.AtualizarAsync(cliente, pessoaFisica, pessoaJuridica);
                if (!rsCliente.Status) 
                    throw new Exception(rsCliente.Mensagem);

                await _unitOfWork.CommitAsync();
                
                return NoContent();
            }
            catch (Exception ex)
            {
                NotificarErro("Mensagem", ex.Message);
                await _unitOfWork.RollbackAsync();
                return CustomResponse();
            }
        }


        [HttpPut("ativar/{id}")]
        public async Task<IActionResult> AtivarAsync(Guid id)
        {
            var cliente = await _clienteService.BuscarPorIdAsync(id);
            if (cliente == null) 
                return NotFound("Cliente não encontrado");

            // cliente.Ativo = true;
            cliente.Updated = DateTime.UtcNow;

            var result = await _clienteService.AtualizarAsync(cliente);
            
            if (!result.Status) 
                return CustomResponse();

            return NoContent();
        }

        [HttpPut("inativar/{id}")]
        public async Task<IActionResult> InativarAsync(Guid id)
        {
            var cliente = await _clienteService.BuscarPorIdAsync(id);
            if (cliente == null) 
                return NotFound("Cliente não encontrado");

            // cliente.Ativo = false;
            cliente.Updated = DateTime.UtcNow;

            var result = await _clienteService.AtualizarAsync(cliente);
            
            if (!result.Status) 
                return CustomResponse();

            return NoContent();
        }

        // [HttpDelete("{id}")]
        // public async Task<IActionResult> RemoverAsync(Guid id)
        // {
        //     var cliente = await _clienteService.BuscarPorIdAsync(id);
        //     if (cliente == null) 
        //         return NotFound("Cliente não encontrado");
        //
        //     var sucesso = await _clienteService.RemoverAsync(id);
        //     
        //     if (!sucesso) 
        //         return CustomResponse();
        //
        //     return NoContent();
        // }

        [HttpGet("tipo-pessoa")]
        public async Task<IActionResult> BuscarTipoPessoasAsync()
        {
            var lista = EnumHelper.GetEnumAsList<TipoPessoa>();
            return Ok(lista);
        }

    }
}