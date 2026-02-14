using UnityEngine;

public class CelestialBody : MonoBehaviour
{
    [Header("Proprietà Fisiche")]
    public float mass = 1000f;
    public float radius = 1f;

    [Header("Orbita")]
    public bool autoCalculateVelocity = false;
    public Transform orbitAround;
    public Vector3 initialVelocity;

    [Header("Visualizzazione")]
    public Color bodyColor = Color.white;
    public Color orbitColor = Color.white;
    public bool drawOrbitPath = true;

    // Velocità corrente (per il GravityManager é pubblica)
    [HideInInspector]
    public Vector3 currentVelocity;

    // Per tracciare l'orbita
    private LineRenderer orbitLine;
    private Vector3[] orbitPosition;
    private int maxOrbitPoints = 500;
    private int currentOrbitIndex = 0;

    void Start()
    {
         // Scala l'oggetto
        transform.localScale = Vector3.one * radius * 2;

        // Colore
        Renderer rend = GetComponent<Renderer>();
        if (rend != null)
        {
            rend.material.color = bodyColor;
        }

        // Imposta la velocità orbitale automatica
        if (autoCalculateVelocity && orbitAround != null)
        {
            CalculateOrbitalVelocity();
        }
        else
        {
            currentVelocity = initialVelocity;
        }

        // Setup del dell'orbita
        if (drawOrbitPath)
        {
            SetupOrbitLine();
        }
    }

    void CalculateOrbitalVelocity()
    {
        // Trova il GravityManager per la costante G
        GravityManager gm = FindObjectOfType<GravityManager>();
        if (gm == null) return;

        // Direzione verso il corpo centrale
        Vector3 direction = orbitAround.position - transform.position;
        float distance = direction.magnitude;

        // v = sqrt(G * M / r)
        CelestialBody otherBody = orbitAround.GetComponent<CelestialBody>();
        if (otherBody == null) return;

        float orbitalSpeed = Mathf.Sqrt(gm.gravitationalConstant * otherBody.mass / distance);

        // Direzione tangente
        // Ruota di 90° sul piano XZ
        Vector3 tangent = new Vector3(-direcition.normalized.z, 0, direction.normalized.x);

        currentVelocity = tangent * orbitalSpeed;

        // Se il corpo attorno a cui orbita si muove, aggiungi la sua velocità
        currentVelocity += otherBody.currentVelocity;

    }

    void SetupOrbitLine()
    {
        orbitLine = gameObject.AddComponent<LineRenderer>();
        orbitLine.positionCount = maxOrbitPoints;
        orbitLine.startWidth = 0.1f;
        orbitLine.endWidth = 0.1f;
        orbitLine.material = new Material(Shader.Find("Sprites/Default"));
        orbitLine.startColor = orbitColor;
        orbitLine.endColor = orbitColor;

        orbitPosition = new Vector3[maxOrbitPoints];
        for (int i = 0; i < maxOrbitPoints; i++)
        {
            orbitPosition[i] = transform.position;
        }
    }

    // Chiamato dal GravityManager
    public void UpdateVelocity(Vector3 acceleration, float timeStep)
    {
        currentVelocity += acceleration * timeStep;
    }

    // Chiamato dal GravityManager
    public void UpdatePosition(float timeStep)
    {
        transform.position += currentVelocity * timeStep;

        // Aggiorna la linea dell'orbita
        if (drawOrbitPath && orbitLine != null)
        {
            orbitPosition[currentOrbitIndex] = transform.position;
            currentOrbitIndex = (currentOrbitIndex + 1) % maxOrbitPoints;
            orbitLine.SetPositions(orbitPosition);
        }
    }
}