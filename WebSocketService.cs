using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace razor {
    struct WebSocketMessage {
        public string Type {get; set;}
        public string Data {get; set;}
    }
    delegate void WSMessageDelegate(WebSocketMessage message);
    class WebSocketService {
        public static WebSocketService Instance {get; private set;} = new();
        private ConcurrentDictionary<WebSocket, bool> connections = new();
        private Publisher<WSMessageDelegate, WebSocketMessage> publisher = new();
        private AuthService authService = AuthService.Instance;
        private GameService gameService = GameService.Instance;
        public void Subscribe(WSMessageDelegate callback) {
            publisher.Subscribe(callback);
        }
        public async Task Middleware(HttpContext context, RequestDelegate next) {
            if (context.Request.Path == Constants.WS_HUB_PATH) {
                if (context.WebSockets.IsWebSocketRequest) {
                    if (connections.Count >= authService.MaximumCapacity) {
                        context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                        await context.Response.WriteAsync("Hub is full.");
                        return; 
                    }

                    var websocket = await context.WebSockets.AcceptWebSocketAsync();
                    connections.TryAdd(websocket, true);
                    await HandleWebsocket(websocket);
                    connections.TryRemove(websocket, out var _);
                }
            } else {
                await next(context);
            }
        }
        private async Task HandleWebsocket(WebSocket websocket) {
            try {
                var buffer = new byte[4 * 1024];
                while (true) {
                    var result = await websocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    if (result.MessageType == WebSocketMessageType.Close) {
                        break;
                    }

                    var str = Encoding.ASCII.GetString(buffer, 0, result.Count);
                    var message = JSON.Parse<WebSocketMessage>(str);
                    publisher.Publish(message);
                }
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
            } finally {
                try {
                    await websocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Normal Closure", CancellationToken.None);
                } catch {}
            }
        }
        public async Task Loop() {
            while (true) {
                await Task.Delay((int)MathF.Floor(Constants.TARGET_LOOP_DELAY));
                var message = gameService.GetStringifiedGameState();
                await BroadcastMessage(message);
            }
        }
        private async Task BroadcastMessage(string message) {
            var bytes = new ArraySegment<byte>(Encoding.ASCII.GetBytes(message));
            var tasks = new List<Task>(connections.Keys.Count);
            foreach (var conn in connections.Keys) {
                var token = new CancellationTokenSource(100).Token;
                tasks.Add(conn.SendAsync(bytes, WebSocketMessageType.Text, true, token));
            }
            await Task.WhenAll(tasks);
        }
    }
}