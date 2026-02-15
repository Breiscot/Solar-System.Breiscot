using UnityEngine;

public class SolarSystemSetup : MonoBehaviour
{
    void Start()
    {
        CreateSun();

        CreatePlanet("Mercury",     20f,    0.4f,   20f,    new Color(0.7f, 0.7f, 0.7f));
        CreatePlanet("Venus",       80f,    0.9f,   35f,    new Color(0.9f, 0.7f, 0.3f));
        CreatePlanet("Earth",       100f,   1.0f,   50f,    new Color(0.2f, 0.5f, 1.0f));
        CreatePlanet("Mars",        60f,    0.7f,   70f,    new Color(0.9f, 0.3f, 0.2f));
        CreatePlanet("Jupiter",     500f,   3.0f,   110f,   new Color(0.8f, 0.6f, 0.4f));
        CreatePlanet("Saturn",      400f,   2.5f,   150f,   new Color(0.9f, 0.8f, 0.5f));
        CreatePlanet("Uranus",      200f,   1.8f,   200f,   new Color(0.5f, 0.8f, 0.9f));
        CreatePlanet("Neptune",     200f,   1.7f,   250f,   new Color(0.3f, 0.4f, 0.9f));

        CreateMoon("Moon", 10f, 0.3f, "Earth", 5f, new Color(0.8f, 0.8f, 0.8f));
    }

    void CreateSun()
    {
        GameObject sun = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sun.name = "Sun";
        sun.transform.position = Vector3.zero;

        // Corpo celeste
        CelestialBody body = sun.AddComponent<CelestialBody>();
        body.mass = 10000f;
        body.radius = 5f;
        body.drawOrbitPath = false;
        body.bodyColor = Color.yellow;

        // Materiale luminoso per il sole
        Renderer rend = sun.GetComponent<Renderer>();
        Material sunMat = new Material(Shader.Find("Standard"));
        sunMat.color = Color.yellow;
        sunMat.EnableKeyword("_EMISSION");
        sunMat.SetColor("_EmissionColor", Color.yellow * 2f);
        rend.material = sunMat;

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

        CelestialBody body = planet.AddComponent<CelestialBody>();
        body.mass = mass;
        body.radius = radius;
        body.bodyColor = color;
        body.orbitColor = color;
        body.drawOrbitPath = true;
        body.trailLength = 50f;

        // Calcolo automatico della velocità
        body.autoCalculateVelocity = true;
        body.orbitAround = GameObject.Find("Sun").transform;
    }

    void CreateMoon(string name, float mass, float radius, string parentName, float distance, Color color)
    {
        GameObject parent = GameObject.Find(parentName);
        if (parent == null) return;

        Vector3 position = parent.transform.position + new Vector3(distance, 0, 0);

        GameObject moon = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        moon.name = name;
        moon.transform.position = position;

        CelestialBody body = moon.AddComponent<CelestialBody>();
        body.mass = mass;
        body.radius = radius;
        body.bodyColor = color;
        body.orbitColor = color;
        body.drawOrbitPath = true;
        body.trailLength = 10f;

        body.autoCalculateVelocity = true;
        body.orbitAround = parent.transform;
    }
}