using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace CollisionSystem.Core
{
    [DefaultExecutionOrder(-5000)]
    public sealed class CollisionManager : MonoBehaviour
    {
        public static CollisionManager Default { get; private set; }

        private readonly HashSet<CollisionPair> pairsThisStep = new();

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
            pairsThisStep.Clear();
        }

        public void Register(GameObject reporter, Collision collision)
        {
            var otherGo = collision.gameObject;
            var ctx = Build3DContext(reporter, otherGo, collision);
            TryAccept(ctx);
        }

        public void Register(GameObject reporter, Collision2D collision)
        {
            var otherGo = collision.gameObject;
            var ctx = Build2DContext(reporter, otherGo, collision);
            TryAccept(ctx);
        }

        private static CollisionContext Build3DContext(GameObject a, GameObject b, Collision collision)
        {
            var point = collision.contactCount > 0 ? collision.GetContact(0).point : a.transform.position;
            var normal = collision.contactCount > 0
                ? collision.GetContact(0).normal
                : -collision.relativeVelocity.normalized;
            var ctx = new CollisionContext(a, b, collision.relativeVelocity, point, normal, collision);
            return ctx;
        }

        private static CollisionContext Build2DContext(GameObject a, GameObject b, Collision2D col2D)
        {
            var point = col2D.contactCount > 0 ? col2D.GetContact(0).point : (Vector2)a.transform.position;
            var normal = col2D.contactCount > 0 ? col2D.GetContact(0).normal : -col2D.relativeVelocity.normalized;
            var ctx = new CollisionContext(a, b, col2D.relativeVelocity, point, normal, col2D);
            return ctx;
        }

        private void TryAccept(in CollisionContext ctx)
        {
            var pair = new CollisionPair(ctx.A, ctx.B);
            if (pairsThisStep.Contains(pair)) return;
            if (!ctx.A || !ctx.B) return;
            if (!ctx.A.TryGetComponent<CollisionParticipant>(out var pa)) return;
            if (!ctx.B.TryGetComponent<CollisionParticipant>(out var pb)) return;
            if (!pa.CheckAll(ctx)) return;
            var ctxForB = ctx.Is2D
                ? new CollisionContext(ctx.B, ctx.A, new Vector2(-ctx.RelativeVelocity.x, -ctx.RelativeVelocity.y),
                    new Vector2(ctx.ContactPoint.x, ctx.ContactPoint.y),
                    new Vector2(-ctx.ContactNormal.x, -ctx.ContactNormal.y), ctx.Collision2D)
                : new CollisionContext(ctx.B, ctx.A, -ctx.RelativeVelocity, ctx.ContactPoint, -ctx.ContactNormal,
                    ctx.Collision3D);
            if (!pb.CheckAll(ctxForB)) return;
            pairsThisStep.Add(pair);
            NotifyAccepted(ctx);
            NotifyAccepted(ctxForB);
        }

        private static void NotifyAccepted(in CollisionContext ctx)
        {
            if (ctx.A.TryGetComponent<ICollisionListener>(out var listener))
                listener.OnCollisionAccepted(ctx);
        }
    }
}