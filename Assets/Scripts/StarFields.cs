using UnityEngine;

public class StarFields : MonoBehaviour
{
    [Header("Stelle")]
    public int numberOfStars = 2000;
    public float spawnRadius = 1000f;
    public float starSize = 0.5f;

    void Start()
    {
        CreateStars();
    }

    void CreateStars()
    {
        // Crea un parent per tutte le stelle
        GameObject starsParent = new GameObject("Stars");

        // Crea il materiale per le stelle
        Material starMaterial = new Material(Shader.Find("Sprites/Default"));
        starMaterial.color = Color.white;

        for (int i = 0; i < numberOfStars; i++)
        {
            GameObject star = GameObject.CreatePrimitive(PrimitiveType.Quad);
            star.name = "Star";
            star.transform.parent = starsParent.transform;

            // Rimuove il collider
            Destroy(star.GetComponent<Collider>());

            // Posizione casuale su una sfera
            Vector3 randomDirection = Random.onUnitSphere;
            star.transform.position = randomDirection * spawnRadius;

            // La stella guarda verso il centro
            star.transform.LookAt(Vector3.zero);

            // Dimensione casuale
            float size = Random.Range(starSize * 0.3f, starSize);
            star.transform.localScale = Vector3.one * size;

            // Colore casuale
            Color starColor = GetRandomStarColor();
            Material mat = new Material(starMaterial);
            mat.color = starColor;
            star.GetComponent<Renderer>().material = mat;
        }
    }

    Color GetRandomStarColor()
    {
        float random = Random.Range(0f, 1f);

        if (random < 0.6f)
            return Color.white;
        else if (random < 0.8f)
            return new Color(1f, 1f, 0.8f); // Giallina
        else if (random < 0.9f)
            return new Color(0.8f, 0.9f, 1f); // Azzurrina
        else
            return new Color(1f, 0.8f, 0.7f); // Rossina
    }
}