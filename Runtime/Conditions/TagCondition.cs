using CollisionSystem.Core;
using UnityEngine;

namespace CollisionSystem.Conditions
{
    [CreateAssetMenu(menuName = "Collision System/Conditions/Tag Condition", fileName = "TagCondition")]
    public sealed class TagCondition : BaseCondition
    {
        [SerializeField] private string requiredTag = "Untagged";
        [SerializeField] private bool checkOther = true;

        public override bool ShouldRegister(in CollisionContext context, Component owner)
        {
            var go = checkOther ? context.B : context.A;
            if (!go) return false;
            if (string.IsNullOrEmpty(requiredTag)) return true;
            return go.CompareTag(requiredTag);
        }
    }
}