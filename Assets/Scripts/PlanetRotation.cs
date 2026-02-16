using UnityEngine;

public class PlanetRotation : MonoBehaviour
{
    public float rotationSpeed = 50f;

    [HideInInspector]
    public float currentTimeScale = 1f;

    void Update()
    {
        float speed = rotationSpeed * currentTimeScale;
        transform.Rotate(Vector3.up, speed * Time.deltaTime, Space.Self);
    }
}