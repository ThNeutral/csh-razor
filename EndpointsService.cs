using System.Diagnostics;
using System.Text;

namespace razor {
    class EndpointsService {
        public static EndpointsService Instance {get; private set;} = new();
        private AuthService authService = AuthService.Instance;
        private GameService gameService = GameService.Instance;
        public async Task GetRegistrationHandler(HttpContext context) {
            var token = authService.IssueToken();
            var json = JSON.Stringify(token);
            context.Response.StatusCode = StatusCodes.Status201Created;
            await context.Response.WriteAsync(json);
        }
        public async Task GetGameMetadataHandler(HttpContext context) {
            TokenData? tokenData = authService.GetTokenData(context);
            if (tokenData == null) throw new ArgumentException($"context.Items[{Constants.TOKEN_ITEM_NAME}] is null in authorized endpoint {nameof(GetGameMetadataHandler)}");
            var message = gameService.GetStringifiedGameMetadata();
            context.Response.StatusCode = StatusCodes.Status200OK;
            await context.Response.WriteAsync(message);
        }
        public async Task GetGameStateHandler(HttpContext context) {
            TokenData? tokenData = authService.GetTokenData(context);
            if (tokenData == null) throw new ArgumentException($"context.Items[{Constants.TOKEN_ITEM_NAME}] is null in authorized endpoint {nameof(GetGameStateHandler)}");
            var message = gameService.GetStringifiedGameState();
            context.Response.StatusCode = StatusCodes.Status200OK;
            await context.Response.WriteAsync(message);
        }
    }
}