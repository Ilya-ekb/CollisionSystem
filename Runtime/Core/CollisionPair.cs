using UnityEngine;

namespace CollisionSystem.Core
{
    public readonly struct CollisionPair
    {
        public readonly int LowId;
        public readonly int HighId;

        public CollisionPair(GameObject a, GameObject b)
        {
            int ia = a ? a.GetInstanceID() : 0;
            int ib = b ? b.GetInstanceID() : 0;
            if (ia <= ib)
            {
                LowId = ia;
                HighId = ib;
            }
            else
            {
                LowId = ib;
                HighId = ia;
            }
        }

        public override int GetHashCode() => (LowId * 486187739) ^ HighId;

        public override bool Equals(object obj) =>
            obj is CollisionPair other && other.LowId == LowId && other.HighId == HighId;
    }
}