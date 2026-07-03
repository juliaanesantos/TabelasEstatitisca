namespace Ftec.ProjetosWeb.Estatistica.Mvc.Models;

public class LoginViewModel
{
    public string Email { get; set; }
    public string Senha { get; set; }
    public string ReturnUrl { get; set; }
}

public class RegisterViewModel
{
    public string Nome { get; set; }
    public string Email { get; set; }
    public string Senha { get; set; }
    public string Documento { get; set; }
    public string Telefone { get; set; }
    public string DataNascimento { get; set; }
}

public class TokenInfo
{
    public string AccessToken { get; set; }
    public string UsuarioId { get; set; }
    public string Nome { get; set; }
    public string Email { get; set; }
}
