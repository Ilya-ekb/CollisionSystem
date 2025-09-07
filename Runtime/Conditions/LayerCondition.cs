using UnityEngine;
using CollisionSystem.Core;

namespace CollisionSystem.Conditions
{
    [CreateAssetMenu(menuName = "Collision System/Conditions/Layer Condition", fileName = "LayerCondition")]
    public sealed class LayerCondition : BaseCondition
    {
        [SerializeField] private LayerMask allowedLayers = ~0;
        [SerializeField] private bool checkOther = true;

        public override bool ShouldRegister(in CollisionContext context, Component owner)
        {
            var go = checkOther ? context.B : context.A;
            if (!go) return false;
            return ((1 << go.layer) & allowedLayers.value) != 0;
        }
    }
}