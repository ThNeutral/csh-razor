using System.Numerics;

namespace razor {
    class Transform {
        public Vector2 Position {get; set;}
        public static Transform Zero => new Transform {
            Position = Vector2.Zero,
        };
        public static Transform FromPosition(Vector2 pos) => new Transform {
            Position = pos
        };
    }
}