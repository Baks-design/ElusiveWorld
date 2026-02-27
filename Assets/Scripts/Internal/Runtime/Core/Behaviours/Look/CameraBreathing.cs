using UnityEngine;

namespace VHS
{
    public class CameraBreathing : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] PerlinNoiseData data;
        [Header("Axis")]
        [SerializeField] bool x;
        [SerializeField] bool y;
        [SerializeField] bool z;
        PerlinNoiseScroller perlinNoiseScroller;
        Vector3 finalRot;
        Vector3 finalPos;

        void Start() => perlinNoiseScroller = new PerlinNoiseScroller(data);

        void LateUpdate()
        {
            perlinNoiseScroller.UpdateNoise();

            var posOffset = Vector3.zero;
            var rotOffset = Vector3.zero;

            switch (data.transformTarget)
            {
                case TransformTarget.Position:
                    if (x)
                        posOffset.x += perlinNoiseScroller.Noise.x;

                    if (y)
                        posOffset.y += perlinNoiseScroller.Noise.y;

                    if (z)
                        posOffset.z += perlinNoiseScroller.Noise.z;

                    finalPos.x = x ? posOffset.x : transform.localPosition.x;
                    finalPos.y = y ? posOffset.y : transform.localPosition.y;
                    finalPos.z = z ? posOffset.z : transform.localPosition.z;

                    transform.localPosition = finalPos;
                    break;
                case TransformTarget.Rotation:
                    if (x) rotOffset.x += perlinNoiseScroller.Noise.x;
                    if (y) rotOffset.y += perlinNoiseScroller.Noise.y;
                    if (z) rotOffset.z += perlinNoiseScroller.Noise.z;

                    finalRot.x = x ? rotOffset.x : transform.localEulerAngles.x;
                    finalRot.y = y ? rotOffset.y : transform.localEulerAngles.y;
                    finalRot.z = z ? rotOffset.z : transform.localEulerAngles.z;

                    transform.localEulerAngles = finalRot;
                    break;

                case TransformTarget.Both:
                    if (x)
                    {
                        posOffset.x += perlinNoiseScroller.Noise.x;
                        rotOffset.x += perlinNoiseScroller.Noise.x;
                    }
                    if (y)
                    {
                        posOffset.y += perlinNoiseScroller.Noise.y;
                        rotOffset.y += perlinNoiseScroller.Noise.y;
                    }
                    if (z)
                    {
                        posOffset.z += perlinNoiseScroller.Noise.z;
                        rotOffset.z += perlinNoiseScroller.Noise.z;
                    }

                    finalPos.x = x ? posOffset.x : transform.localPosition.x;
                    finalPos.y = y ? posOffset.y : transform.localPosition.y;
                    finalPos.z = z ? posOffset.z : transform.localPosition.z;

                    finalRot.x = x ? rotOffset.x : transform.localEulerAngles.x;
                    finalRot.y = y ? rotOffset.y : transform.localEulerAngles.y;
                    finalRot.z = z ? rotOffset.z : transform.localEulerAngles.z;

                    transform.localPosition = finalPos;
                    transform.localEulerAngles = finalRot;
                    break;
            }
        }
    }
}
