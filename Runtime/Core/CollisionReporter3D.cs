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

        private void OnTriggerEnter(Collider other)
        {
            if (!isActiveAndEnabled || other == null) return;
            if (CollisionManager.Default != null)
            {
                var self = GetComponent<Collider>();
                CollisionManager.Default.Register(gameObject, self, other);
            }
        }
    }
}