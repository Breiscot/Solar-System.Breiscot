using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class SpacetimeCurvature : MonoBehaviour
{
    [Header("Griglia")]
    public float gridExtent = 300f;
    public float cellSize = 15f;
    public int pointsPerLine = 80;

    [Header("Curvatura")]
    public float curvatureStrength = 0.003f;
    public float maxDepth = 30f;
    public float softeningDistance = 5f;

    [Header("Aspetto")]
    public Color gridColor = new Color(0f, 0.8f, 1f, 0.3f);
    public float lineWidth =  0.1f;

    private List<LineRenderer> linesX = new List<LineRenderer>();
    private List<LineRenderer> linesZ = new List<LineRenderer>();
    private CelestialBody[] bodies;
    private bool initialized = false;
    private bool visible = true;
    private GameObject gridParent;
    private int numLines;
    private Transform sunTransform;

    void Start()
    {
        Invoke("Initialize", 0.8f);
    }

    void Initialize()
    {
        bodies = FindObjectsOfType<CelestialBody>();

        foreach (CelestialBody body in bodies)
        {
            if (body.isSun)
            {
                sunTransform = body.transform;
                break;
            }
        }

        gridParent = new GameObject("SpacetimeGrid");
        gridParent.transform.parent = transform;

        CreateGridLines();
        initialized = true;
        Debug.Log("Curvatura spazio-tempo attivata (G per toggle)");
    }

    void CreateGridLines()
    {
        Material lineMat = new Material(Shader.Find("Sprites/Default"));
        lineMat.color = gridColor;

        numLines = Mathf.CeilToInt(gridExtent * 2f / cellSize) + 1;

        for (int i = 0; i < numLines; i++)
        {
            LineRenderer lr = CreateLine("LineX_" + i, lineMat);
            lr.positionCount = pointsPerLine;
            linesX.Add(lr);
        }

        for (int i = 0; i < numLines; i++)
        {
            LineRenderer lr = CreateLine("LineZ_" + i, lineMat);
            lr.positionCount = pointsPerLine;
            linesZ.Add(lr);
        }
    }

    LineRenderer CreateLine(string name, Material mat)
    {
        GameObject lineObj = new GameObject(name);
        lineObj.transform.parent = gridParent.transform;

        LineRenderer lr = lineObj.AddComponent<LineRenderer>();
        lr.material = mat;
        lr.startWidth = lineWidth;
        lr.endWidth = lineWidth;
        lr.startColor = gridColor;
        lr.endColor = gridColor;
        lr.useWorldSpace = true;
        lr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        lr.receiveShadows = false;

        return lr;
    }

    void Update()
    {
        if (!initialized) return;

        Keyboard keyboard = Keyboard.current;
        if (keyboard != null && keyboard.gKey.wasPressedThisFrame)
        {
            visible = !visible;
            gridParent.SetActive(visible);
            Debug.Log("Curvatura: " + (visible ? "ON" : "OFF"));
        }

        if (visible)
        {
            UpdateCurvature();
        }
    }

    void UpdateCurvature()
    {
        // Centro sulla griglia
        Vector3 center = Vector3.zero;
        if (sunTransform != null)
        {
            center = sunTransform.position;
        }

        // Aggiorna linee X
        for (int i = 0; i < numLines; i++)
        {
            float localZ = -gridExtent + i * cellSize;
            LineRenderer lr = linesX[i];

            for (int p = 0; p < pointsPerLine; p++)
            {
                float t = p / (float)(pointsPerLine - 1);
                float localX = -gridExtent + t * gridExtent * 2f;

                float worldX = center.x + localX;
                float worldZ = center.z + localZ;
                float y = center.y + CalculateDepth(worldX, worldZ);

                lr.SetPosition(p, new Vector3(worldX, y, worldZ));
            }
        }
        
        // Aggiorna linee Z
        for (int i = 0; i < numLines; i++)
        {
            float localX = -gridExtent + i * cellSize;
            LineRenderer lr = linesZ[i];

            for (int p = 0; p < pointsPerLine; p++)
            {
                float t = p / (float)(pointsPerLine - 1);
                float localZ = -gridExtent + t * gridExtent * 2f;

                float worldX = center.x + localX;
                float worldZ = center.z + localZ;
                float y = center.y + CalculateDepth(worldX, worldZ);

                lr.SetPosition(p, new Vector3(worldX, y, worldZ));
            }
        }
    }

    float CalculateDepth(float x, float z)
    {
        float totalDepth = 0f;

        foreach (CelestialBody body in bodies)
        {
            if (body == null) continue;

            Vector3 pos = body.transform.position;
            float dx = x - pos.x;
            float dz = z - pos.z;
            float distance = Mathf.Sqrt(dx * dx + dz * dz);

            // Curvatura con più massa, é più profondo, invece più vicino é più profondo
            float depth = curvatureStrength * body.mass / (distance + softeningDistance);

            totalDepth += depth;
        }

        // Limita la profondità massima
        totalDepth = Mathf.Min(totalDepth, maxDepth);

        return -totalDepth;
    }
}