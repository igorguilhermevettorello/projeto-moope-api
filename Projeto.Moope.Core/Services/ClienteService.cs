using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Projeto.Moope.Core.DTOs.Clientes;
using Projeto.Moope.Core.Enums;
using Projeto.Moope.Core.Interfaces.Notifications;
using Projeto.Moope.Core.Interfaces.Repositories;
using Projeto.Moope.Core.Interfaces.Services;
using Projeto.Moope.Core.Interfaces.UnitOfWork;
using Projeto.Moope.Core.Models;
using Projeto.Moope.Core.Models.Validators.Base;
using Projeto.Moope.Core.Models.Validators.Cliente;
using Projeto.Moope.Core.Models.Validators.Endereco;
using Projeto.Moope.Core.Models.Validators.Usuario;
using Projeto.Moope.Core.Services.Base;

namespace Projeto.Moope.Core.Services
{
    public class ClienteService : BaseService, IClienteService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IClienteRepository _clienteRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IEnderecoRepository _enderecoRepository;
        private readonly IPessoaFisicaRepository _pessoaFisicaRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ClienteService(
            UserManager<IdentityUser> userManager,
            IClienteRepository clienteRepository,
            IUsuarioRepository usuarioRepository,
            IEnderecoRepository enderecoRepository,
            IPessoaFisicaRepository pessoaFisicaRepository,
            IUnitOfWork unitOfWork,
            INotificador notificador) : base(notificador)
        {
            _userManager = userManager;
            _clienteRepository = clienteRepository;
            _usuarioRepository = usuarioRepository;
            _enderecoRepository = enderecoRepository;
            _pessoaFisicaRepository = pessoaFisicaRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Cliente> BuscarPorIdAsync(Guid id)
        {
            return await _clienteRepository.BuscarPorIdAsync(id);
        }

        public async Task<IEnumerable<Cliente>> BuscarTodosAsync()
        {
            return await _clienteRepository.BuscarTodosAsync();
        }

        public async Task<Result<Cliente>> SalvarAsync(Cliente cliente, Endereco endereco, Usuario usuario, ClienteStoreDto auxiliar)
        {
            if (!ExecutarValidacaoCliente(new ClienteValidator(), cliente)
                || !ExecutarValidacao(new EnderecoValidator(), endereco)
                || !ExecutarValidacao(new UsuarioValidator(), usuario))
            {
                return new Result<Cliente>()
                {
                    Status = false,
                    Mensagem = "Dados do cliente são inválidos"
                };
            }

            await _unitOfWork.BeginTransactionAsync();
            try
            { 
                var user = new IdentityUser
                {
                    UserName = usuario.Email,
                    Email = usuario.Email,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(user, auxiliar.Senha);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, TipoUsuario.Cliente.ToString());
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        if (error.Code.Equals("PasswordRequiresUpper"))
                        {
                            Notificar("Senha", error.Description);
                        }
                        else if (error.Code.Equals("PasswordRequiresLower"))
                        {
                            Notificar("Senha", error.Description);
                        }
                        else if (error.Code.Equals("PasswordRequiresDigit"))
                        {
                            Notificar("Senha", error.Description);
                        }
                        else if (error.Code.Equals("PasswordRequiresNonAlphanumeric"))
                        {
                            Notificar("Senha", error.Description);
                        }
                        else
                        {
                            Notificar("Senha", error.Description);
                        }
                    }

                    throw new Exception("Falha ao cadastrar cliente");
                }  

                var userId = await _userManager.GetUserIdAsync(user);
                var entityEndereco = await _enderecoRepository.SalvarAsync(endereco);
                usuario.IdentityUserId = userId;
                usuario.EnderecoId = entityEndereco.Id;
                var entityUsuario = await _usuarioRepository.SalvarAsync(usuario);
                cliente.Usuario = entityUsuario;                
                var entityCliente = await _clienteRepository.SalvarAsync(cliente);

                if (cliente.TipoPessoa == TipoPessoa.FISICA)
                {
                    var pessoaFisica = new PessoaFisica
                    {
                        Cliente = cliente,
                        Nome = usuario.Nome,
                        Cpf = auxiliar.CpfCnpj,
                        Created = DateTime.UtcNow
                    };
                    await _pessoaFisicaRepository.SalvarAsync(pessoaFisica);
                }


                await _unitOfWork.CommitAsync();
                return new Result<Cliente>()
                {
                    Status = true,
                    Dados = entityCliente,
                    Mensagem = "Cliente salvo com sucesso"
                };
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                Notificar("Cliente", $"Erro ao salvar cliente: {ex.Message}");
                return new Result<Cliente>()
                {
                    Status = false,
                    Mensagem = "Erro interno ao salvar cliente"
                };
            }
        }

        public async Task<Result<Cliente>> AtualizarAsync(Cliente cliente, Endereco endereco, Usuario usuario, ClienteStoreDto auxiliar)
        {
            if (!ExecutarValidacaoCliente(new ClienteValidator(), cliente)
                || !ExecutarValidacao(new EnderecoValidator(), endereco)
                || !ExecutarValidacao(new UsuarioValidator(), usuario))
            {
                return new Result<Cliente>
                {
                    Status = false,
                    Mensagem = "Dados do cliente são inválidos"
                };
            }

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var enderecoExistente = await _enderecoRepository.BuscarPorIdAsync(endereco.Id);
                if (enderecoExistente == null)
                {
                    return new Result<Cliente> { Status = false, Mensagem = "Endereço não encontrado" };
                }

                await _enderecoRepository.AtualizarAsync(enderecoExistente);

                var usuarioExistente = await _usuarioRepository.BuscarPorIdAsync(usuario.Id);
                if (usuarioExistente == null)
                {
                    return new Result<Cliente> { Status = false, Mensagem = "Usuário não encontrado" };
                }

                await _usuarioRepository.AtualizarAsync(usuarioExistente);

                var clienteExistente = await _clienteRepository.BuscarPorIdAsync(cliente.Id);
                if (clienteExistente == null)
                {
                    return new Result<Cliente> { Status = false, Mensagem = "Cliente não encontrado" };
                }

                clienteExistente.Usuario = usuarioExistente;
                var clienteAtualizado = await _clienteRepository.AtualizarAsync(clienteExistente);

                // Atualiza pessoa física, se aplicável
                if (cliente.TipoPessoa == TipoPessoa.FISICA)
                {
                    var pessoaFisica = await _pessoaFisicaRepository.BuscarPorIdAsync(cliente.Id);
                    if (pessoaFisica != null)
                    {
                        pessoaFisica.Nome = usuario.Nome;
                        pessoaFisica.Cpf = auxiliar.CpfCnpj;
                        await _pessoaFisicaRepository.AtualizarAsync(pessoaFisica);
                    }
                }

                await _unitOfWork.CommitAsync();

                return new Result<Cliente>
                {
                    Status = true,
                    Dados = clienteAtualizado,
                    Mensagem = "Cliente atualizado com sucesso"
                };
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                Notificar("Cliente", $"Erro ao atualizar cliente: {ex.Message}");
                return new Result<Cliente>
                {
                    Status = false,
                    Mensagem = "Erro interno ao atualizar cliente"
                };
            }
        }
        public async Task<Result<Cliente>> AtualizarAsync(Cliente cliente)
        {
            if (!ExecutarValidacaoCliente(new ClienteValidator(), cliente))
            {
                return new Result<Cliente>()
                {
                    Status = false,
                    Mensagem = "Dados do cliente são inválidos"
                };
            }

            try
            {
                var clienteExistente = await _clienteRepository.BuscarPorIdAsync(cliente.Id);
                if (clienteExistente == null)
                {
                    Notificar("Cliente", "Cliente não encontrado");
                    return new Result<Cliente>()
                    {
                        Status = false,
                        Mensagem = "Cliente não encontrado"
                    };
                }

                var entity = await _clienteRepository.AtualizarAsync(cliente);
                return new Result<Cliente>()
                {
                    Status = true,
                    Dados = entity,
                    Mensagem = "Cliente atualizado com sucesso"
                };
            }
            catch (Exception ex)
            {
                Notificar("Cliente", $"Erro ao atualizar cliente: {ex.Message}");
                return new Result<Cliente>()
                {
                    Status = false,
                    Mensagem = "Erro interno ao atualizar cliente"
                };
            }
        }

        public async Task<bool> RemoverAsync(Guid id)
        {
            try
            {
                var clienteExistente = await _clienteRepository.BuscarPorIdAsync(id);
                if (clienteExistente == null)
                {
                    Notificar("Cliente", "Cliente não encontrado");
                    return false;
                }

                await _clienteRepository.RemoverAsync(id);
                return true;
            }
            catch (Exception ex)
            {
                Notificar("Cliente", $"Erro ao remover cliente: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Executa validação específica para Cliente (adaptação do ExecutarValidacao da BaseService)
        /// </summary>
        private bool ExecutarValidacaoCliente<TV>(TV validacao, Cliente entidade) where TV : AbstractValidator<Cliente>
        {
            var validator = validacao.Validate(entidade);

            if (validator.IsValid) return true;

            Notificar(validator);
            return false;
        }
    }
}