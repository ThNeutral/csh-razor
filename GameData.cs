using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace razor {
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
                var transform = new Transform {
                   Position = new Vector2(width / 10, height * (i + 1) / 3),
                   Speed = new Vector2(300, 0), 
                   Rotation = 0
                };
                Players.Add(transform);
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
    }
}