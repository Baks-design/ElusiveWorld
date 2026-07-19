namespace ElusiveWorld.Internal.Runtime.Systems.Physics
{
    public interface IJumpHandler
    {
        void Jump(IPhysicsEntity entity, float jumpForce);
    }
}