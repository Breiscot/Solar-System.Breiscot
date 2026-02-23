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
        // Luce del sole - illumina tanto
        Light mainLight = gameObject.AddComponent<Light>();
        mainLight.type = LightType.Point;
        mainLight.intensity = 6000f;
        mainLight.range = 400f;
        mainLight.color = new Color(1f, 0.95f, 0.85f);
        mainLight.shadows = LightShadows.None;

        // Luce Media
        GameObject midLightObj = new GameObject("SunMidLight");
        midLightObj.transform.parent = transform;
        midLightObj.transform.localPosition = Vector3.zero;

        Light midLight = midLightObj.AddComponent<Light>();
        midLight.type = LightType.Point;
        midLight.intensity = 3000f;
        midLight.range = 800f;
        midLight.color = new Color(1f, 0.93f, 0.8f);
        midLight.shadows = LightShadows.None;

        // Luce debole
        GameObject farLightObj = new GameObject("SunFarLight");
        farLightObj.transform.parent = transform;
        farLightObj.transform.localPosition = Vector3.zero;
        
        Light farLight = farLightObj.AddComponent<Light>();
        farLight.type = LightType.Point;
        farLight.intensity = 2000f;
        farLight.range = 1500f;
        farLight.color = new Color(1f, 0.9f, 0.7f);
        farLight.shadows = LightShadows.None;
    }
}