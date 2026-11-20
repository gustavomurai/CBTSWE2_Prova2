

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

namespace Prova2.Web.Models;

public class ProdutoViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Informe o nome do produto.")]
    [StringLength(
        100,
        MinimumLength = 2,
        ErrorMessage = "O nome deve ter entre 2 e 100 caracteres."
    )]
    public string Nome { get; set; } = string.Empty;

    [Range(
        0,
        float.MaxValue,
        ErrorMessage = "Informe um preço válido."
    )]
    public float Preco { get; set; }

    public bool Status { get; set; } = true;

    public int IdUsuarioCadastro { get; set; }

    public int IdUsuarioUpdate { get; set; }
}
