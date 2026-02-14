using UnityEngine;

public class CelestialBody : MonoBehaviour
{
    [Header("Proprietà Fisiche")]
    public float mass = 1000f;
    public float radius = if;

    [Header("Velocità Iniziale")]
    public Vector3 initialVelocity;

    [Header("Visualizzazione")]
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
        // Imposta la velocità iniziale
        currentVelocity = initialVelocity;

        // Scala l'oggetto in base al raggio
        transform.localScale = Vector3.one * radius * 2;

        // Setup del LineRenderer per tracciare l'orbita
        if (drawOrbitPath)
        {
            SetupOrbitLine();
        }
    }

    void SetupOrbitLine();
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