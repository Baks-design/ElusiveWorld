using UnityEngine;

namespace VHS
{
    public class PerlinNoiseScroller
    {
        readonly PerlinNoiseData data;
        Vector3 noiseOffset;
        Vector3 noise;
      
        public Vector3 Noise => noise;
     
        public PerlinNoiseScroller(PerlinNoiseData data)
        {
            this.data = data;

            var rand = 32f;
            noiseOffset.x = Random.Range(0f, rand);
            noiseOffset.y = Random.Range(0f, rand);
            noiseOffset.z = Random.Range(0f, rand);
        }

        public void UpdateNoise()
        {
            var scrollOffset = Time.deltaTime * data.frequency;
            noiseOffset.x += scrollOffset;
            noiseOffset.y += scrollOffset;
            noiseOffset.z += scrollOffset;

            noise.x = Mathf.PerlinNoise(noiseOffset.x, 0f);
            noise.y = Mathf.PerlinNoise(noiseOffset.x, 1f);
            noise.z = Mathf.PerlinNoise(noiseOffset.x, 2f);

            noise -= Vector3.one * 0.5f;
            noise *= data.amplitude;
        }
    }
}
