using UnityEngine;

public class CameraSpring : MonoBehaviour
{
    [SerializeField] float halfLife = 0.075f;
    [SerializeField] float frequency = 18f;
    [SerializeField] float angularDisplacement = 2f;
    [SerializeField] float linearDisplacement = 0.05f;
    Vector3 springPosition;
    Vector3 springVelocity;

    public void Initialize()
    {
        springPosition = transform.position;
        springVelocity = Vector3.zero;
    }

    public void UpdateSpring(float deltaTime, Vector3 up)
    {
        Spring(ref springPosition, ref springVelocity, transform.position, halfLife, frequency, deltaTime);

        var relativeSpringPosition = springPosition - transform.position;
        var springHeight = Vector3.Dot(relativeSpringPosition, up);

        transform.localEulerAngles = new Vector3(-springHeight * angularDisplacement, 0f, 0f);
        transform.localPosition = relativeSpringPosition * linearDisplacement;
    }

    static void Spring(
        ref Vector3 current, ref Vector3 velocity, Vector3 target,
        float halfLife, float frequency, float timeStep)
    {
        var dampingRatio = -Mathf.Log(0.5f) / (frequency * halfLife);
        var f = 1f + 2f * timeStep * dampingRatio * frequency;
        var oo = frequency * frequency;
        var hoo = timeStep * oo;
        var hhoo = timeStep * hoo;
        var detInv = 1f / (f + hhoo);
        var detX = f * current + timeStep * velocity + hhoo * target;
        var detV = velocity + hoo * (target - current);
        current = detX * detInv;
        velocity = detV * detInv;
    }
}