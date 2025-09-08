using System.Collections.Generic;
using UnityEngine;

// namespace CollisionSystem.Core
// {
//     [DefaultExecutionOrder(-5000)]
//     public sealed class CollisionManager : MonoBehaviour
//     {
//         public static CollisionManager Default { get; private set; }
//
//         private readonly HashSet<CollisionPair> pairsThisStep = new();
//
//         private void Awake()
//         {
//             if (Default != null && Default != this)
//             {
//                 Destroy(gameObject);
//                 return;
//             }
//
//             Default = this;
//         }
//
//         private void FixedUpdate()
//         {
//             pairsThisStep.Clear();
//         }
//
//         private void OnDestroy()
//         {
//             if (Default != null && Default == this)
//                 Default = null;
//         }
//
//         public void Register(GameObject reporter, Collision collision)
//         {
//             var otherGo = collision.gameObject;
//             var ctx = Build3DContext(reporter, otherGo, collision);
//             TryAccept(ctx);
//         }
//
//         public void Register(GameObject reporter, Collider collider)
//         {
//             var otherGo = collider.gameObject;
//             var ctx = Build3DContext(reporter, otherGo, collider);
//         }
// #if UNITY_2D_PHYSICS
//         public void Register(GameObject reporter, Collision2D collision)
//         {
//             var otherGo = collision.gameObject;
//             var ctx = Build2DContext(reporter, otherGo, collision);
//             TryAccept(ctx);
//         }
// #endif
//
//         private static CollisionContext Build3DContext(GameObject a, GameObject b, Collision collision)
//         {
//             var point = collision.contactCount > 0 ? collision.GetContact(0).point : a.transform.position;
//             var normal = collision.contactCount > 0
//                 ? collision.GetContact(0).normal
//                 : -collision.relativeVelocity.normalized;
//             var ctx = new CollisionContext(a, b, collision.relativeVelocity, point, normal, collision);
//             return ctx;
//         }
//
//         private static CollisionContext Build3DContext(GameObject a, GameObject b, Collider collider)
//         {
//         }
// #if UNITY_2D_PHYSICS
//         private static CollisionContext Build2DContext(GameObject a, GameObject b, Collision2D col2D)
//         {
//             var point = col2D.contactCount > 0 ? col2D.GetContact(0).point : (Vector2)a.transform.position;
//             var normal = col2D.contactCount > 0 ? col2D.GetContact(0).normal : -col2D.relativeVelocity.normalized;
//             var ctx = new CollisionContext(a, b, col2D.relativeVelocity, point, normal, col2D);
//             return ctx;
//         }
// #endif
//
//         private void TryAccept(in CollisionContext ctx)
//         {
//             var pair = new CollisionPair(ctx.A, ctx.B);
//             if (pairsThisStep.Contains(pair)) return;
//             if (!ctx.A || !ctx.B) return;
//             if (!ctx.A.TryGetComponent<CollisionParticipant>(out var pa)) return;
//             if (!ctx.B.TryGetComponent<CollisionParticipant>(out var pb)) return;
//             if (!pa.CheckAll(ctx)) return;
//             var ctxForB =
// #if UNITY_2D_PHYSICS
//       ctx.Is2D
//                 ? new CollisionContext(ctx.B, ctx.A, new Vector2(-ctx.RelativeVelocity.x, -ctx.RelativeVelocity.y),
//                     new Vector2(ctx.ContactPoint.x, ctx.ContactPoint.y),
//                     new Vector2(-ctx.ContactNormal.x, -ctx.ContactNormal.y), ctx.Collision2D)
//                 :
// #endif
//                 new CollisionContext(ctx.B, ctx.A, -ctx.RelativeVelocity, ctx.ContactPoint, -ctx.ContactNormal,
//                     ctx.Collision3D);
//             if (!pb.CheckAll(ctxForB)) return;
//             pairsThisStep.Add(pair);
//             NotifyAccepted(ctx);
//             NotifyAccepted(ctxForB);
//         }
//
//         private static void NotifyAccepted(in CollisionContext ctx)
//         {
//             if (ctx.A.TryGetComponent<ICollisionListener>(out var listener))
//                 listener.OnCollisionAccepted(ctx);
//         }
//     }
// }


namespace CollisionSystem.Core
{
    [DefaultExecutionOrder(-5000)]
    public sealed class CollisionManager : MonoBehaviour
    {
        public static CollisionManager Default { get; private set; }
        private readonly HashSet<CollisionPair> pairs = new();

        private void Awake()
        {
            if (Default != null && Default != this)
            {
                Destroy(gameObject);
                return;
            }

            Default = this;
        }

        private void FixedUpdate()
        {
            pairs.Clear();
        }

        public void Register(GameObject reporter, Collision col)
        {
            var ctx = Build3DContext(reporter, col.gameObject, col);
            TryAccept(ctx);
        }

        private static CollisionContext Build3DContext(GameObject a, GameObject b, Collision col)
        {
            var pt = col.contactCount > 0 ? col.GetContact(0).point : a.transform.position;
            var n = col.contactCount > 0 ? col.GetContact(0).normal : -col.relativeVelocity.normalized;
            return new CollisionContext(a, b, col.relativeVelocity, pt, n, col);
        }

