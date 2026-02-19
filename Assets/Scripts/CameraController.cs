using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [Header("Modalità")]
    public bool freeCamera = false;

    [Header("Target (modalità follow)")]
    public Transform target;

    [Header("Follow Camera")]
    public float followZoomSpeed = 20f;
    public float followRotationSpeed = 0.3f;
    public float minDistance = 5f;
    public float maxDistance = 500f;

    [Header("Free Camera")]
    public float moveSpeed = 50f;
    public float fastMoveSpeed = 150f;
    public float freeLookSpeed = 0.2f;

    // Follow camera
    private float currentDistance = 100f;
    private float rotationX = 0f;
    private float rotationY = 45f;

    // Free camera
    private float yaw = 0f;
    private float pitch = 0f;

    // Lista dei corpi celesti per il cambio target
    private CelestialBody[] allBodies;
    private int currentTargetIndex = 0;

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

        // Aspetta un frame per trovare tutti i corpi
        Invoke("FindAllBodies", 0.1f);
    }

    void FindAllBodies()
    {
        allBodies = FindObjectsOfType<CelestialBody>();
        Debug.Log("Corpi torvati per la camera: " + allBodies.Length);
    }

    void Update()
    {
        HandleModeSwitching();
        HandleTargetSwitching();

        if (freeCamera)
        {
            HandleFreeCamera();
        }
        else
        {
            HandleFollowCamera();
        }
    }

    void HandleModeSwitching()
    {
        // Premere TAB per cambiare modalità
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (keyboard.tabKey.wasPressedThisFrame)
        {
            freeCamera = !freeCamera;

            if (freeCamera)
            {
                // Salva rotazione attuale
                yaw = transform.eulerAngles.y;
                pitch = transform.eulerAngles.x;
                Debug.Log("FREE CAMERA attivata - WASD per muoversi.");
            }
            else
            {
                Debug.Log("FOLLOW CAMERA attivata - segue " + (target != null ? target.name : "nessuno"));
            }
        }
    }

    void HandleTargetSwitching()
    {
        // Solo in modalità follow
        if (freeCamera) return;
        if (allBodies == null || allBodies.Length == 0) return;

        Keyboard keyboard = Keyboard.current;
        if (keyboard == null) return;

        // Premere F per cambiare Target
        if (keyboard.fKey.wasPressedThisFrame)
        {
            currentTargetIndex = (currentTargetIndex + 1) % allBodies.Length;
            target = allBodies[currentTargetIndex].transform;
            Debug.Log("Target: " + target.name);
        }
    }

    void HandleFollowCamera()
    {
        Mouse mouse = Mouse.current;
        if (mouse == null) return;

        // Zoom con rotella
        float scroll = mouse.scroll.ReadValue().y;
        currentDistance -= scroll * followZoomSpeed * Time.deltaTime;
        currentDistance = Mathf.Clamp(currentDistance, minDistance, maxDistance);

        // Rotazione con tasto destro
        if (mouse.rightButton.isPressed)
        {
            Vector2 delta = mouse.delta.ReadValue();
            rotationX += delta.x * followRotationSpeed;
            rotationY -= delta.y * followRotationSpeed;
            rotationY = Mathf.Clamp(rotationY, 5f, 85f);
        }

        // Posiziona la camera
        if (target == null) return;

        Quaternion rotation = Quaternion.Euler(rotationY, rotationX, 0);
        Vector3 offset = rotation * new Vector3(0, 0, -currentDistance);

        transform.position = target.position + offset;
        transform.LookAt(target);
    }

    void HandleFreeCamera()
    {
        Mouse mouse = Mouse.current;
        Keyboard keyboard = Keyboard.current;
        if (mouse == null || keyboard == null) return;

        // Rotazione con tasto destro
        if (mouse.rightButton.isPressed)
        {
            Vector2 delta = mouse.delta.ReadValue();
            yaw += delta.x * freeLookSpeed;
            pitch -= delta.y * freeLookSpeed;
            pitch = Mathf.Clamp(pitch, -89f, 89f);
        }

        transform.rotation = Quaternion.Euler(pitch, yaw, 0);

        // Movimento con WASD
        float speed = moveSpeed;
        if (keyboard.leftShiftKey.isPressed)
        {
            speed = fastMoveSpeed;
        }

        Vector3 move = Vector3.zero;

        if (keyboard.wKey.isPressed) move += transform.forward;
        if (keyboard.sKey.isPressed) move -= transform.forward;
        if (keyboard.aKey.isPressed) move -= transform.right;
        if (keyboard.dKey.isPressed) move += transform.right;
        if (keyboard.eKey.isPressed) move += Vector3.up;
        if (keyboard.qKey.isPressed) move -= Vector3.up;

        transform.position += move.normalized * speed * Time.deltaTime;
    }
}
