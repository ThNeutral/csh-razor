using System.Numerics;

namespace razor {
    class Transform {
        public Vector2 Position { get; set; }
        public Vector2 SpeedDirection { get; set; }
        public float SpeedMagnitude { get; set; }
        public float MaximumSpeed { get; set; }
        public float AccelerationRate { get; set; }
        public float AccelerationAmount { get; set; }
        public float AccelerationQueue;
        public float RotationSpeed { get; set; }
        public float RotationAmount { get; set; } 
        private float RotationQueue;

        public void UpdateSelf(float deltaTime) {
            if (RotationQueue != 0) {
                float rotationAmount = MathF.Sign(RotationQueue) * MathF.Min(MathF.Abs(RotationQueue), RotationSpeed * deltaTime);
                RotateDirection(rotationAmount);
                RotationQueue -= rotationAmount;
            }

            if (AccelerationQueue != 0) {
                float accelerationAmount = MathF.Sign(AccelerationQueue) * MathF.Min(MathF.Abs(AccelerationQueue), AccelerationRate * deltaTime);
                SpeedMagnitude += accelerationAmount;
                SpeedMagnitude = MathF.Max(-MaximumSpeed, MathF.Min(SpeedMagnitude, MaximumSpeed));
                AccelerationQueue -= accelerationAmount;
            }

            Position += SpeedDirection * SpeedMagnitude * deltaTime;
        }

        public void QueueRotation(float degrees) {
            RotationQueue = degrees;
        }

        public void QueueAcceleration(float value) {
            AccelerationQueue = value;
        }

        private void RotateDirection(float degrees) {
            float radians = MathF.PI / 180.0f * degrees;

            float cosTheta = MathF.Cos(radians);
            float sinTheta = MathF.Sin(radians);

            var rotatedDirection = new Vector2(
                SpeedDirection.X * cosTheta - SpeedDirection.Y * sinTheta,
                SpeedDirection.X * sinTheta + SpeedDirection.Y * cosTheta
            );

            SpeedDirection = Vector2.Normalize(rotatedDirection);
        }
    }
}