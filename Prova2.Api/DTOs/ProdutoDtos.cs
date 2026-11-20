
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

public class ProdutoEntradaDto
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Nome { get; set; } = string.Empty;

    [Range(0, float.MaxValue)]
    public float Preco { get; set; }

    public bool Status { get; set; } = true;
}