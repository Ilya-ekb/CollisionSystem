using System.Collections.Generic;
using UnityEngine;
using CollisionSystem.Conditions;

namespace CollisionSystem.Core
{
    [DisallowMultipleComponent]
    public sealed class CollisionParticipant : MonoBehaviour
    {
        public IReadOnlyList<ICollisionCondition> Conditions => conditions;

        [SerializeField] private List<BaseCondition> conditions = new();
        
        internal bool CheckAll(in CollisionContext ctx)
        {
            var owner = this;
            if (conditions is null || conditions.Count == 0) return true;
            foreach (var c in conditions)
            {
                if (c is null) continue;
                if (!c.ShouldRegister(ctx, owner)) return false;
            }

            return true;
        }
    }
}