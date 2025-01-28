using System.Net.WebSockets;
using System.Numerics;
using razor;
using razor.Components;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.WebHost.UseUrls(["http://localhost:5168"]);

builder.Services.AddScoped<HttpClient>();
builder.Services.AddScoped<ClientWebSocket>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStatusCodePagesWithRedirects("/not-found");

app.UseAntiforgery();

var options = new WebSocketOptions() {
    KeepAliveInterval = TimeSpan.FromSeconds(5),
};
app.UseWebSockets(options);

var game = new Game(2, 600, 800);

var endpoints = new Endpoints(game);

app.MapGet(WSHandler.Path, endpoints.wshandler.Handler);
app.MapGet(Endpoints.MetadataPath, endpoints.GetMetadata);
app.MapFallback(endpoints.HandleNotFound);

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

Task.Run(game.Run);
Task.Run(endpoints.wshandler.Run);

app.Run();