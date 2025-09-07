using CollisionSystem.Core;
using UnityEngine;

namespace CollisionSystem.Conditions
{
    [CreateAssetMenu(menuName = "Collision System/Conditions/Min Relative Velocity", fileName = "MinRelativeVelocity")]
    public sealed class MinRelativeVelocity : BaseCondition
    {
        [SerializeField] [Min(0f)] private float minSpeed = 1f;

        public override bool ShouldRegister(in CollisionContext context, Component owner)
        {
            return context.RelativeSpeed >= minSpeed;
        }
    }
}

namespace CollisionSystem.Conditions {
}