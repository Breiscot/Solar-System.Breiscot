using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class ControlsUI : MonoBehaviour
{
    private GameObject panelObj;
    private TextMeshProUGUI controlsText;
    private bool isVisible = true;
    private CameraController cameraController;

    void Start()
    {
        Invoke("Initialize", 0.8f);
    }
    
    void Initialize()
    {
        cameraController = FindObjectOfType<CameraController>();
        CreateUI();
    }

    void Update()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null) return;

        // H per mostrare/nascondere i controlli
        if (keyboard.hKey.wasPressedThisFrame)
        {
            isVisible = !isVisible;
            panelObj.SetActive(isVisible);
        }

        // Aggiorna il testo in base alla modalità camera
        UpdateControlsText();
    }

    void UpdateControlsText()
    {
        if (controlsText == null || cameraController == null) return;

        if (cameraController.freeCamera)
        {
            controlsText.text =
                "<color=#FFD700>FREE CAMERA</color>\n" +
                "\n" +
                "<color=#88CCFF>Movement</color>\n" +
                "W A S D - Move\n" +
                "E - Up\n" +
                "Q - Down\n" +
                "Shift - Fast\n" +
                "Right Click - Look\n" +
                "\n" +
                "<color=#88CCFF>Camera</color>\n" +
                "TAB - Follow Mode\n" +
                "\n" +
                "<color=#88CCFF>Time</color>\n" +
                "↑↓ - Speed\n" +
                "Space - Pause\n" +
                "\n" +
                "<color=#88CCFF>View</color>\n" +
                "G - Spacetime Grid\n" +
                "H - Hide Controls";
        }
        else
        {
            controlsText.text =
                "<color=#FFD700>FOLLOW CAMERA</color>\n" +
                "\n" +
                "<color=#88CCFF>Camera</color>\n" +
                "F - Next Planet\n" +
                "Scroll - Zoom\n" +
                "Right Click - Rotate\n" +
                "TAB - Free Mode\n" +
                "\n" +
                "<color=#88CCFF>Time</color>\n" +
                "↑↓ - Speed\n" +
                "Space - Pause\n" +
                "\n" +
                "<color=#88CCFF>View</color>\n" +
                "G - Spacetime Grid\n" +
                "H - Hide Controls";

        }
    }

    void CreateUI()
    {
        // Cerca se esiste già un Canvas
        Canvas existingCanvas = FindObjectOfType<Canvas>();
        GameObject canvasObj;

        if (existingCanvas != null)
        {
            canvasObj = existingCanvas.gameObject;
        }
        else
        {
            canvasObj = new GameObject("ControlsCanvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 99;
            canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();
            canvasObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();
        }

        // Pannello sfondo a destra
        panelObj = new GameObject("ControlsPanel");
        panelObj.transform.SetParent(canvasObj.transform, false);

        UnityEngine.UI.Image panelImage = panelObj.AddComponent<UnityEngine.UI.Image>();
        panelImage.color = new Color(0, 0, 0, 0.7f);

        RectTransform panelRect = panelObj.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(1f, 0.3f);
        panelRect.anchorMax = new Vector2(1f, 0.9f);
        panelRect.pivot = new Vector2(1f, 0.5f);
        panelRect.sizeDelta = new Vector2(200f, 0f);
        panelRect.anchoredPosition = new Vector2(-10f, 0f);

        // Testo controlli
        GameObject textObj = new GameObject("ControlsText");
        textObj.transform.SetParent(panelObj.transform, false);

        controlsText = textObj.AddComponent<TextMeshProUGUI>();
        controlsText.fontSize = 14;
        controlsText.color = Color.white;
        controlsText.alignment = TextAlignmentOptions.TopLeft;

        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(15f, 15f);
        textRect.offsetMax = new Vector2(-15f, -15f);

        UpdateControlsText();
    }
}