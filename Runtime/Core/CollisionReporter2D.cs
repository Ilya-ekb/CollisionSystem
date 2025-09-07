#if UNITY_2D_PHYSICS
using UnityEngine;

namespace CollisionSystem.Core
{
    [RequireComponent(typeof(Collider2D))]
    public sealed class CollisionReporter2D : MonoBehaviour
    {
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (CollisionManager.Default != null)
                CollisionManager.Default.Register(gameObject, collision);
        }
    }
}
#endif