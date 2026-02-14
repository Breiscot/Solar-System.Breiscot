using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Movimento")]
    public float moveSpeed = 50f;
    public float zoomSpeed = 100f;
    public float rotationSpeed = 100f;

    [Header("Limiti Zoom")]
    public float minDistance = 5f;
    public float maxDistance = 500f;

    private float currentDistance = 100f;
    private float rotationX = 0f;
    private float rotationY = 45f;

    void Update()
    {
        HandleInput();
        UpdateCameraPosition();
    }

    void HandleInput()
    {
        // Zoom con la rotella del mouse
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        currentDistance -= scroll * zoomSpeed;
        currentDistance = Mathf.Clamp(currentDistance, minDistance, maxDistance);

        // Rotazione con tasto destro del mouse
        if (Input.GetMouseButton(1))
        {
            rotationX += Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
            rotationY -= Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;
            rotationY = Mathf.Clamp(rotationY, 5f, 85f);
        }

        // Cambio target numerici (1-9)
        for (int i = 1; 1 <= 9; i++)
        {
            if (Input.GetKeyDown(i.ToString()))
            {
                CelestialBody[] bodies = FindObjectsOfType<CelestialBody>();
                if (i - 1 < bodies.Length)
                {
                    target = bodies[i - 1].transform;
                }
            }
        }
    }

    void UpdateCameraPosition()
    {
        if (target == null) return;

        // Calcola la posizione della camera
        Quaternion rotation = Quaternion.Euler(rotationY, rotationX, 0);
        Vector3 offset = rotation * new Vector3(0, 0, -currentDistance);

        transform.position = target.position + offset;
        transform.LookAt(target);
    }
}
