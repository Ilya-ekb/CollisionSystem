using CollisionSystem.Core;
using UnityEngine;

namespace CollisionSystem.Conditions
{
    [CreateAssetMenu(menuName = "Collision System/Conditions/Require Component", fileName = "RequireComponentCond")]
    public sealed class RequireComponent : BaseCondition
    {
        [SerializeField] private string requiredComponentTypeName;
        [SerializeField] private bool checkOther = true;

        public override bool ShouldRegister(in CollisionContext context, Component owner)
        {
            if (string.IsNullOrEmpty(requiredComponentTypeName)) return true;
            var go = checkOther ? context.B : context.A;
            if (!go) return false;
            var type = System.Type.GetType(requiredComponentTypeName);
            if (type == null) return false;
            return go.GetComponent(type) != null;
        }
    }
}