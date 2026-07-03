using Microsoft.AspNetCore.Mvc;
using Ftec.ProjetosWeb.Estatistica.Mvc.Models;
using Ftec.ProjetosWeb.Estatistica.Mvc.Services;

namespace Ftec.ProjetosWeb.Estatistica.Mvc.Controllers;

public class AuthController : Controller
{
    private readonly AuthService _auth;

    public AuthController(AuthService auth)
    {
        _auth = auth;
    }

    public IActionResult Login(string returnUrl = null)
    {
        ViewBag.ReturnUrl = returnUrl;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        try
        {
            var token = await _auth.LoginAsync(model.Email, model.Senha);

            HttpContext.Session.SetString("AccessToken", token.AccessToken);
            HttpContext.Session.SetString("UsuarioId", token.UsuarioId);
            HttpContext.Session.SetString("UsuarioNome", token.Nome);
            HttpContext.Session.SetString("UsuarioEmail", token.Email);

            if (!string.IsNullOrEmpty(model.ReturnUrl))
                return LocalRedirect(model.ReturnUrl);

            return RedirectToAction("Index", "Home");
        }
        catch
        {
            ModelState.AddModelError("", "E-mail ou senha invalidos.");
            return View(model);
        }
    }

    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        try
        {
            var success = await _auth.RegisterAsync(model);
            if (success)
            {
                TempData["Success"] = "Conta criada com sucesso! Faca login.";
                return RedirectToAction("Login");
            }
            ModelState.AddModelError("", "Erro ao criar conta. Verifique os dados.");
        }
        catch
        {
            ModelState.AddModelError("", "Erro ao criar conta. Verifique os dados.");
        }

        return View(model);
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Home");
    }
}
