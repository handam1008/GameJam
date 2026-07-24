using UnityEngine;

namespace Systems
{
    public class PlatformCarrier
    {
        private readonly Rigidbody2D parentRb;
        private float lastRotation;
        private Vector2 lastPosition;

        public PlatformCarrier(Rigidbody2D parentRb)
        {
            this.parentRb = parentRb;
            if (parentRb == null) return;
            lastRotation = parentRb.rotation;
            lastPosition = parentRb.position;
        }

        public Vector2 GetCarriedVelocity(Vector2 worldPosition)
        {
            if (parentRb == null) return Vector2.zero;

            float deltaRotation = parentRb.rotation - lastRotation;
            Vector2 deltaPosition = parentRb.position - lastPosition;
            lastRotation = parentRb.rotation;
            lastPosition = parentRb.position;

            Vector2 relativePosition = worldPosition - parentRb.position;
            float angularVelocityInRadians = (deltaRotation * Mathf.Deg2Rad) / Time.fixedDeltaTime;
            Vector2 rotationVelocity = new Vector2(-relativePosition.y, relativePosition.x) * angularVelocityInRadians;
            Vector2 platformLinearVelocity = deltaPosition / Time.fixedDeltaTime;

            return rotationVelocity + platformLinearVelocity;
        }
    }
}
