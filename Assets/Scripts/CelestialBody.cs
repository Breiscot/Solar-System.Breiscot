using UnityEngine;

public class CelestialBody : MonoBehaviour
{
    [Header("Proprietà Fisiche")]
    public float mass = 1000f;
    public float radius = 1f;

    [Header("Tipo")]
    public bool isSun = false;
    public bool isStatic = false;
    public bool isMoon = false;

    [Header("Orbita")]
    public bool autoCalculateVelocity = false;
    public Transform orbitAround;
    public Vector3 initialVelocity;

    [Header("Movimento Galattico (solo Sole)")]
    public Vector3 galacticVelocity = Vector3.zero;

    [Header("Visualizzazione")]
    public Color bodyColor = Color.white;
    public Color orbitColor = Color.white;
    public bool drawOrbitPath = true;
    public float trailLength = 20f;

    // Velocità corrente (per il GravityManager é pubblica)
    [HideInInspector]
    public Vector3 currentVelocity;
    [HideInInspector]
    public Vector3 currentAcceleration;

    void Start()
    {
        transform.localScale = Vector3.one * radius * 2;

        if (!autoCalculateVelocity)
        {
            currentVelocity = initialVelocity;
        }

        if (isSun)
        {
            currentVelocity = galacticVelocity;
        }

        if (drawOrbitPath && !isSun)
        {
            SetupTrail();
        }
    }

    public void CalculateOrbitalVelocity(float G)
    {
        if (orbitAround == null) return;

        // Direzione verso il corpo centrale
        Vector3 direction = orbitAround.position - transform.position;
        float distance = direction.magnitude;

        // v = sqrt(G * M / r)
        CelestialBody otherBody = orbitAround.GetComponent<CelestialBody>();
        if (otherBody == null) return;

        float orbitalSpeed = Mathf.Sqrt(G * otherBody.mass / distance);

        Vector3 tangent = new Vector3(-direction.normalized.z, 0, direction.normalized.x);
        currentVelocity = tangent * orbitalSpeed;

        currentVelocity += otherBody.currentVelocity;

        Debug.Log(gameObject.name + ": velocità = " + orbitalSpeed + " parent vel=" + otherBody.currentVelocity.magnitude);
    }

    void SetupTrail()
    {
        TrailRenderer trail = gameObject.AddComponent<TrailRenderer>();
        trail.time = trailLength;
        trail.startWidth = 0.15f;
        trail.endWidth = 0.02f;

        Color startColor = orbitColor;
        Color endColor = orbitColor;
        endColor.a = 0f;

        trail.startColor = startColor;
        trail.endColor = endColor;
        trail.material = new Material(Shader.Find("Sprites/Default"));
        trail.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        trail.receiveShadows = false;
        trail.minVertexDistance = 0.3f;
    }
}