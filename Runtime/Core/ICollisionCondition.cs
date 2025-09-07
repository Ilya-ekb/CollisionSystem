namespace CollisionSystem.Core
{
    public interface ICollisionCondition
    {
        bool ShouldRegister(in CollisionContext context, UnityEngine.Component owner);
    }
}