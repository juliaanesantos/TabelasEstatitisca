using Ftec.ProjetosWeb.Estatistica.Mvc.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(2);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddHttpClient<EstatisticaService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration.GetValue<string>("UrlsApi:Estatistica") ?? "http://localhost:5168");
});

builder.Services.AddHttpClient<AuthService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration.GetValue<string>("UrlsApi:Usuario") ?? "http://usuario.neurosky.com.br");
});

builder.Services.AddHttpClient<ProdutoService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration.GetValue<string>("UrlsApi:Produto") ?? "http://produto.neurosky.com.br");
});

builder.Services.AddHttpClient<PedidoService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration.GetValue<string>("UrlsApi:Pedido") ?? "http://pedido.neurosky.com.br");
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
