using UnityEngine;

public class SolarSystemSetup : MonoBehaviour
{
    void Start()
    {
        CreateSun();

        CreatePlanet("Mercury",     1f,    0.4f,   25f,    new Color(0.7f, 0.7f, 0.7f));
        CreatePlanet("Venus",       3f,    0.9f,   40f,    new Color(0.9f, 0.7f, 0.3f));
        CreatePlanet("Earth",       500f,  1.0f,   55f,    new Color(0.2f, 0.5f, 1.0f));
        CreatePlanet("Mars",        2f,    0.7f,   75f,    new Color(0.9f, 0.3f, 0.2f));
        CreatePlanet("Jupiter",     20f,   3.0f,   120f,   new Color(0.8f, 0.6f, 0.4f));
        CreatePlanet("Saturn",      15f,   2.5f,   170f,   new Color(0.9f, 0.8f, 0.5f));
        CreatePlanet("Uranus",      8f,    1.8f,   230f,   new Color(0.5f, 0.8f, 0.9f));
        CreatePlanet("Neptune",     8f,    1.7f,   280f,   new Color(0.3f, 0.4f, 0.9f));

        CreateMoon("Moon", 0.1f, 0.3f, "Earth", 4f, new Color(0.8f, 0.8f, 0.8f));
    }

    Material CreateMaterial(string planetName, Color fallbackColor, bool emissive)
    {
        Shader shader = Shader.Find("Universal Render Pipeline/Lit");

        if (shader == null)
            shader = Shader.Find("Standard");
        
        if (shader == null)
            shader = Shader.Find("Sprites/Default");

        Material mat = new Material(shader);
        mat.color = color;

        if (mat.HasProperty("_BaseColor"))
            mat.SetColor("_BaseColor", color);

        if (emissive)
        {
            if (mat.HasProperty("_EmissionColor"))
            {
                mat.EnableKeyword("_EMISSION");
                mat.SetColor("_EmissionColor", color * 2f);
            }

            if (mat.HasProperty("_EmissiveColor"))
            {
                mat.SetColor("_EmissiveColor", color * 2f);
            }
        }

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

        // Materiale luminoso per il sole
        Renderer rend = sun.GetComponent<Renderer>();
        rend.material = CreateMaterial(Color.yellow, true);

        // Luce del sole
        Light sunLight = sun.AddComponent<Light>();
        sunLight.type = LightType.Point;
        sunLight.intensity = 2f;
        sunLight.range = 500f;
        sunLight.color = new Color (1f, 0.95f, 0.8f);
    }

    void CreatePlanet(string name, float mass, float radius, float distance, Color color)
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
        rend.material = CreateMaterial(color, false);

        CelestialBody body = planet.AddComponent<CelestialBody>();
        body.mass = mass;
        body.radius = radius;
        body.bodyColor = color;
        body.orbitColor = color;
        body.drawOrbitPath = true;
        body.trailLength = 80f;
        body.autoCalculateVelocity = true;
        body.orbitAround = GameObject.Find("Sun").transform;

        if (name == "Saturn")
        {
            planet.AddComponent<SaturnRings>();
        }
    }

    void CreateMoon(string name, float mass, float radius, string parentName, float distance, Color color)
    {
        GameObject parent = GameObject.Find(parentName);
        if (parent == null) return;

        Vector3 position = parent.transform.position + new Vector3(distance, 0, 0);

        GameObject moon = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        moon.name = name;
        moon.transform.position = position;

        Renderer rend = moon.GetComponent<Renderer>();
        rend.material = CreateMaterial(color, false);

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
    }
}