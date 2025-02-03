using System.Collections.Concurrent;
using System.Diagnostics;

namespace razor {
    struct TokenData {
        public string Token {get; set;}
        public int Index {get; set;}
        public static TokenData Empty => new() {Token = "", Index = -1};
        public static bool IsEmpty(TokenData data) => data.Index == -1 || data.Token == "";
    }
    class AuthService {
        public static AuthService Instance {get; private set;} = new();
        private int currentCapacity = 0;
        public int MaximumCapacity {get;} = Constants.NUMBER_OF_PLAYERS;
        private ConcurrentDictionary<string, int> tokens = new();
        private ConcurrentDictionary<int, string> indexes = new();
        public TokenData IssueToken() {
            if (currentCapacity >= MaximumCapacity) {
                return TokenData.Empty;
            }

            var token = Guid.NewGuid().ToString();
            var index = currentCapacity;
            
            tokens.TryAdd(token, index);
            indexes.TryAdd(index, token);

            currentCapacity += 1;
        
            return new TokenData {Token = token, Index = index};
        }
        public string? GetIndex(int index) {
            indexes.TryGetValue(index, out string? token);
            return token;
        } 
        public int GetIndex(string token) {
            var result = tokens.TryGetValue(token, out int index);
            if (!result) {
                return -1;
            }
            return index;
        } 
        public async Task Middleware(HttpContext context, RequestDelegate next) {
            if (IsAssetOrPage(context.Request.Path)) {
                await next(context);
                return;
            }

            var tokenHeaders = context.Request.Headers[Constants.TOKEN_HEADER_NAME];
            var token = tokenHeaders.Count == 0 ? null : tokenHeaders.First();
            if (token == null) {
                if (context.Request.Path == Constants.WS_HUB_PATH) {
                    await next(context);
                    return;
                }

                if (currentCapacity >= MaximumCapacity) {
                    context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                    await context.Response.WriteAsync("Hub is full.");
                    return;    
                }

                if (context.Request.Path != Constants.REGISTRATION_PATH) {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync($"Tokenless requests are allowed only on {Constants.REGISTRATION_PATH}");
                    return;    
                }

                await next(context);
                return;
            }

            var result = tokens.TryGetValue(token, out int index);
            if (!result) {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("Invalid token.");
                return;
            }

            var tokenData = new TokenData { Token = token, Index = index };
            SetTokenData(context, tokenData);
            await next(context);
        }
        public TokenData? GetTokenData(HttpContext context) {
            var result = context.Items.TryGetValue(Constants.TOKEN_ITEM_NAME, out object? value);
            if (!result) return null;
            return value as TokenData?;
        }
        public void SetTokenData(HttpContext context, TokenData? data) {
            context.Items[Constants.TOKEN_HEADER_NAME] = data;
        }
        private bool IsAssetOrPage(PathString path) {
            var str = path.ToString();
            return Constants.PAGE_PATHS.Contains(str) || str.Contains("css") || str.Contains("png") || str.Contains("js") || str.Contains("_blazor"); 
        }
    }
}