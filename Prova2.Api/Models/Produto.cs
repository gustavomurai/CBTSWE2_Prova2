
// ============================================================
// PROJETO: PROVA II - CBTSWE2
// INSTITUIÇÃO: IFSP - CAMPUS CUBATÃO
//
// INTEGRANTES:
// Gustavo Cerqueira Murai - CB3033261
// Igor Cerqueira Murai - CB3033295
//
// ============================================================


namespace Prova2.Api.Models;

public class Produto
{
    public int Id { get; set; }

    public string Nome { get; set; } = string.Empty;

    public float Preco { get; set; }

    public bool Status { get; set; } = true;

    public int IdUsuarioCadastro { get; set; }

    public int IdUsuarioUpdate { get; set; }
}