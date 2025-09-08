using UnityEngine;

namespace CollisionSystem.Core
{
    public readonly struct CollisionContext
    {
        public readonly GameObject A, B;
        public readonly bool Is2D, IsTrigger;
        public readonly Vector3 RelativeVelocity;
        public readonly float RelativeSpeed;
        public readonly Vector3 ContactPoint, ContactNormal;
        public readonly Collision Collision3D;
        public readonly Collider ColliderA, ColliderB;
#if ENABLE_2D_PHYSICS 
public readonly Collision2D Collision2D;
        public readonly Collider2D ColliderA2D, ColliderB2D;
#endif
        public CollisionContext(GameObject a, GameObject b, Vector3 relVel, Vector3 pt, Vector3 n, Collision col)
        {
            A = a;
            B = b;
            Is2D = false;
            IsTrigger = false;
            RelativeVelocity = relVel;
            RelativeSpeed = relVel.magnitude;
            ContactPoint = pt;
            ContactNormal = n;
            Collision3D = col;
            ColliderA = null;
            ColliderB = null; 
#if ENABLE_2D_PHYSICS
            Collision2D = default;
            ColliderA2D = null;
            ColliderB2D = null; 
#endif
        }

        public CollisionContext(GameObject a, GameObject b, Collider aCol, Collider bCol, Vector3 relVel, Vector3 pt, Vector3 n)
        {
            A = a;
            B = b;
            Is2D = false;
            IsTrigger = true;
            RelativeVelocity = relVel;
            RelativeSpeed = relVel.magnitude;
            ContactPoint = pt;
            ContactNormal = n;
            Collision3D = null;
            ColliderA = aCol;
            ColliderB = bCol;
#if ENABLE_2D_PHYSICS
            Collision2D = default;
            ColliderA2D = null;
            ColliderB2D = null;
#endif
        }
#if ENABLE_2D_PHYSICS
        public CollisionContext(GameObject a, GameObject b, Vector2 relVel2, Vector2 pt2,
            Vector2 n2, Collision2D col2d)
        {
            A = a;
            B = b;
            Is2D = true;
            IsTrigger = false;
            var rel = new Vector3(relVel2.x, relVel2.y, 0f);
            RelativeVelocity = rel;
            RelativeSpeed = rel.magnitude;
            ContactPoint = new Vector3(pt2.x, pt2.y, 0f);
            ContactNormal = new Vector3(n2.x, n2.y, 0f);
            Collision3D = null;
            ColliderA = null;
            ColliderB = null;
            Collision2D = col2d;
            ColliderA2D = col2d ? col2d.collider : a ? a.GetComponent<Collider2D>() : null;
            ColliderB2D = col2d ? col2d.otherCollider : b ? b.GetComponent<Collider2D>() : null;
        }

        public CollisionContext(GameObject a, GameObject b, Collider2D aCol, Collider2D bCol, Vector2 relVel2,
            Vector2 pt2, Vector2 n2)
        {
            A = a;
            B = b;
            Is2D = true;
            IsTrigger = true;
            var rel = new Vector3(relVel2.x, relVel2.y, 0f);
            RelativeVelocity = rel;
            RelativeSpeed = rel.magnitude;
            ContactPoint = new Vector3(pt2.x, pt2.y, 0f);
            ContactNormal = new Vector3(n2.x, n2.y, 0f);
            Collision3D = null;
            ColliderA = null;
            ColliderB = null;
            Collision2D = default;
            ColliderA2D = aCol;
            ColliderB2D = bCol;
        }
#endif
    }
}

// namespace CollisionSystem.Core
// {
//     public readonly struct CollisionContext
//     {
//         public readonly GameObject A;
//         public readonly GameObject B;
// #if UNITY_2D_PHYSICS
//         public readonly bool Is2D;
// #endif
//
//         public readonly Vector3 RelativeVelocity;
//         public readonly float RelativeSpeed;
//         public readonly Vector3 ContactPoint;
//         public readonly Vector3 ContactNormal;
//
//         public readonly Collision Collision3D;
// #if UNITY_2D_PHYSICS
//         public readonly Collision2D Collision2D;
// #endif
//
//
//         public CollisionContext(GameObject a, GameObject b, Vector3 relVel, Vector3 contactPoint, Vector3 contactNormal,
//             Collision collision3D)
//         {
//             A = a;
//             B = b;
// #if UNITY_2D_PHYSICS
//             Is2D = false;
// #endif
//             RelativeVelocity = relVel;
//             RelativeSpeed = relVel.magnitude;
//             ContactPoint = contactPoint;
//             ContactNormal = contactNormal;
//             Collision3D = collision3D;
// #if UNITY_2D_PHYSICS
//             Collision2D = default;
// #endif
//         }
//
// #if UNITY_2D_PHYSICS
//         public CollisionContext(GameObject a, GameObject b, Vector2 relVel2D, Vector2 contactPoint2D,
//             Vector2 contactNormal2D, Collision2D collision2D)
//         {
//             A = a;
//             B = b;
//             Is2D = true;
//             var rel = new Vector3(relVel2D.x, relVel2D.y, 0f);
//             RelativeVelocity = rel;
//             RelativeSpeed = rel.magnitude;
//             ContactPoint = new Vector3(contactPoint2D.x, contactPoint2D.y, 0f);
//             ContactNormal = new Vector3(contactNormal2D.x, contactNormal2D.y, 0f);
//             Collision3D = default;
//             Collision2D = collision2D;
//         }
// #endif
//     }
// }