using UnityEngine;

public class SnowIntensityController : MonoBehaviour
{
    [Header("Duration")]
    public float rampDuration = 300f; // 5 minutes

    [Header("Emission")]
    public float targetEmissionRate = 200f;

    [Header("Velocity X")]
    public float targetVelocityXMin = 0;
    public float targetVelocityXMax = 5f;

    private ParticleSystem ps;
    private ParticleSystem.EmissionModule emission;
    private ParticleSystem.VelocityOverLifetimeModule velocity;

    private float startEmissionRate;
    private float startVelocityXMin;
    private float startVelocityXMax;

    private float timer;

    void Awake()
    {
        ps = GetComponent<ParticleSystem>();

        emission = ps.emission;
        velocity = ps.velocityOverLifetime;

        // Read current values (starting point)
        startEmissionRate = emission.rateOverTime.constant;
        startVelocityXMin = velocity.x.constantMin;
        startVelocityXMax = velocity.x.constantMax;
    }

    void Update()
    {
        timer += Time.deltaTime;
        float t = Mathf.Clamp01(timer / rampDuration);

        // Emission
        emission.rateOverTime = Mathf.Lerp(
            startEmissionRate,
            targetEmissionRate,
            t
        );

        // Velocity X (min/max)
        velocity.x = new ParticleSystem.MinMaxCurve(
            Mathf.Lerp(startVelocityXMin, targetVelocityXMin, t),
            Mathf.Lerp(startVelocityXMax, targetVelocityXMax, t)
        );
    }
}
