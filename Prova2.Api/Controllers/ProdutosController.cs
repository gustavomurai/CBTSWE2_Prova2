
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
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Prova2.Api.Data;
using Prova2.Api.DTOs;
using Prova2.Api.Models;
using System.Security.Claims;

namespace Prova2.Api.Controllers;

[ApiController]
[Route("api/produtos")]
[Authorize]
public class ProdutosController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProdutosController(AppDbContext context)
    {
        _context = context;
    }

    private async Task<Usuario?> ObterUsuarioAutenticadoAsync()
    {
        var idUsuario = User.FindFirst(
            ClaimTypes.NameIdentifier)?.Value;

        if (!int.TryParse(idUsuario, out int usuarioId))
            return null;

        var usuario = await _context.Usuarios.FindAsync(usuarioId);

        if (usuario == null || !usuario.Status)
            return null;

        return usuario;
    }

    // LISTAR PRODUTOS
    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var usuario = await ObterUsuarioAutenticadoAsync();

        if (usuario == null)
            return Unauthorized();

        var produtos = await _context.Produtos
            .AsNoTracking()
            .OrderBy(p => p.Id)
            .ToListAsync();

        return Ok(produtos);
    }

    // BUSCAR PRODUTO
    [HttpGet("{id:int}")]
    public async Task<IActionResult> Buscar(int id)
    {
        var usuario = await ObterUsuarioAutenticadoAsync();

        if (usuario == null)
            return Unauthorized();

        var produto = await _context.Produtos
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);

        if (produto == null)
            return NotFound();

        return Ok(produto);
    }

    // CADASTRAR PRODUTO
    [HttpPost]
    public async Task<IActionResult> Cadastrar(
        [FromBody] ProdutoEntradaDto entrada)
    {
        var usuario = await ObterUsuarioAutenticadoAsync();

        if (usuario == null)
            return Unauthorized();

        var produto = new Produto
        {
            Nome = entrada.Nome.Trim(),
            Preco = entrada.Preco,
            Status = entrada.Status,

            IdUsuarioCadastro = usuario.Id,
            IdUsuarioUpdate = usuario.Id
        };

        _context.Produtos.Add(produto);

        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(Buscar),
            new { id = produto.Id },
            produto);
    }

    // ATUALIZAR PRODUTO
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Atualizar(
        int id,
        [FromBody] ProdutoEntradaDto entrada)
    {
        var usuario = await ObterUsuarioAutenticadoAsync();

        if (usuario == null)
            return Unauthorized();

        var produto = await _context.Produtos.FindAsync(id);

        if (produto == null)
            return NotFound();

        produto.Nome = entrada.Nome.Trim();
        produto.Preco = entrada.Preco;
        produto.Status = entrada.Status;

        // Mantém o usuário original do cadastro.
        // Atualiza somente quem realizou a alteração.
        produto.IdUsuarioUpdate = usuario.Id;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // EXCLUIR PRODUTO
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Excluir(int id)
    {
        var usuario = await ObterUsuarioAutenticadoAsync();

        if (usuario == null)
            return Unauthorized();

        var produto = await _context.Produtos.FindAsync(id);

        if (produto == null)
            return NotFound();

        _context.Produtos.Remove(produto);

        await _context.SaveChangesAsync();

        return NoContent();
    }
}
