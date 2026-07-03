using Ftec.ProjetosWeb.Estatistica.Aplicacao;
using Ftec.ProjetosWeb.Estatistica.Dominio.Interfaces;
using Ftec.ProjetosWeb.Estatistica.Persistencia;
using Ftec.ProjetosWeb.Estatistica.Persistencia.ApiClientes;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var strConexao = builder.Configuration["strConexao"] ?? "";
var urlsApi = builder.Configuration.GetSection("UrlsApi");

builder.Services.AddHttpClient<PedidoApiClient>(client =>
{
    client.BaseAddress = new Uri(urlsApi["Pedido"] ?? "http://pedido.neurosky.com.br");
    client.Timeout = TimeSpan.FromSeconds(10);
});

builder.Services.AddHttpClient<ProdutoApiClient>(client =>
{
    client.BaseAddress = new Uri(urlsApi["Produto"] ?? "http://produto.neurosky.com.br");
    client.Timeout = TimeSpan.FromSeconds(10);
});

builder.Services.AddHttpClient("Usuario", client =>
{
    client.BaseAddress = new Uri(urlsApi["Usuario"] ?? "http://usuario.neurosky.com.br");
    client.Timeout = TimeSpan.FromSeconds(10);
});
builder.Services.AddTransient<UsuarioApiClient>(sp =>
{
    var factory = sp.GetRequiredService<IHttpClientFactory>();
    var client = factory.CreateClient("Usuario");
    return new UsuarioApiClient(client, urlsApi["UsuarioEmail"] ?? "", urlsApi["UsuarioSenha"] ?? "");
});

builder.Services.AddHttpClient<AvaliacaoApiClient>(client =>
{
    client.BaseAddress = new Uri(urlsApi["Avaliacao"] ?? "http://avaliacao.neurosky.com.br");
    client.Timeout = TimeSpan.FromSeconds(10);
});

builder.Services.AddTransient<IEstatisticaRepositorio>(sp =>
{
    var pedidoClient = sp.GetRequiredService<PedidoApiClient>();
    var produtoClient = sp.GetRequiredService<ProdutoApiClient>();
    var usuarioClient = sp.GetRequiredService<UsuarioApiClient>();
    var avaliacaoClient = sp.GetRequiredService<AvaliacaoApiClient>();
    return new EstatisticaRepositorio(strConexao, pedidoClient, produtoClient, usuarioClient, avaliacaoClient);
});

builder.Services.AddTransient<IMediaAvaliacaoProdutoRepositorio>(sp =>
{
    var avaliacaoClient = sp.GetRequiredService<AvaliacaoApiClient>();
    var produtoClient = sp.GetRequiredService<ProdutoApiClient>();
    return new MediaAvaliacaoProdutoRepositorio(strConexao, avaliacaoClient, produtoClient);
});

builder.Services.AddTransient<EstatisticaAplicacao>();
builder.Services.AddTransient<MediaAvaliacaoProdutoAplicacao>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

app.Run();
