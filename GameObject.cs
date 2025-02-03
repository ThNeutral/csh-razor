using System.Numerics;
using System.Text.Json.Serialization;

namespace razor {
    class GameObject {
        public Transform Transform {get; protected set;}
        public bool IsStarted;
        [JsonConstructor]
        public GameObject(Transform Transform) {
            this.Transform = Transform;
        }
        public GameObject() {
            Transform = Transform.Zero;
            IsStarted = false;
        }
        public GameObject(Vector2 pos) {
            Transform = Transform.FromPosition(pos);
            IsStarted = false;
        }
        public virtual void Start() {} 
        public virtual void Update(float deltaTime) {}
    }
}