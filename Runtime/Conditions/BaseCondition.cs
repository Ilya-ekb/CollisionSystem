using UnityEngine;
using CollisionSystem.Core;

namespace CollisionSystem.Conditions
{
    public abstract class BaseCondition : ScriptableObject, ICollisionCondition
    {
        public abstract bool ShouldRegister(in CollisionContext context, Component owner);
    }
}