using System.Net.WebSockets;
using razor;
using razor.Components;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.WebHost.UseUrls([Constants.BASE_HTTP_URL]);

builder.Services.AddScoped<HttpClient>();
builder.Services.AddScoped<ClientWebSocket>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStatusCodePagesWithRedirects(Constants.NOT_FOUND_PAGE_PATH);

app.UseAntiforgery();

var options = new WebSocketOptions() {
    KeepAliveInterval = TimeSpan.FromSeconds(5),
};
app.UseWebSockets(options);

app.Use(AuthService.Instance.Middleware);
app.Use(WebSocketService.Instance.Middleware);

app.MapStaticAssets();

app.MapGet(Constants.REGISTRATION_PATH, EndpointsService.Instance.GetRegistrationHandler);
app.MapGet(Constants.GAME_METADATA_PATH, EndpointsService.Instance.GetGameMetadataHandler);
app.MapGet(Constants.GAME_STATE_PATH, EndpointsService.Instance.GetGameStateHandler);

app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

await Task.WhenAll([app.RunAsync(), WebSocketService.Instance.Loop(), GameService.Instance.Loop()]);