using System;

namespace ElusiveWorld.Core.Assets.Scripts.Systems.SceneManagement
{
    public class LoadingProgress : IProgress<float>
    {
        public event Action<float> Progressed = delegate { };

        public void Report(float value) => Progressed.Invoke(value / 1f);
    }
}