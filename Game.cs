using System.Collections.Concurrent;
using System.Numerics;
namespace razor {
    class Game {
        public ConcurrentDictionary<string, int> tokenLookup;
        public GameData metadata;
        public Game(int num, int height, int width) {
            metadata = new GameData(num, height, width);
            tokenLookup = new();
        }
        public async Task Run() {
            while (true) {
                await Task.Delay(Constants.gameThreadWaitTime);
                lock (metadata) {
                    for (int i = 0; i < metadata.Players.Count; i++) {
                        metadata.Players[i].UpdateSelf(0.016f);
                    }
                }
            }
        }
        public void SetSpeed(string token, Vector2 newSpeed) {
            tokenLookup.TryGetValue(token, out int index);
            metadata.Players[index].Speed = newSpeed;
        }
        public (string, int) GenerateToken() {
            if (tokenLookup.Count >= metadata.NumberOfPlayers) return ("", -1);
            var token = Guid.NewGuid().ToString();
            var index = tokenLookup.Count;
            tokenLookup.TryAdd(token, tokenLookup.Count);
            return (token, index);
        }
    }
}