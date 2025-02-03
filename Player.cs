using System.Numerics;

namespace razor {
    class Player : GameObject {
        private WebSocketService webSocketService = WebSocketService.Instance;
        public Vector2 SpeedDirection {get; set;} = new Vector2(0, 1);
        public float SpeedMagnitude {get; set;} = 60f;
        public int Index {get;}
        public Player() : base() {}
        public Player(Vector2 pos) : base(pos) {}
        public override void Start() {
            // webSocketService.Subscribe(WebSocketCallback);
        }
        public override void Update(float deltaTime) {
            Transform.Position += deltaTime * SpeedMagnitude * SpeedDirection; 
        }
        private void WebSocketCallback(WebSocketMessage message) {

        }
    }
}