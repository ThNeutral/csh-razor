using System.Numerics;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace razor {
    class GameState {
        public List<GameObject> objects {get; private set;}
        [JsonConstructor]
        public GameState(List<GameObject> objects) {
            this.objects = objects;
        }
        public GameState(int numberOfPlayers) {
            objects = new(numberOfPlayers);
            for (int i = 0; i < numberOfPlayers; i++) {
                var player = new Player(new Vector2(50 * (i + 1), 20));
                objects.Add(player);
            }
        }
    }
}