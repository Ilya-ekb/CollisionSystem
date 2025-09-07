namespace CollisionSystem.Core
{
    public interface ICollisionListener
    {
        void OnCollisionAccepted(in CollisionContext context);
    }
}