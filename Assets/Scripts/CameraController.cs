using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [Header("Modalità")]
    public bool freeCamera = false;

    [Header("Target")]
    public Transform target;

    [Header("Follow Camera")]
    public float followRotationSpeed = 0.3f;
    public float minDistance = 2f;
    public float maxDistance = 800f;

    [Header("Free Camera")]
    public float moveSpeed = 50f;
    public float minMoveSpeed = 10f;
    public float maxMoveSpeed = 500f;
    public float speedScrollSensitivity = 20f;
    public float freeLookSpeed = 0.2f;

    // Private
    private float currentDistance = 100f;
    private float rotationX = 0f;
    private float rotationY = 45f;
    private float yaw = 0f;
    private float pitch = 0f;
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
        Invoke("FindAllBodies", 0.6f);
    }

    void FindAllBodies()
    {
        allBodies = FindObjectsOfType<CelestialBody>();
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

            // Adatta la distanza alla dimensione del pianeta
            CelestialBody body = target.GetComponent<CelestialBody>();
            if (body != null)
            {
                currentDistance = body.radius * 8f;
            }
        }
    }

    void HandleFollowCamera()
    {
        Mouse mouse = Mouse.current;
        if (mouse == null) return;

        // Zoom con rotella
        float scroll = mouse.scroll.ReadValue().y;
        if (scroll != 0)
        {
            // Zoom proporzionale alla distanza attuale
            float zoomAmount = currentDistance * 0.15f;
            currentDistance -= scroll * zoomAmount;
            currentDistance = Mathf.Clamp(currentDistance, minDistance, maxDistance);
        }

        // Rotazione con tasto destro
        if (mouse.rightButton.isPressed)
        {
            Vector2 delta = mouse.delta.ReadValue();
            rotationX += delta.x * followRotationSpeed;
            rotationY -= delta.y * followRotationSpeed;
            rotationY = Mathf.Clamp(rotationY, -89f, 89f);
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

        // Rotella cambia velocità
        float scroll = mouse.scroll.ReadValue().y;
        if (scroll != 0)
        {
            moveSpeed += scroll * speedScrollSensitivity;
            moveSpeed = Mathf.Clamp(moveSpeed, minMoveSpeed, maxMoveSpeed);
        }

        // Rotazione con tasto destro
        if (mouse.rightButton.isPressed)
        {
            Vector2 delta = mouse.delta.ReadValue();
            yaw += delta.x * freeLookSpeed;
            pitch -= delta.y * freeLookSpeed;
            pitch = Mathf.Clamp(pitch, -89f, 89f);
        }

        transform.rotation = Quaternion.Euler(pitch, yaw, 0);

        // Velocità base
        float speed = moveSpeed;
        if (keyboard.leftShiftKey.isPressed)
        {
            speed *= 3f;
        }

        // Movimento
        Vector3 move = Vector3.zero;

        if (keyboard.wKey.isPressed) move += transform.forward;
        if (keyboard.sKey.isPressed) move -= transform.forward;
        if (keyboard.aKey.isPressed) move -= transform.right;
        if (keyboard.dKey.isPressed) move += transform.right;
        if (keyboard.eKey.isPressed) move += Vector3.up;
        if (keyboard.qKey.isPressed) move -= Vector3.up;

        transform.position += move.normalized * speed * Time.deltaTime;
    }

    // Per l'UI - mostra la velocità attuale
    public float GetCurrentSpeed()
    {
        return moveSpeed;
    }
}