        public void Register(GameObject reporter, Collider self, Collider other)
        {
            var ctx = Build3DTriggerContext(reporter, self, other);
            TryAccept(ctx);
        }

        private static CollisionContext Build3DTriggerContext(GameObject a, Collider self, Collider other)
        {
            var aGo = a;
            var bGo = other ? other.gameObject : null;
            var ra = aGo ? aGo.GetComponent<Rigidbody>() : null;
            var rb = bGo ? bGo.GetComponent<Rigidbody>() : null;
            var rel = (ra ? ra.linearVelocity : Vector3.zero) - (rb ? rb.linearVelocity : Vector3.zero);
            Vector3 pa = self
                ? self.ClosestPoint(bGo ? bGo.transform.position : aGo.transform.position)
                : aGo.transform.position;
            Vector3 pb = other
                ? other.ClosestPoint(aGo ? aGo.transform.position : (bGo ? bGo.transform.position : Vector3.zero))
                : (bGo ? bGo.transform.position : pa);
            var mid = (pa + pb) * 0.5f;
            var dir = pa - pb;
            var n = dir.sqrMagnitude > 1e-6f
                ? dir.normalized
                : (aGo && bGo ? (aGo.transform.position - bGo.transform.position).normalized : Vector3.up);
            return new CollisionContext(aGo, bGo, self, other, rel, mid, n);
        }
        
#if ENABLE_2D_PHYSICS
        public void Register(GameObject reporter, Collision2D col)
        {
            var ctx = Build2DContext(reporter, col.gameObject, col);
            TryAccept(ctx);
        }

        private static CollisionContext Build2DContext(GameObject a, GameObject b, Collision2D col)
        {
            var pt = col.contactCount > 0 ? col.GetContact(0).point : (Vector2)a.transform.position;
            var n = col.contactCount > 0 ? col.GetContact(0).normal : -col.relativeVelocity.normalized;
            return new CollisionContext(a, b, col.relativeVelocity, pt, n, col);
        }

        public void RegisterTrigger(GameObject reporter, Collider2D self, Collider2D other)
        {
            var ctx = Build2DTriggerContext(reporter, self, other);
            TryAccept(ctx);
        }

        private static CollisionContext Build2DTriggerContext(GameObject a, Collider2D self, Collider2D other)
        {
            var aGo = a;
            var bGo = other ? other.gameObject : null;
            var ra = aGo ? aGo.GetComponent<Rigidbody2D>() : null;
            var rb = bGo ? bGo.GetComponent<Rigidbody2D>() : null;
            var rel = (ra ? ra.velocity : Vector2.zero) - (rb ? rb.velocity : Vector2.zero);
            Vector2 pa = self
                ? self.ClosestPoint(bGo ? (Vector2)bGo.transform.position : (Vector2)aGo.transform.position)
                : (Vector2)aGo.transform.position;
            Vector2 pb = other
                ? other.ClosestPoint(aGo
                    ? (Vector2)aGo.transform.position
                    : (Vector2)(bGo ? bGo.transform.position : Vector3.zero))
                : (bGo ? (Vector2)bGo.transform.position : pa);
            var mid = (pa + pb) * 0.5f;
            var dir = pa - pb;
            var n = dir.sqrMagnitude > 1e-6f
                ? dir.normalized
                : (aGo && bGo
                    ? ((Vector2)aGo.transform.position - (Vector2)bGo.transform.position).normalized
                    : Vector2.up);
            return new CollisionContext(aGo, bGo, self, other, rel, mid, n);
        }
#endif
        private void TryAccept(in CollisionContext ctx)
        {
            var pair = new CollisionPair(ctx.A, ctx.B);
            if (pairs.Contains(pair)) return;
            if (!ctx.A || !ctx.B) return;
            if (!ctx.A.TryGetComponent<CollisionParticipant>(out var pa)) return;
            if (!ctx.B.TryGetComponent<CollisionParticipant>(out var pb)) return;
            if (!pa.CheckAll(ctx)) return;
            var ctxForB =
#if ENABLE_2D_PHYSICS
ctx.Is2D
                ? new CollisionContext(ctx.B, ctx.A,
                    ctx.Is2D ? new Vector2(-ctx.RelativeVelocity.x, -ctx.RelativeVelocity.y) : Vector2.zero,
                    new Vector2(ctx.ContactPoint.x, ctx.ContactPoint.y),
                    new Vector2(-ctx.ContactNormal.x, -ctx.ContactNormal.y), ctx.IsTrigger ? default : ctx.Collision2D)
                :
#endif
                new CollisionContext(ctx.B, ctx.A, -ctx.RelativeVelocity, ctx.ContactPoint, -ctx.ContactNormal,
                    ctx.Collision3D);
            if (!pb.CheckAll(ctxForB)) return;
            pairs.Add(pair);
            NotifyAccepted(ctx);
            NotifyAccepted(ctxForB);
        }

        private static void NotifyAccepted(in CollisionContext ctx)
        {
            if (ctx.A.TryGetComponent<ICollisionListener>(out var l)) l.OnCollisionAccepted(ctx);
        }
    }
}