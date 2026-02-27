using UnityEngine;

namespace VHS
{
    [CreateAssetMenu(menuName = "Managers/SO_DoTweenManager")]
    public class SOManagerDoTween : SOSingleton<SOManagerDoTween>
    {
        // [SerializeField] private bool m_autoKill = true;
        // [SerializeField] private bool m_autoRecycable = true;
        // [SerializeField] private bool m_autoPlay = true;
        
        public override void OnGameStart() //TODO: Change Dotween
        {
            // DOTween.defaultAutoKill = m_autoKill;
            // DOTween.defaultRecyclable = m_autoRecycable;
            // DOTween.defaultAutoPlay = m_autoPlay ? AutoPlay.All : AutoPlay.None;
        }
    }
}
