using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Projeto.Moope.API.DTOs;
using Projeto.Moope.Core.Interfaces.Services;

namespace Projeto.Moope.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;
        private readonly IValidator<UsuarioDto> _validator;
        private readonly IMapper _mapper;

        public UsuarioController(IUsuarioService usuarioService, IValidator<UsuarioDto> validator, IMapper mapper)
        {
            _usuarioService = usuarioService;
            _validator = validator;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> BuscarTodosAsync()
        {
            var usuarios = await _usuarioService.BuscarTodosAsync();
            return Ok(_mapper.Map<IEnumerable<UsuarioDto>>(usuarios));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> BuscarPorIdAsync(Guid id)
        {
            var usuario = await _usuarioService.BuscarPorIdAsync(id);
            if (usuario == null) return NotFound();
            return Ok(_mapper.Map<UsuarioDto>(usuario));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UsuarioDto usuarioDto)
        {
            var validation = await _validator.ValidateAsync(usuarioDto);
            if (!validation.IsValid)
                return BadRequest(validation.Errors);
            var usuario = _mapper.Map<Core.Models.Usuario>(usuarioDto);
            var created = await _usuarioService.SalvarAsync(usuario);
            return Ok();
            //return CreatedAtAction(nameof(GetById), new { id = created.Id }, _mapper.Map<UsuarioDto>(created));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UsuarioDto usuarioDto)
        {
            if (id != usuarioDto.Id) return BadRequest();
            var validation = await _validator.ValidateAsync(usuarioDto);
            if (!validation.IsValid)
                return BadRequest(validation.Errors);
            var usuario = _mapper.Map<Core.Models.Usuario>(usuarioDto);
            var updated = await _usuarioService.AtualizarAsync(usuario);
            return Ok(_mapper.Map<UsuarioDto>(updated));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _usuarioService.RemoverAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
} 