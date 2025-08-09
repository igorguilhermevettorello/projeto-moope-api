using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Projeto.Moope.API.Controllers.Base;
using Projeto.Moope.API.DTOs;
using Projeto.Moope.API.DTOs.Clientes;
using Projeto.Moope.API.DTOs.Planos;
using Projeto.Moope.Core.DTOs.Clientes;
using Projeto.Moope.Core.Enums;
using Projeto.Moope.Core.Interfaces.Identity;
using Projeto.Moope.Core.Interfaces.Notifications;
using Projeto.Moope.Core.Interfaces.Services;
using Projeto.Moope.Core.Models;
using Projeto.Moope.Infrastructure.Helpers;

namespace Projeto.Moope.API.Controllers
{
    [ApiController]
    [Route("api/cliente")]
    public class ClienteController : MainController
    {
        private readonly IClienteService _clienteService;
        private readonly IMapper _mapper;

        public ClienteController(
            IClienteService clienteService,
            IMapper mapper,
            INotificador notificador,
            IUser user) : base(notificador, user)
        {
            _clienteService = clienteService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> BuscarTodosAsync()
        {
            var clientes = await _clienteService.BuscarTodosAsync();
            return Ok(_mapper.Map<IEnumerable<ClienteDto>>(clientes));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> BuscarPorIdAsync(Guid id)
        {
            var cliente = await _clienteService.BuscarPorIdAsync(id);
            if (cliente == null) 
                return NotFound("Cliente não encontrado");
            
            return Ok(_mapper.Map<ClienteDto>(cliente));
        }

        [HttpPost]
        [ProducesResponseType(typeof(CreateClienteDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> CriarAsync([FromBody] CreateClienteDto createClienteDto)
        {
            if (!ModelState.IsValid)
                return CustomResponse(ModelState);

            var cliente = _mapper.Map<Cliente>(createClienteDto);
            var endereco = _mapper.Map<Endereco>(createClienteDto.Endereco);
            var usuario = new Usuario
            {
                Nome = createClienteDto.Nome,
                Email = createClienteDto.Email,
                Telefone = createClienteDto.Celular,
                Tipo = TipoUsuario.Cliente,
                Ativo = createClienteDto.Ativo
            };

            cliente.Created = DateTime.UtcNow;
            cliente.Updated = DateTime.UtcNow;

            var auxiliar = new ClienteStoreDto
            {
                CpfCnpj = createClienteDto.CpfCnpj,
                Senha = createClienteDto.Senha
            };

            var result = await _clienteService.SalvarAsync(cliente, endereco, usuario, auxiliar);
            
            if (!result.Status) 
                return CustomResponse();

            return Created(string.Empty, new { id = result.Dados.Id });
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(UpdateClienteDto), StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> AtualizarAsync(Guid id, [FromBody] UpdateClienteDto updateClienteDto)
        {
            if (id != updateClienteDto.Id) 
                return BadRequest("ID do parâmetro não confere com o ID do objeto");

            if (!ModelState.IsValid) 
                return CustomResponse(ModelState);

            var clienteExistente = await _clienteService.BuscarPorIdAsync(id);
            if (clienteExistente == null) 
                return NotFound("Cliente não encontrado");

            var cliente = _mapper.Map<Cliente>(updateClienteDto);
            cliente.Created = clienteExistente.Created;
            cliente.Updated = DateTime.UtcNow;

            var result = await _clienteService.AtualizarAsync(cliente);
            
            if (!result.Status) 
                return CustomResponse();

            return NoContent();
        }


        [HttpPut("ativar/{id}")]
        public async Task<IActionResult> AtivarAsync(Guid id)
        {
            var cliente = await _clienteService.BuscarPorIdAsync(id);
            if (cliente == null) 
                return NotFound("Cliente não encontrado");

            cliente.Ativo = true;
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

            cliente.Ativo = false;
            cliente.Updated = DateTime.UtcNow;

            var result = await _clienteService.AtualizarAsync(cliente);
            
            if (!result.Status) 
                return CustomResponse();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoverAsync(Guid id)
        {
            var cliente = await _clienteService.BuscarPorIdAsync(id);
            if (cliente == null) 
                return NotFound("Cliente não encontrado");

            var sucesso = await _clienteService.RemoverAsync(id);
            
            if (!sucesso) 
                return CustomResponse();

            return NoContent();
        }

        [HttpGet("tipo-pessoa")]
        public async Task<IActionResult> BuscarTipoPessoasAsync()
        {
            var lista = EnumHelper.GetEnumAsList<TipoPessoa>();
            return Ok(lista);
        }

    }
}