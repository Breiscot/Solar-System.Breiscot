using UnityEngine;

public class PlanetRotation : MonoBehaviour
{
    [Header("Rotazione")]
    public float rotationSpeed = 50f;
    public float axialTilt = 0f;

    [HideInInspector]
    public float currentTimeScale = 1f;

    private Vector3 rotationAxis;

    void Start()
    {
        // Calcola l'asse di rotazione inclinato
        rotationAxis = Quaternion.Euler(0, 0, axialTilt) * Vector3.up;
    }

    void Update()
    {
        float speed = rotationSpeed * currentTimeScale;
        transform.Rotate(Vector3.up, speed * Time.deltaTime, Space.Self);
    }
}