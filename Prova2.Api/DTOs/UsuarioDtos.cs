
// ============================================================
// PROJETO: PROVA II - CBTSWE2
// INSTITUIÇÃO: IFSP - CAMPUS CUBATÃO
//
// INTEGRANTES:
// Gustavo Cerqueira Murai - CB3033261
// Igor Cerqueira Murai - CB3033295
//
// ============================================================



using System.ComponentModel.DataAnnotations;

namespace Prova2.Api.DTOs;

public class UsuarioCriarDto
{
    [Required(ErrorMessage = "O nome é obrigatório.")]
    [StringLength(100, MinimumLength = 2)]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "A senha é obrigatória.")]
    [MinLength(8, ErrorMessage = "A senha deve ter pelo menos 8 caracteres.")]
    public string Senha { get; set; } = string.Empty;

    public bool Status { get; set; } = true;
}

public class UsuarioAtualizarDto
{
    [Required(ErrorMessage = "O nome é obrigatório.")]
    [StringLength(100, MinimumLength = 2)]
    public string Nome { get; set; } = string.Empty;

    [MinLength(8, ErrorMessage = "A nova senha deve ter pelo menos 8 caracteres.")]
    public string? NovaSenha { get; set; }

    [Required(ErrorMessage = "O status é obrigatório.")]
    public bool? Status { get; set; }
}

public record UsuarioRespostaDto(
    int Id,
    string Nome,
    bool Status
);

public class UsuarioLoginDto
{
    [Required(ErrorMessage = "O nome é obrigatório.")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "A senha é obrigatória.")]
    public string Senha { get; set; } = string.Empty;
}
