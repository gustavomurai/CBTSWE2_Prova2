
// ============================================================
// PROJETO: PROVA II - CBTSWE2
// INSTITUIÇÃO: IFSP - CAMPUS CUBATÃO
//
// INTEGRANTES:
// Gustavo Cerqueira Murai - CB3033261
// Igor Cerqueira Murai - CB3033295
//
// ============================================================



using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Prova2.Web.Models;

namespace Prova2.Web.Controllers;

public class AccountController : Controller
{
    private readonly IHttpClientFactory _factory;
    private readonly IConfiguration _configuration;

    public AccountController(
        IHttpClientFactory factory,
        IConfiguration configuration)
    {
        _factory = factory;
        _configuration = configuration;
    }

    [HttpGet]
    public IActionResult Login()
    {
        if (!string.IsNullOrEmpty(
            HttpContext.Session.GetString("JwtToken")))
        {
            return RedirectToAction("Index", "Produtos");
        }

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var api = _factory.CreateClient("Api");

            string endpoint =
                _configuration["Api:LoginEndpoint"]!;

            using var resposta = await api.PostAsJsonAsync(
                endpoint,
                new
                {
                    nome = model.Nome,
                    senha = model.Senha
                });

            string conteudo =
                await resposta.Content.ReadAsStringAsync();

            if (!resposta.IsSuccessStatusCode)
            {
                ModelState.AddModelError(
                    "",
                    $"Falha no login: HTTP {(int)resposta.StatusCode}. {conteudo}");

                return View(model);
            }

            using var documento = JsonDocument.Parse(conteudo);

            string? token = EncontrarTexto(
                documento.RootElement,
                "token",
                "accessToken",
                "access_token",
                "jwt");

            if (token == null &&
                documento.RootElement.ValueKind == JsonValueKind.String)
            {
                token = documento.RootElement.GetString();
            }

            if (string.IsNullOrWhiteSpace(token))
            {
                ModelState.AddModelError(
                    "",
                    "A API não retornou o token JWT esperado.");

                return View(model);
            }

            token = token.Replace("Bearer ", "").Trim();

            int? usuarioId = EncontrarNumero(
                documento.RootElement,
                "usuarioId",
                "userId",
                "id");

            if (usuarioId == null)
                usuarioId = ObterIdDoToken(token);

            if (usuarioId == null || usuarioId <= 0)
            {
                ModelState.AddModelError(
                    "",
                    "O login funcionou, mas não foi possível identificar o ID do usuário no retorno ou no token JWT.");

                return View(model);
            }

            HttpContext.Session.SetString("JwtToken", token);

            HttpContext.Session.SetInt32(
                "UsuarioId",
                usuarioId.Value);

            HttpContext.Session.SetString(
                "UsuarioNome",
                model.Nome);

            return RedirectToAction("Index", "Produtos");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(
                "",
                "Erro ao conectar com a API: " + ex.Message);

            return View(model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();

        return RedirectToAction("Login");
    }

    private static string? EncontrarTexto(
        JsonElement elemento,
        params string[] nomes)
    {
        if (elemento.ValueKind == JsonValueKind.Object)
        {
            foreach (var propriedade in elemento.EnumerateObject())
            {
                if (nomes.Contains(
                    propriedade.Name,
                    StringComparer.OrdinalIgnoreCase)
                    && propriedade.Value.ValueKind == JsonValueKind.String)
                {
                    return propriedade.Value.GetString();
                }

                var resultado = EncontrarTexto(
                    propriedade.Value,
                    nomes);

                if (resultado != null)
                    return resultado;
            }
        }

        if (elemento.ValueKind == JsonValueKind.Array)
        {
            foreach (var item in elemento.EnumerateArray())
            {
                var resultado = EncontrarTexto(item, nomes);

                if (resultado != null)
                    return resultado;
            }
        }

        return null;
    }

    private static int? EncontrarNumero(
        JsonElement elemento,
        params string[] nomes)
    {
        if (elemento.ValueKind == JsonValueKind.Object)
        {
            foreach (var propriedade in elemento.EnumerateObject())
            {
                if (nomes.Contains(
                    propriedade.Name,
                    StringComparer.OrdinalIgnoreCase))
                {
                    if (propriedade.Value.ValueKind == JsonValueKind.Number
                        && propriedade.Value.TryGetInt32(out int numero))
                    {
                        return numero;
                    }

                    if (propriedade.Value.ValueKind == JsonValueKind.String
                        && int.TryParse(
                            propriedade.Value.GetString(),
                            out numero))
                    {
                        return numero;
                    }
                }

                var resultado = EncontrarNumero(
                    propriedade.Value,
                    nomes);

                if (resultado != null)
                    return resultado;
            }
        }

        if (elemento.ValueKind == JsonValueKind.Array)
        {
            foreach (var item in elemento.EnumerateArray())
            {
                var resultado = EncontrarNumero(item, nomes);

                if (resultado != null)
                    return resultado;
            }
        }

        return null;
    }

    private static int? ObterIdDoToken(string token)
    {
        try
        {
            string[] partes = token.Split('.');

            if (partes.Length != 3)
                return null;

            string payload = partes[1]
                .Replace('-', '+')
                .Replace('_', '/');

            payload = payload.PadRight(
                payload.Length + (4 - payload.Length % 4) % 4,
                '=');

            byte[] dados = Convert.FromBase64String(payload);

            using var documento = JsonDocument.Parse(dados);

            return EncontrarNumero(
                documento.RootElement,
                "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier",
                "nameid",
                "sub",
                "usuarioId",
                "userId",
                "id");
        }
        catch
        {
            return null;
        }
    }
}
