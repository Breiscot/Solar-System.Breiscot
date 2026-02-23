using UnityEngine;

public class SunLight : MonoBehaviour
{
    void Start()
    {
        // Rimuovi eventuali luci esistenti
        Light existingLight = GetComponent<Light>();
        if (existingLight != null)
        {
            Destroy(existingLight);
        }

        // Luce direzionale principale
        GameObject dirLightObj = new GameObject("SunDirectionalLight");
        dirLightObj.transform.parent = transform;
        dirLightObj.transform.localPosition = Vector3.zero;

        Light dirLight = dirLightObj.AddComponent<Light>();
        dirLight.type = LightType.Directional;
        dirLight.intensity = 1.5f;
        dirLight.color = new Color(1f, 0.95f, 0.85f);
        dirLight.shadows = LightShadows.Soft;
        dirLight.shadowStrength = 0.8f;

        // Luce point per illuminare tutti i lati
        Light pointLight = gameObject.AddComponent<Light>();
        pointLight.type = LightType.Point;
        pointLight.intensity = 5f;
        pointLight.range = 800f;
        pointLight.color = new Color(1f, 0.95f, 0.85f);
        pointLight.shadows = LightShadows.None;

        // Luce lontana debole
        GameObject farLightObj = new GameObject("SunFarLight");
        farLightObj.transform.parent = transform;
        farLightObj.transform.localPosition = Vector3.zero;

        Light farLight = farLightObj.AddComponent<Light>();
        farLight.type = LightType.Point;
        farLight.intensity = 2f;
        farLight.range = 1500f;
        farLight.color = new Color(1f, 0.9f, 0.7f);
        farLight.shadows = LightShadows.None;

        // Aggiorna la direzione della luce direzionale
        StartCoroutine(UpdateDirectionalLight(dirLightObj.transform));
    }

    System.Collections.IEnumerator UpdateDirectionalLight(Transform dirLight)
    {
        while (true)
        {
            dirLight.rotation = Quaternion.Euler(30f, 0f, 0f);
            yield return new WaitForSeconds(0.5f);
        }
    }
}