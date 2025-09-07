using UnityEngine;

namespace CollisionSystem.Core
{
    public readonly struct CollisionContext
    {
        public readonly GameObject A;
        public readonly GameObject B;
        public readonly bool Is2D;

        public readonly Vector3 RelativeVelocity;
        public readonly float RelativeSpeed;
        public readonly Vector3 ContactPoint;
        public readonly Vector3 ContactNormal;

        public readonly Collision Collision3D;
        public readonly Collision2D Collision2D;

        public CollisionContext(GameObject a, GameObject b, Vector3 relVel, Vector3 contactPoint,
            Vector3 contactNormal, Collision collision3D)
        {
            A = a;
            B = b;
            Is2D = false;
            RelativeVelocity = relVel;
            RelativeSpeed = relVel.magnitude;
            ContactPoint = contactPoint;
            ContactNormal = contactNormal;
            Collision3D = collision3D;
            Collision2D = default;
        }

        public CollisionContext(GameObject a, GameObject b, Vector2 relVel2D, Vector2 contactPoint2D,
            Vector2 contactNormal2D, Collision2D collision2D)
        {
            A = a;
            B = b;
            Is2D = true;
            var rel = new Vector3(relVel2D.x, relVel2D.y, 0f);
            RelativeVelocity = rel;
            RelativeSpeed = rel.magnitude;
            ContactPoint = new Vector3(contactPoint2D.x, contactPoint2D.y, 0f);
            ContactNormal = new Vector3(contactNormal2D.x, contactNormal2D.y, 0f);
            Collision3D = default;
            Collision2D = collision2D;
        }
    }
}