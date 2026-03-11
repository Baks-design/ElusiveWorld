using System;

namespace ElusiveWorld.Core.Assets.Scripts.Systems.Game
{
    public class ShowLoadingScreenDisposable : IDisposable
    {
        readonly LoadingScreen loadingScreen;

        public ShowLoadingScreenDisposable(LoadingScreen loadingScreen)
        {
            this.loadingScreen = loadingScreen;
            loadingScreen.Show();
        }

        public void SetLoadingBarPercent(float percent)
            => loadingScreen.SetLoadingBarPercent(percent);

        public void Dispose() => loadingScreen.Hide();
    }
}