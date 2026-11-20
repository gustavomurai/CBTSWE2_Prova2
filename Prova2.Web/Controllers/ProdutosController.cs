
// ============================================================
// PROJETO: PROVA II - CBTSWE2
// INSTITUIÇÃO: IFSP - CAMPUS CUBATÃO
//
// INTEGRANTES:
// Gustavo Cerqueira Murai - CB3033261
// Igor Cerqueira Murai - CB3033295
//
// ============================================================



using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Prova2.Web.Models;

namespace Prova2.Web.Controllers;

public class ProdutosController : Controller
{
    private readonly IHttpClientFactory _factory;

    public ProdutosController(IHttpClientFactory factory)
    {
        _factory = factory;
    }

    public override void OnActionExecuting(
        ActionExecutingContext context)
    {
        base.OnActionExecuting(context);

        if (string.IsNullOrWhiteSpace(
            HttpContext.Session.GetString("JwtToken")))
        {
            context.Result = new RedirectToActionResult(
                "Login",
                "Account",
                null);
        }
    }

    private HttpClient CriarCliente()
    {
        var cliente = _factory.CreateClient("Api");

        string? token =
            HttpContext.Session.GetString("JwtToken");

        if (string.IsNullOrWhiteSpace(token))
            throw new InvalidOperationException("Sessão expirada.");

        cliente.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        return cliente;
    }

    private IActionResult SessaoExpirada()
    {
        HttpContext.Session.Clear();

        return RedirectToAction("Login", "Account");
    }

    // LISTAR PRODUTOS
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        try
        {
            using var cliente = CriarCliente();

            using var resposta =
                await cliente.GetAsync("api/produtos");

            if (resposta.StatusCode == HttpStatusCode.Unauthorized)
                return SessaoExpirada();

            if (!resposta.IsSuccessStatusCode)
            {
                TempData["Erro"] =
                    await resposta.Content.ReadAsStringAsync();

                return View(new List<ProdutoViewModel>());
            }

            var produtos =
                await resposta.Content.ReadFromJsonAsync<
                    List<ProdutoViewModel>>();

            return View(produtos ?? new List<ProdutoViewModel>());
        }
        catch (HttpRequestException)
        {
            TempData["Erro"] = "Não foi possível conectar à API.";

            return View(new List<ProdutoViewModel>());
        }
        catch (Exception)
        {
            TempData["Erro"] = "Ocorreu um erro ao carregar os produtos.";

            return View(new List<ProdutoViewModel>());
        }
    }

    // ABRIR CADASTRO
    [HttpGet]
    public IActionResult Criar()
    {
        return View(new ProdutoViewModel
        {
            Status = true
        });
    }

    // CADASTRAR PRODUTO
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Criar(ProdutoViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            using var cliente = CriarCliente();

            var dados = new
            {
                nome = model.Nome,
                preco = model.Preco,
                status = model.Status
            };

            using var resposta = await cliente.PostAsJsonAsync(
                "api/produtos",
                dados);

            if (resposta.StatusCode == HttpStatusCode.Unauthorized)
                return SessaoExpirada();

            if (!resposta.IsSuccessStatusCode)
            {
                TempData["Erro"] =
                    await resposta.Content.ReadAsStringAsync();

                return View(model);
            }

            TempData["Sucesso"] = "Produto cadastrado com sucesso!";

            return RedirectToAction(nameof(Index));
        }
        catch (HttpRequestException)
        {
            TempData["Erro"] = "Não foi possível conectar à API.";
            return View(model);
        }
        catch (Exception)
        {
            TempData["Erro"] = "Erro ao cadastrar produto.";
            return View(model);
        }
    }

    // ABRIR EDIÇÃO
    [HttpGet]
    public async Task<IActionResult> Editar(int id)
    {
        try
        {
            using var cliente = CriarCliente();

            using var resposta =
                await cliente.GetAsync($"api/produtos/{id}");

            if (resposta.StatusCode == HttpStatusCode.Unauthorized)
                return SessaoExpirada();

            if (resposta.StatusCode == HttpStatusCode.NotFound)
                return NotFound();

            if (!resposta.IsSuccessStatusCode)
            {
                TempData["Erro"] =
                    await resposta.Content.ReadAsStringAsync();

                return RedirectToAction(nameof(Index));
            }

            var produto =
                await resposta.Content.ReadFromJsonAsync<
                    ProdutoViewModel>();

            if (produto == null)
                return NotFound();

            return View(produto);
        }
        catch (HttpRequestException)
        {
            TempData["Erro"] = "Não foi possível conectar à API.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception)
        {
            TempData["Erro"] = "Erro ao carregar produto.";
            return RedirectToAction(nameof(Index));
        }
    }

    // ATUALIZAR PRODUTO
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(
        int id,
        ProdutoViewModel model)
    {
        model.Id = id;

        if (!ModelState.IsValid)
            return View(model);

        try
        {
            using var cliente = CriarCliente();

            var dados = new
            {
                nome = model.Nome,
                preco = model.Preco,
                status = model.Status
            };

            using var resposta = await cliente.PutAsJsonAsync(
                $"api/produtos/{id}",
                dados);

            if (resposta.StatusCode == HttpStatusCode.Unauthorized)
                return SessaoExpirada();

            if (resposta.StatusCode == HttpStatusCode.NotFound)
                return NotFound();

            if (!resposta.IsSuccessStatusCode)
            {
                TempData["Erro"] =
                    await resposta.Content.ReadAsStringAsync();

                return View(model);
            }

            TempData["Sucesso"] = "Produto atualizado com sucesso!";

            return RedirectToAction(nameof(Index));
        }
        catch (HttpRequestException)
        {
            TempData["Erro"] = "Não foi possível conectar à API.";
            return View(model);
        }
        catch (Exception)
        {
            TempData["Erro"] = "Erro ao atualizar produto.";
            return View(model);
        }
    }

    // EXCLUIR PRODUTO
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Excluir(int id)
    {
        try
        {
            using var cliente = CriarCliente();

            using var resposta =
                await cliente.DeleteAsync($"api/produtos/{id}");

            if (resposta.StatusCode == HttpStatusCode.Unauthorized)
                return SessaoExpirada();

            if (resposta.StatusCode == HttpStatusCode.NotFound)
                return NotFound();

            if (!resposta.IsSuccessStatusCode)
            {
                TempData["Erro"] =
                    await resposta.Content.ReadAsStringAsync();

                return RedirectToAction(nameof(Index));
            }

            TempData["Sucesso"] = "Produto excluído com sucesso!";

            return RedirectToAction(nameof(Index));
        }
        catch (HttpRequestException)
        {
            TempData["Erro"] = "Não foi possível conectar à API.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception)
        {
            TempData["Erro"] = "Erro ao excluir produto.";
            return RedirectToAction(nameof(Index));
        }
    }
}
