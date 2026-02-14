using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Movimento")]
    public float zoomSpeed = 100f;
    public float rotationSpeed = 100f;

    [Header("Limiti Zoom")]
    public float minDistance = 5f;
    public float maxDistance = 500f;

    private float currentDistance = 100f;
    private float rotationX = 0f;
    private float rotationY = 45f;

    void Start()
    {
        if (target == null)
        {
            GameObject sun = GameObject.Find("Sun");
            if (sun != null)
            {
                target = sun.transform;
            }
        }
    }

    void Update()
    {
        HandleInput();
        UpdateCameraPosition();
    }

    void HandleInput()
    {
        // Zoom con la rotella del mouse
        float scroll = Input.mouseScrollDelta.y;
        currentDistance -= scroll * zoomSpeed;
        currentDistance = Mathf.Clamp(currentDistance, minDistance, maxDistance);

        // Rotazione con tasto destro del mouse
        if (Input.GetMouseButton(1))
        {
            rotationX += Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
            rotationY -= Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;
            rotationY = Mathf.Clamp(rotationY, 5f, 85f);
        }
       
    }

    void UpdateCameraPosition()
    {
        if (target == null)
        {
            Debug.LogWarning("CameraController: nessun target assegnato.");
            return;
        }

        // Calcola la posizione della camera
        Quaternion rotation = Quaternion.Euler(rotationY, rotationX, 0);
        Vector3 offset = rotation * new Vector3(0, 0, -currentDistance);

        transform.position = target.position + offset;
        transform.LookAt(target);
    }
}
