using System;

namespace ElusiveWorld.Core.Assets.Scripts.Systems.SceneManagement
{
    public class LoadingProgress : IProgress<float>
    {
        const float ratio = 1f;

        public event Action<float> Progressed = delegate { };

        public void Report(float value) => Progressed.Invoke(value / ratio);
    }
}