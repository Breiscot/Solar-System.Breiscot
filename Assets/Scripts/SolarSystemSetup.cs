using UnityEngine;

public class SolarSystemSetup : MonoBehaviour
{

    [SerializeField] private Texture2D[] planetTextures;
    void Start()
    {
        CreateSun();
                    // Name        Massa | Raggio | Dist | Colore                          Tilt | Rot.Speed
        CreatePlanet("Mercury",     1f,    0.4f,   25f,    new Color(0.7f, 0.7f, 0.7f),     0.03f,  15f);
        CreatePlanet("Venus",       3f,    0.9f,   40f,    new Color(0.9f, 0.7f, 0.3f),     177f,   -5f);
        CreatePlanet("Earth",       500f,  1.0f,   55f,    new Color(0.2f, 0.5f, 1.0f),     23.4f,  60f);
        CreatePlanet("Mars",        2f,    0.7f,   75f,    new Color(0.9f, 0.3f, 0.2f),     25.2f,  58f);
        CreatePlanet("Jupiter",     20f,   3.0f,   120f,   new Color(0.8f, 0.6f, 0.4f),     3.1f,   150f);
        CreatePlanet("Saturn",      15f,   2.5f,   170f,   new Color(0.9f, 0.8f, 0.5f),     26.7f,  140f);
        CreatePlanet("Uranus",      8f,    1.8f,   230f,   new Color(0.5f, 0.8f, 0.9f),     97.8f,  80f);
        CreatePlanet("Neptune",     8f,    1.7f,   280f,   new Color(0.3f, 0.4f, 0.9f),     28.3f,  90f);

        CreateMoon("Moon", 0.1f, 0.3f, "Earth", 4f, new Color(0.8f, 0.8f, 0.8f), 6.7f, 20f);
    }

    Material CreateMaterial(string planetName, Color fallbackColor, bool emissive)
    {
        Material loadedMat = Resources.Load<Material>("Materials/" + planetName + "_Mat");

        if (loadedMat != null)
        {
            Debug.Log("YES " + planetName + ": materiale pre-creato trovato.");
            return new Material(loadedMat); // Crea copia
        }

        Texture2D texture = Resources.Load<Texture2D>("Textures/" + planetName);
        
        if (texture == null)
            texture = Resources.Load<Texture2D>("Textures/" + planetName.ToLower());

        Material mat;

        // Trova lo shader
        Shader shader = Shader.Find("Universal Render Pipeline/Lit");

        if (shader == null)
            shader = Shader.Find("Universal Render Pipeline/Simple Lit");

        if (shader == null)
            shader = Shader.Find("Standard");
            
        if (shader != null)
        {
            mat = new Material(shader);
        }
        else
        {
            // FallBack
            mat = new Material(Shader.Find("Sprites/Default"));
        }

        mat.SetInt("_Cull", 2); // 2 = Nero

        if (mat.HasProperty("_DoubleSidedEnable"))
            mat.SetFloat("_DoubleSidedEnable", 0f);

        if (mat.HasProperty("_CullMode"))
            mat.SetFloat("_CullMode", 2f);

        if (texture != null)
        {
            Debug.Log(planetName + ": texture caricata.");
            texture.filterMode = FilterMode.Bilinear;
            texture.wrapMode = TextureWrapMode.Clamp;

            mat.mainTexture = texture;

            if (mat.HasProperty("_BaseMap"))
                mat.SetTexture("_BaseMap", texture);
            if (mat.HasProperty("_MainTex"))
                mat.SetTexture("_MainTex", texture);

            // Colore Bianco per mostrare texture con maggiore probabilità
            mat.color = Color.white;
            if (mat.HasProperty("_BaseColor"))
                mat.SetColor("_BaseColor", Color.white);

            // No riflessi metallici
            if (mat.HasProperty("_Metallic"))
                mat.SetFloat("_Metallic", 0f);
            if (mat.HasProperty("_Smoothness"))
                mat.SetFloat("_Smoothness", 0.0f);
        }
        else
        {
            Debug.LogWarning(planetName + ": texture non trovata.");

            mat.color = fallbackColor;
            if (mat.HasProperty("_BaseColor"))
                mat.SetColor("_BaseColor", fallbackColor);
            if (mat.HasProperty("_Metallic"))
                mat.SetFloat("_Metallic", 0f);
            if (mat.HasProperty("_Smoothness"))
                mat.SetFloat("_Smoothness", 0.1f);
        }

        if (emissive)
        {
            mat.EnableKeyword("_EMISSION");

            if (texture != null)
            {
                if (mat.HasProperty("_EmissionMap"))
                    mat.SetTexture("_EmissionMap", texture);
                if (mat.HasProperty("_EmissionColor"))
                    mat.SetColor("_EmissionColor", Color.white * 2f);
            }
            else
            {
                if (mat.HasProperty("_EmissionColor"))
                    mat.SetColor("_EmissionColor", fallbackColor * 3f);
            }
        }

        mat.renderQueue = 2000;

        return mat;
    }

