using System;
using UnityEngine;

namespace ElusiveWorld.Core.Assets.Scripts.Behaviours.Characters
{
    public class PerlinNoiseScroller
    {
        readonly PerlinNoiseData data;
        readonly Vector3 scrollVelocity;
        const float MIN_VALUE = 0.0001f;
        const float OFFSET1 = 17.3f;
        const float OFFSET2 = 29.1f;
        Vector3 noiseOffset;
        Vector3 noise;

        public Vector3 Noise => noise;
        public Vector3 NormalizedNoise
        {
            get
            {
                var amp = Mathf.Max(MIN_VALUE, data.amplitude);
                return (noise / amp) + Vector3.one * 0.5f;
            }
        }

        public PerlinNoiseScroller(PerlinNoiseData data, Vector3? scrollVelocity = null)
        {
            this.data = data != null ? data : throw new ArgumentNullException(nameof(data));
            this.scrollVelocity = scrollVelocity ?? Vector3.one;

            Reset();
        }

        public void Update()
        {
            var dt = Time.deltaTime;
            noiseOffset += scrollVelocity * dt;

            var freq = Mathf.Max(MIN_VALUE, data.frequency);
            var x = noiseOffset.x * freq;
            var y = noiseOffset.y * freq;

            noise = new Vector3(
                Mathf.PerlinNoise(x, y),
                Mathf.PerlinNoise(x + OFFSET1, y + OFFSET1),
                Mathf.PerlinNoise(x + OFFSET2, y + OFFSET2)
            );
            noise = (noise - Vector3.one * 0.5f) * data.amplitude;
        }

        public void Reset()
        {
            const float BASE_RANGE = 1000f;
            noiseOffset = new Vector3(
                UnityEngine.Random.Range(0f, BASE_RANGE),
                UnityEngine.Random.Range(0f, BASE_RANGE),
                UnityEngine.Random.Range(0f, BASE_RANGE)
            );
        }
    }
}