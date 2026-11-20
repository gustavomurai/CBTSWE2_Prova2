
// ============================================================
// PROJETO: PROVA II - CBTSWE2
// INSTITUIÇÃO: IFSP - CAMPUS CUBATÃO
//
// INTEGRANTES:
// Gustavo Cerqueira Murai - CB3033261
// Igor Cerqueira Murai - CB3033295
//
// ============================================================


using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Prova2.Api.Data;
using Prova2.Api.DTOs;
using Prova2.Api.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Prova2.Api.Controllers;

[ApiController]
[Route("api/usuarios")]
[Authorize]
public class UsuariosController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IPasswordHasher<Usuario> _passwordHasher;
    private readonly IConfiguration _configuration;

    public UsuariosController(
        AppDbContext context,
        IPasswordHasher<Usuario> passwordHasher,
        IConfiguration configuration)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _configuration = configuration;
    }

    // LOGIN E GERAÇÃO DO TOKEN JWT
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] UsuarioLoginDto entrada)
    {
        string nome = entrada.Nome.Trim();

        var usuario = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Nome == nome);

        if (usuario == null || !usuario.Status)
        {
            return Unauthorized(new
            {
                mensagem = "Usuário ou senha inválidos."
            });
        }

        var resultado = _passwordHasher.VerifyHashedPassword(
            usuario,
            usuario.SenhaHash,
            entrada.Senha);

        if (resultado == PasswordVerificationResult.Failed)
        {
            return Unauthorized(new
            {
                mensagem = "Usuário ou senha inválidos."
            });
        }

        var claims = new[]
        {
            new Claim(
                ClaimTypes.NameIdentifier,
                usuario.Id.ToString()),

            new Claim(
                ClaimTypes.Name,
                usuario.Nome)
        };

        var chave = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(
                _configuration["Jwt:Key"]!));

        var credenciais = new SigningCredentials(
            chave,
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: credenciais);

        string tokenGerado =
            new JwtSecurityTokenHandler().WriteToken(token);

        return Ok(new
        {
            token = tokenGerado,
            usuario = new UsuarioRespostaDto(
                usuario.Id,
                usuario.Nome,
                usuario.Status)
        });
    }

    // VERIFICA SE O USUÁRIO DO TOKEN AINDA EXISTE E ESTÁ ATIVO
    private async Task<bool> UsuarioAutenticadoEstaAtivoAsync()
    {
        string? idUsuario = User.FindFirstValue(
            ClaimTypes.NameIdentifier);

        if (!int.TryParse(idUsuario, out int usuarioId))
            return false;

        return await _context.Usuarios.AnyAsync(
            u => u.Id == usuarioId && u.Status);
    }

    // LISTAR TODOS OS USUÁRIOS
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UsuarioRespostaDto>>> Listar()
    {
        if (!await UsuarioAutenticadoEstaAtivoAsync())
        {
            return Unauthorized(new
            {
                mensagem = "Usuário não autenticado ou inativo."
            });
        }

        var usuarios = await _context.Usuarios
            .AsNoTracking()
            .OrderBy(u => u.Id)
            .Select(u => new UsuarioRespostaDto(
                u.Id,
                u.Nome,
                u.Status))
            .ToListAsync();

        return Ok(usuarios);
    }

    // BUSCAR USUÁRIO PELO ID
    [HttpGet("{id:int}")]
    public async Task<ActionResult<UsuarioRespostaDto>> BuscarPorId(int id)
    {
        if (!await UsuarioAutenticadoEstaAtivoAsync())
        {
            return Unauthorized(new
            {
                mensagem = "Usuário não autenticado ou inativo."
            });
        }

        var usuario = await _context.Usuarios
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id);

        if (usuario == null)
        {
            return NotFound(new
            {
                mensagem = "Usuário não encontrado."
            });
        }

        return Ok(new UsuarioRespostaDto(
            usuario.Id,
            usuario.Nome,
            usuario.Status));
    }

    // CADASTRAR USUÁRIO
    // O primeiro usuário pode ser criado sem login, somente quando o banco estiver vazio.
    // Depois disso, somente um usuário autenticado e ativo pode cadastrar outros usuários.
    [AllowAnonymous]
    [HttpPost]
    public async Task<ActionResult<UsuarioRespostaDto>> Cadastrar(
        [FromBody] UsuarioCriarDto entrada)
    {
        bool jaExisteUsuario = await _context.Usuarios.AnyAsync();

        if (jaExisteUsuario)
        {
            if (User.Identity?.IsAuthenticated != true ||
                !await UsuarioAutenticadoEstaAtivoAsync())
            {
                return Unauthorized(new
                {
                    mensagem = "Entre com um usuário ativo para cadastrar outros usuários."
                });
            }
        }

        string nome = entrada.Nome.Trim();

        if (nome.Length < 2)
        {
            return BadRequest(new
            {
                mensagem = "O nome deve ter pelo menos 2 caracteres."
            });
        }

        bool nomeExiste = await _context.Usuarios
            .AnyAsync(u => u.Nome == nome);

        if (nomeExiste)
        {
            return Conflict(new
            {
                mensagem = "Já existe um usuário com esse nome."
            });
        }

        var usuario = new Usuario
        {
            Nome = nome,
            Status = entrada.Status
        };

        usuario.SenhaHash = _passwordHasher.HashPassword(
            usuario,
            entrada.Senha);

        _context.Usuarios.Add(usuario);

        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(BuscarPorId),
            new { id = usuario.Id },
            new UsuarioRespostaDto(
                usuario.Id,
                usuario.Nome,
                usuario.Status));
    }

    // ATUALIZAR USUÁRIO
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Atualizar(
        int id,
        [FromBody] UsuarioAtualizarDto entrada)
    {
        if (!await UsuarioAutenticadoEstaAtivoAsync())
        {
            return Unauthorized(new
            {
                mensagem = "Usuário não autenticado ou inativo."
            });
        }

        var usuario = await _context.Usuarios.FindAsync(id);

        if (usuario == null)
        {
            return NotFound(new
            {
                mensagem = "Usuário não encontrado."
            });
        }

        string nome = entrada.Nome.Trim();

        if (nome.Length < 2)
        {
            return BadRequest(new
            {
                mensagem = "O nome deve ter pelo menos 2 caracteres."
            });
        }

        bool nomeExiste = await _context.Usuarios
            .AnyAsync(u => u.Id != id && u.Nome == nome);

        if (nomeExiste)
        {
            return Conflict(new
            {
                mensagem = "Já existe um usuário com esse nome."
            });
        }

        usuario.Nome = nome;
        usuario.Status = entrada.Status!.Value;

        if (!string.IsNullOrWhiteSpace(entrada.NovaSenha))
        {
            usuario.SenhaHash = _passwordHasher.HashPassword(
                usuario,
                entrada.NovaSenha);
        }

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // EXCLUIR USUÁRIO
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Excluir(int id)
    {
        if (!await UsuarioAutenticadoEstaAtivoAsync())
        {
            return Unauthorized(new
            {
                mensagem = "Usuário não autenticado ou inativo."
            });
        }

        var usuario = await _context.Usuarios.FindAsync(id);

        if (usuario == null)
        {
            return NotFound(new
            {
                mensagem = "Usuário não encontrado."
            });
        }

        bool possuiProdutos = await _context.Produtos.AnyAsync(
            p => p.IdUsuarioCadastro == id ||
                 p.IdUsuarioUpdate == id);

        if (possuiProdutos)
        {
            return Conflict(new
            {
                mensagem = "Usuário vinculado a produtos não pode ser excluído."
            });
        }

        _context.Usuarios.Remove(usuario);

        await _context.SaveChangesAsync();

        return NoContent();
    }
}
