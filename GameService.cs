namespace razor {
    class GameService {
        public static GameService Instance {get; private set;} = new();
        private AuthService authService = AuthService.Instance;
        private GameMetadata gameMetadata = new(Constants.GAME_WIDTH, Constants.GAME_HEIGHT, AuthService.Instance.MaximumCapacity);
        private GameState gameState = new(AuthService.Instance.MaximumCapacity);
        public string GetStringifiedGameMetadata() {
            return JSON.Stringify(gameMetadata);
        }
        public string GetStringifiedGameState() {
            lock (gameState) {
                return JSON.Stringify(gameState);
            };
        }
        public async Task Loop() {
            while (true) {
                await Task.Delay((int)MathF.Floor(Constants.TARGET_LOOP_DELAY));
                var deltaTime = Constants.TARGET_LOOP_DELAY / 1000;
                lock(gameState.objects) {
                    foreach (var obj in gameState.objects) {
                        if (!obj.IsStarted) {
                            obj.Start();
                            obj.IsStarted = true;
                        }
                        obj.Update(deltaTime);
                    }
                }
            }
        }
    }
}