using System.Text;
using System.Text.Json;
using Ftec.ProjetosWeb.Estatistica.Mvc.Models;

namespace Ftec.ProjetosWeb.Estatistica.Mvc.Services;

public class AuthService
{
    private readonly HttpClient _httpClient;

    public AuthService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<TokenInfo> LoginAsync(string email, string senha)
    {
        var body = new { email, senha };
        var json = JsonSerializer.Serialize(body);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("/api/autenticacao/login", content);
        response.EnsureSuccessStatusCode();

        var responseJson = await response.Content.ReadAsStringAsync();
        var doc = JsonDocument.Parse(responseJson);

        return new TokenInfo
        {
            AccessToken = doc.RootElement.GetProperty("accessToken").GetString(),
            UsuarioId = doc.RootElement.GetProperty("usuarioId").GetString(),
            Nome = doc.RootElement.GetProperty("nome").GetString(),
            Email = doc.RootElement.GetProperty("email").GetString()
        };
    }

    public async Task<bool> RegisterAsync(RegisterViewModel model)
    {
        var body = new
        {
            nome = model.Nome,
            email = model.Email,
            senha = model.Senha,
            documento = model.Documento,
            tipoPessoa = 0,
            funcao = 0,
            dataNascimento = model.DataNascimento,
            telefone = model.Telefone
        };

        var json = JsonSerializer.Serialize(body);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("/api/usuario", content);
        return response.IsSuccessStatusCode;
    }
}
