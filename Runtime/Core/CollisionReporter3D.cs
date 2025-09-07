using UnityEngine;

namespace CollisionSystem.Core
{
    [RequireComponent(typeof(Collider))]
    public sealed class CollisionReporter3D : MonoBehaviour
    {
        private void OnCollisionEnter(Collision collision)
        {
            if (CollisionManager.Default != null)
                CollisionManager.Default.Register(gameObject, collision);
        }
    }
}