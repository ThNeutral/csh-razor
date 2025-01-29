using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace razor {
    enum Actions {
        ROTATE_LEFT,
        ROTATE_RIGHT,
        ACCELERATE,
        DECELERATE,
    }
    class ActionData {
        public string Token {get; set;}
        public Dictionary<Actions, bool> Actions {get; set;}
    }
    class GameData {
        public int NumberOfPlayers {get; set;}
        public int FieldHeight {get; set;}
        public int FieldWidth {get; set;}
        public List<Transform> Players {get; set;}
        public GameData(int num, int height, int width) {
            NumberOfPlayers = num;
            FieldHeight = height;
            FieldWidth = width;
            Players = new List<Transform>(num);
            for (int i = 0; i < num; i++) {
                Players.Add(new Transform {
                   Position = new Vector2(20, height * (i + 1) / 3),
                   SpeedDirection = new Vector2(1, 0),
                   SpeedMagnitude = 0,
                   AccelerationRate = 200,
                   AccelerationAmount = 10,
                   MaximumSpeed = 320,
                   RotationSpeed = 120,
                   RotationAmount = 10
                });
            }
        }
        [JsonConstructor]
        public GameData(int NumberOfPlayers, int FieldHeight, int FieldWidth, List<Transform> Players) {
            this.NumberOfPlayers = NumberOfPlayers;
            this.FieldHeight = FieldHeight;
            this.FieldWidth = FieldWidth;
            this.Players = Players;
        }
        public override string ToString() {
            return JSON.Stringify(this);
        }
        public string StringifyPositions() {
            return JSON.Stringify(Players);
        }
        public void ApplyAction(Dictionary<Actions, bool> actions, int index) {
            lock (this) {
                var transform = Players[index];
                if (actions[Actions.ROTATE_LEFT]) {
                    transform.QueueRotation(-transform.RotationAmount);
                }
                if (actions[Actions.ROTATE_RIGHT]) {
                    transform.QueueRotation(transform.RotationAmount);
                }
                if (actions[Actions.ACCELERATE]) {
                    transform.QueueAcceleration(transform.AccelerationAmount);
                }
                if (actions[Actions.DECELERATE]) {
                    transform.QueueAcceleration(-transform.AccelerationAmount);
                }
            }
        }
    }
}