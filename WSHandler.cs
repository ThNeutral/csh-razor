using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net.WebSockets;
using System.Numerics;
using System.Text;

namespace razor {
    class NewSpeedData {
        public Vector2 Speed {get; set;} 
        public string Token {get; set;}
    }
    class WSHandler {
        public static string Path = "/ws";
        public readonly Game game;
        public readonly ConcurrentDictionary<WebSocket, bool> conns = new();
        public WSHandler(Game g) {
            game = g;
        }
        public async Task Handler(HttpContext context) {
            if (conns.Count >= game.metadata.NumberOfPlayers) {
                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                await context.Response.WriteAsync("No place left for new connection");
                return;
            }
        
            if (context.WebSockets.IsWebSocketRequest) {
                var ws = await context.WebSockets.AcceptWebSocketAsync();
                conns.TryAdd(ws, true);
                await HandleWebsocket(ws);
                conns.TryRemove(ws, out bool _);
            } else {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }
        private async Task HandleWebsocket(WebSocket ws) {
            var buffer = new byte[1024 * 4];
            try {
                while (true) {
                    var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    var message = Encoding.ASCII.GetString(buffer, 0, result.Count);
                    var newData = JSON.Parse<NewSpeedData>(message);
                    game.SetSpeed(newData.Token, newData.Speed);
                }
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
            } finally {
                await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Normal closure", CancellationToken.None);
            }
        }
        public async Task Run() {
            while (true) {
                Thread.Sleep(Constants.webSocketThreadWaitTime);
                
                string message;
                lock (game.metadata) {
                    message = game.metadata.StringifyPositions();
                }
                var bytes = Encoding.ASCII.GetBytes(message);

                var tasks = new List<Task>();
                foreach (var conn in conns.Keys) {
                    var token = new CancellationTokenSource(100).Token;
                    tasks.Add(conn.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, token));
                }
                await Task.WhenAll(tasks);
            }
        }
    }
}