    void CreateSun()
    {
        GameObject sun = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sun.name = "Sun";
        sun.transform.position = Vector3.zero;

        // Corpo celeste
        CelestialBody body = sun.AddComponent<CelestialBody>();
        body.mass = 100000f;
        body.radius = 5f;
        body.isSun = true;
        body.isStatic = true;
        body.drawOrbitPath = false;
        body.bodyColor = Color.yellow;

        // Rotazione del sole
        PlanetRotation rot = sun.AddComponent<PlanetRotation>();
        rot.rotationSpeed = 3f;
        rot.axialTilt = 7.25f;

        // Materiale luminoso per il sole
        Renderer rend = sun.GetComponent<Renderer>();
        rend.material = CreateMaterial("Sun", Color.yellow, true);

        // Rimuovi collider del sole
        Destroy(sun.GetComponent<Collider>());

        // Luce del sole
        sun.AddComponent<SunLight>();
    }

    void CreatePlanet(string name, float mass, float radius, float distance, Color color, float tilt, float rotSpeed)
    {
        // Angolo casuale per non avere tutti i pianeti in fila
        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        Vector3 position = new Vector3(
            Mathf.Cos(angle) * distance,
            0,
            Mathf.Sin(angle) * distance
        );

        GameObject planet = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        planet.name = name;
        planet.transform.position = position;

        Renderer rend = planet.GetComponent<Renderer>();
        rend.material = CreateMaterial(name, color, false);

        CelestialBody body = planet.AddComponent<CelestialBody>();
        body.mass = mass;
        body.radius = radius;
        body.bodyColor = color;
        body.orbitColor = color;
        body.drawOrbitPath = true;
        body.trailLength = 80f;
        body.autoCalculateVelocity = true;
        body.orbitAround = GameObject.Find("Sun").transform;

        // Rotazione realistica
        PlanetRotation rot = planet.AddComponent<PlanetRotation>();
        rot.rotationSpeed = rotSpeed;
        rot.axialTilt = tilt;

        if (name == "Saturn")
        {
            planet.AddComponent<SaturnRings>();
        }
    }

    void CreateMoon(string name, float mass, float radius, string parentName, float distance, Color color, float tilt, float rotSpeed)
    {
        GameObject parent = GameObject.Find(parentName);
        if (parent == null) return;

        Vector3 position = parent.transform.position + new Vector3(distance, 0, 0);

        GameObject moon = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        moon.name = name;
        moon.transform.position = position;

        Renderer rend = moon.GetComponent<Renderer>();
        rend.material = CreateMaterial(name, color, false);

        CelestialBody body = moon.AddComponent<CelestialBody>();
        body.mass = mass;
        body.radius = radius;
        body.bodyColor = color;
        body.orbitColor = color;
        body.drawOrbitPath = true;
        body.trailLength = 15f;
        body.autoCalculateVelocity = true;
        body.isMoon = true;
        body.orbitAround = parent.transform;

        PlanetRotation rot = moon.AddComponent<PlanetRotation>();
        rot.rotationSpeed = rotSpeed;
        rot.axialTilt = tilt;
    }
}