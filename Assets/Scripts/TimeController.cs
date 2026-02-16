using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class TimeController : MonoBehaviour
{
    [Header("Velocità Tempo")]
    public float[] timeScales = { 0.1f, 0.25f, 0.5f, 1f, 2f, 5f, 10f, 50f };
    public int currentScaleIndex = 3; // Parte da 1x

    [Header("UI")]
    public TextMeshProUGUI timeText;

    private GravityManager gravityManager;
    private PlanetRotation[] allRotations;

    void Start()
    {
        Invoke("Initialize", 0.6f);
    }

    void Initialize()
    {
        gravityManager = FindObjectOfType<GravityManager>();
        allRotations = FindObjectOfType<PlanetRotation>();

        // Crea UI
        if (timeText == null)
        {
            CreateUI();
        }

        UpdateTimeScale();
    }

    void Update()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null) return;

        // Freccia SU = più veloce
        if (keyboard.upArrowKey.wasPressedThisFrame)
        {
            if (currentScaleIndex < timeScales.Length - 1)
            {
                currentScaleIndex++;
                UpdateTimeScale();
            }
        }

        // Freccia GIU' = più lento
        if (keyboard.downArrowKey.wasPressedThisFrame)
        {
            if (currentScaleIndex > 0)
            {
                currentScaleIndex--;
                UpdateTimeScale();
            }
        }

        // SPAZIO = pausa
        if (keyboard.spaceKey.wasPressedThisFrame)
        {
            if (gravityManager.timeScale > 0)
            {
                gravityManager.timeScale = 0;
                UpdateRotationSpeeds(0);
                UpdateUI("⏸️ PAUSA");
            }
            else
            {
                UpdateTimeScale();
            }
        }
    }

    void UpdateTimeScale()
    {
        float scale = timeScales[currentScaleIndex];

        // Aggiorna la gravità
        if (gravityManager != null)
        {
            gravityManager.timeScale = scale;
        }

        // Aggiorna rotazione dei pianeti
        UpdateRotationSpeeds(scale);

        // Aggiorna la UI
        string speedText = scale + "x";
        UpdateUI("⏩ " + speedText);

        Debug.Log("Velocità tempo: " + speedText);
    }

    void UpdateRotationSpeeds(float scale)
    {
        if (allRotations == null) return;

        foreach (PlanetRotation rot in allRotations)
        {
            if (rot != null)
            {
                rot.currentTimeScale = scale;
            }
        }
    }

    void UpdateUI(string text)
    {
        if (timeText != null)
        {
            timeText.text = text;
        }
    }

    void CreateUI()
    {
        // Crea Canvas
        GameObject canvasObj = new GameObject("TimeCanvas");
        Canvas.canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;
        canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();
    }
}