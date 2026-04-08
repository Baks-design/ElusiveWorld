using UnityEngine;

namespace ElusiveWorld.Core.Assets.Scripts.Behaviours.Characters
{
    public class PerlinNoiseScroller
    {
        readonly PerlinNoiseData data;
        Vector3 noiseOffset;
        Vector3 noise;
        Vector3 scrollSpeeds;

        public Vector3 Noise => noise;
        public Vector3 NormalizedNoise => (noise / data.amplitude) + Vector3.one * 0.5f;

        public PerlinNoiseScroller(PerlinNoiseData data, Vector3? scrollSpeeds = null)
        {
            this.data = data;
            this.scrollSpeeds = scrollSpeeds ?? Vector3.one;

            var range = 100f / data.frequency;
            noiseOffset = new Vector3(
                Random.Range(0f, range),
                Random.Range(0f, range),
                Random.Range(0f, range)
            );
        }

        public void Update()
        {
            var delta = Time.deltaTime * data.frequency;
            noiseOffset.x += delta * scrollSpeeds.x;
            noiseOffset.y += delta * scrollSpeeds.y;
            noiseOffset.z += delta * scrollSpeeds.z;

            noise.x = Mathf.PerlinNoise(noiseOffset.x, noiseOffset.y);
            noise.y = Mathf.PerlinNoise(noiseOffset.y, noiseOffset.z);
            noise.z = Mathf.PerlinNoise(noiseOffset.z, noiseOffset.x);

            noise = (noise - Vector3.one * 0.5f) * data.amplitude;
        }

        public void Reset()
        {
            var range = 100f / data.frequency;
            noiseOffset = new Vector3(
                Random.Range(0f, range),
                Random.Range(0f, range),
                Random.Range(0f, range)
            );
        }
    }
}