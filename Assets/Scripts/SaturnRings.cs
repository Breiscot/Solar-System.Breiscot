using UnityEngine;

public class SaturnRings : MonoBehaviour
{
    public float innerRadius = 3.5f;
    public float outerRadius = 5.5f;
    public int segments = 100;
    public Color ringColor = new Color(0.9f, 0.8f, 0.5f, 0.5f);

    void Start()
    {
        CreateRing();
    }

    void CreateRing()
    {
        // 3 Anelli con diverse dimensioni
        for (int ring = 0; ring < 3; ring++)
        {
            float inner = innerRadius + ring * 0.6f;
            float outer = inner + 0.4f;

            GameObject ringObj = new GameObject("Ring_" + ring);
            ringObj.transform.parent = transform;
            ringObj.transform.localPosition = Vector3.zero;

            LineRenderer line = ringObj.AddComponent<LineRenderer>();
            line.positionCount = segments + 1;
            line.loop = true;
            line.startWidth = 0.3f;
            line.endWidth = 0.3f;
            line.useWorldSpace = false;

            Material mat = new Material(Shader.Find("Sprites/Default"));
            mat.color = ringColor;
            line.material = mat;
            line.startColor = ringColor;
            line.endColor = ringColor;

            float radius = (inner + outer) / 2f;

            for (int i = 0; i <= segments; i++)
            {
                float angle = (float)i / segments * 360f * Mathf.Deg2Rad;
                float x = Mathf.Cos(angle) * radius;
                float z = Mathf.Sin(angle) * radius;
                line.SetPosition(i, new Vector3(x, 0, z));
            }
        }
    }
}