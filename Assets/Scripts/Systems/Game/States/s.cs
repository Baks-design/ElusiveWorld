namespace ElusiveWorld.Core.Assets.Scripts.Systems.Game.States
{
    public interface IState
    {
        void Update() { }
        void FixedUpdate() { }
        void OnEnter() { }
        void OnExit() { }
    }
}