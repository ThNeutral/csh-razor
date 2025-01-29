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
        public void SetActions(ActionData action) {
            var res = tokenLookup.TryGetValue(action.Token, out int index);
            if (!res) return;

            metadata.ApplyAction(action.Actions, index);
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