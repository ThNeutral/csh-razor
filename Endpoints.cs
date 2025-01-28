
namespace razor {
    class Endpoints {
        public readonly Game game;
        public readonly WSHandler wshandler;
        public static string MetadataPath = "/metadata";
        public Endpoints(Game g) {
            game = g;
            wshandler = new WSHandler(g);
        }
        public async Task GetMetadata(HttpContext context) {
            if (wshandler.conns.Count >= game.metadata.NumberOfPlayers) {
                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                await context.Response.WriteAsync("No place left for new connection");
                return;
            }

            var (token, index) = game.GenerateToken();

            var data = game.metadata.ToString();
            context.Response.StatusCode = StatusCodes.Status200OK;
            context.Response.Headers.Append("token", index.ToString() + "///" + token);
            await context.Response.WriteAsync(data);
        } 
        public async Task HandleNotFound(HttpContext context) {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            await context.Response.WriteAsync($"Endpoint was not found. Requested {context.Request.Method} at {context.Request.Path}");
        }
        public async Task HandleNotEnoughRoom(HttpContext context) {
            context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            await context.Response.WriteAsync("WS room is full");
        }
    }
}