using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Ftec.ProjetosWeb.Estatistica.Persistencia.ApiClientes
{
    public class UsuarioApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _email;
        private readonly string _senha;
        private string _accessToken;
        private DateTime _tokenExpiresAt = DateTime.MinValue;

        public UsuarioApiClient(HttpClient httpClient, string email, string senha)
        {
            _httpClient = httpClient;
            _email = email;
            _senha = senha;
        }

        private async Task<string> ObterAccessTokenAsync()
        {
            if (!string.IsNullOrEmpty(_accessToken) && DateTime.UtcNow < _tokenExpiresAt)
                return _accessToken;

            var loginBody = new LoginRequest { Email = _email, Senha = _senha };
            var json = JsonSerializer.Serialize(loginBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/api/autenticacao/login", content);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<LoginResponse>(responseJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            _accessToken = result.AccessToken;
            _tokenExpiresAt = result.AccessTokenExpiresIn.AddMinutes(-1);

            return _accessToken;
        }

        public async Task<string> ObterNomeUsuarioAsync(Guid usuarioId)
        {
            try
            {
                var token = await ObterAccessTokenAsync();

                var request = new HttpRequestMessage(HttpMethod.Get, $"/api/usuario/{usuarioId}");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.SendAsync(request);
                if ((int)response.StatusCode == 401)
                {
                    _accessToken = null;
                    token = await ObterAccessTokenAsync();
                    request = new HttpRequestMessage(HttpMethod.Get, $"/api/usuario/{usuarioId}");
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    response = await _httpClient.SendAsync(request);
                }

                if (!response.IsSuccessStatusCode)
                    return "Desconhecido";

                var json = await response.Content.ReadAsStringAsync();
                var usuario = JsonSerializer.Deserialize<UsuarioResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return usuario?.Nome ?? "Desconhecido";
            }
            catch
            {
                return "Desconhecido";
            }
        }
    }
}
