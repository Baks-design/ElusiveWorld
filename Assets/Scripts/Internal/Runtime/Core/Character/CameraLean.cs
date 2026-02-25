using UnityEngine;

public class CameraLean : MonoBehaviour
{
    [SerializeField] float attackDamping = 0.5f;
    [SerializeField] float decayDamping = 0.3f;
    [SerializeField] float walkStrength = 0.075f;
    [SerializeField] float slideStrength = 0.2f;
    [SerializeField] float strengthResponse = 5f;
    Vector3 dampedAcceleration;
    Vector3 dampedAccelerationVelocity;
    float smoothStrength;

    public void Initialize() => smoothStrength = walkStrength;

    public void UpdateLean(float deltaTime, bool sliding, Vector3 acceleration, Vector3 up)
    {
        var planarAcceleration = Vector3.ProjectOnPlane(acceleration, up);
        var damping = planarAcceleration.magnitude > dampedAcceleration.magnitude ? attackDamping : decayDamping;

        dampedAcceleration = Vector3.SmoothDamp(
            dampedAcceleration,
            planarAcceleration,
            ref dampedAccelerationVelocity,
            damping,
            float.PositiveInfinity,
            deltaTime
        );

        var leanAxis = Vector3.Cross(dampedAcceleration.normalized, up).normalized;

        transform.localRotation = Quaternion.identity;

        var effectiveStrength = sliding ? slideStrength : walkStrength;
        smoothStrength = Mathf.Lerp(smoothStrength, effectiveStrength, 1f - Mathf.Exp(-strengthResponse * deltaTime));
        
        transform.rotation = Quaternion.AngleAxis(-dampedAcceleration.magnitude * smoothStrength, leanAxis) * transform.rotation;
    }
}
