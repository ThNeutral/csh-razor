using System.Numerics;

namespace razor {
    class Transform {
        public Vector2 Position {get; set;}
        public Vector2 Speed {get; set;}
        public float Rotation {get; set;}
        public void UpdateSelf(float deltaTime) {
            Position += Speed * deltaTime;
        }
    }
}