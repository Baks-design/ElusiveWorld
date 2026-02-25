using ElusiveWorld.Core.Character;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class StanceVignette : MonoBehaviour
{
    [SerializeField] float min = 0.1f;
    [SerializeField] float max = 0.35f;
    [SerializeField] float response = 10f;
    VolumeProfile profile;
    Vignette vignette;

    public void Initialize(VolumeProfile profile)
    {
        this.profile = profile;

        if (!profile.TryGet(out vignette))
            vignette = profile.Add<Vignette>();
        vignette.intensity.Override(min);
    }

    public void UpdateVignette(float deltaTime, Stance stance)
    {
        var targetIntensity = stance is Stance.Stand ? min : max;
        vignette.intensity.value = Mathf.Lerp(
            vignette.intensity.value, targetIntensity, 1f - Mathf.Exp(-response * deltaTime));
    }
}