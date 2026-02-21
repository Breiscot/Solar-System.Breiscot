using UnityEngine;

public class SunGlow : MonoBehaviour
{
    [Header("Glow")]
    public float baseGlowSize = 2f;
    public float maxGlowSize = 8f;
    public float glowGrowDistance = 200f;
    public Color glowColor = new Color(1f, 0.9f, 0.3f, 0.15f);

    [Header("Corona")]
    public int coronaLayers = 3;

    private GameObject[] glowLayers;
    private Transform cam;
    private float sunRadius;

    void Start()
    {
        cam = Camera.main.transform;
        sunRadius = transform.localScale.x / 2f;

        CreateGlow();
    }

    void CreateGlow()
    {
        glowLayers = new GameObject[coronaLayers];

        // Shader per il glow
        Material glowMat = new Material(Shader.Find("Sprites/Default"));

        for (int i = 0; i < coronaLayers; i++)
        {
            GameObject glow = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            glow.name = "SunGlow_" + i;
            glow.transform.parent = transform;
            glow.transform.localPosition = Vector3.zero;

            // Rimuovi collider
            Destroy(glow.GetComponent<Collider>());

            // Ogni strato é più grande e più transparente
            float layerScale = baseGlowSize + (i * 0.8f);
            glow.transform.localScale = Vector3.one * layerScale;

            // Materiale transparente
            Material mat = new Material(glowMat);
            Color layerColor = glowColor;
            layerColor.a = glowColor.a / (i + 1);
            mat.color = layerColor;

            // Renderizza come transparente
            mat.SetFloat("_Mode", 3);
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.One);
            mat.SetInt("_ZWrite", 0);
            mat.renderQueue = 3000 + i;

            glow.GetComponent<Renderer>().material = mat;
            glow.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

            glowLayers[i] = glow;
        }
    }

    void Update()
    {
        if (cam == null) return;
        
        float distance = Vector3.Distance(cam.position, transform.position);

        // Più lontano = glow maggiore
        float distanceFactor = Mathf.Clamp01(distance / glowGrowDistance);

        for (int i = 0; i < coronaLayers; i++)
        {
            if (glowLayers[i] == null) continue;

            float baseScale = baseGlowSize + (i * 0.8f);
            float targetScale = Mathf.Lerp(baseScale, maxGlowSize + (i * 1.5f), distanceFactor);

            glowLayers[i].transform.localScale = Vector3.one * targetScale;

            // Glow sempre rivolto alla camera
            glowLayers[i].transform.LookAt(cam);

            // Più lontano = leggermente più opaco
            Renderer rend = glowLayers[i].GetComponent<Renderer>();
            Color c = rend.material.color;
            c.a = (glowColor.a / (i + 1)) * (0.5f + distanceFactor * 0.5f);
            rend.material.color = c;
        }
    }
}