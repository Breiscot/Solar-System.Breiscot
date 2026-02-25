using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class TimeController : MonoBehaviour
{
    [Header("Velocità Tempo")]
    public float[] timeScales = { 0.1f, 0.25f, 0.5f, 1f, 2f, 5f, 10f, 50f };
    public int currentScaleIndex = 3; // Parte da 1x

    private GravityManager gravityManager;
    private PlanetRotation[] allRotations;
    private CameraController cameraController;
    private TextMeshProUGUI timeText;
    private bool isPaused = false;

    void Start()
    {
        Invoke("Initialize", 0.7f);
    }

    void Initialize()
    {
        gravityManager = FindObjectOfType<GravityManager>();
        allRotations = FindObjectsOfType<PlanetRotation>();
        cameraController = FindObjectOfType<CameraController>();
        CreateUI();
        UpdateTimeScale();
    }

    void Update()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null) return;

        // Freccia SU = più veloce
        if (keyboard.upArrowKey.wasPressedThisFrame)
        {
            if (isPaused) isPaused = false;
            if (currentScaleIndex < timeScales.Length - 1)
            {
                currentScaleIndex++;
            }
            UpdateTimeScale();
        }

        // Freccia GIU' = più lento
        if (keyboard.downArrowKey.wasPressedThisFrame)
        {
            if (isPaused) isPaused = false;
            if (currentScaleIndex > 0)
            {
                currentScaleIndex--;
            }
            UpdateTimeScale();
        }

        // SPAZIO = pausa
        if (keyboard.spaceKey.wasPressedThisFrame)
        {
            isPaused = !isPaused;

            if (isPaused)
            {
                if (gravityManager != null)
                    gravityManager.timeScale = 0;
                UpdateRotationSpeeds(0);
                UpdateUI();
            }
            else
            {
                UpdateTimeScale();
            }
        }

        // Aggiorna UI ogni frame per mostrare velocità camera
        if (cameraController != null && cameraController.freeCamera)
        {
            UpdateUI();
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
        UpdateUI();
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

    void UpdateUI()
    {
        if (timeText == null) return;

        string timeStr;
        if (isPaused)
        {
            timeStr = "⏸ Paused";
        }
        else
        {
            timeStr = "⏩ " + timeScales[currentScaleIndex] + "x";
        }

        // Mostra velocità camera se in free mode
        if (cameraController != null && cameraController.freeCamera)
        {
            timeStr += " | Speed: " + Mathf.Round(cameraController.GetCurrentSpeed());
        }

        timeText.text = timeStr;
    }

    void CreateUI()
    {
        // Crea Canvas esistente
        Canvas existingCanvas = FindObjectOfType<Canvas>();
        GameObject canvasObj;

        if (existingCanvas != null)
        {
            canvasObj = existingCanvas.gameObject;
        }
        else
        {
            canvasObj = new GameObject("UICanvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;
            canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();
        }

        // Pannello in alto
        GameObject panel = new GameObject("TimePanel");
        panel.transform.SetParent(canvasObj.transform, false);

        UnityEngine.UI.Image panelImage = panel.AddComponent<UnityEngine.UI.Image>();
        panelImage.color = new Color(0, 0, 0, 0.6f);

        RectTransform panelRect = panel.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.3f, 0.92f);
        panelRect.anchorMax = new Vector2(0.7f, 1f);
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        // Tasto
        GameObject textObj = new GameObject("TimeText");
        textObj.transform.SetParent(panel.transform, false);

        timeText = textObj.AddComponent<TextMeshProUGUI>();
        timeText.text = "⏩ 1x";
        timeText.fontSize = 22;
        timeText.color = Color.white;
        timeText.alignment = TextAlignmentOptions.Center;

        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
    }
}