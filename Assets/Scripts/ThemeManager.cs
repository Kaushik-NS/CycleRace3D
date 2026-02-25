using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ThemeManager : MonoBehaviour
{
    public TMP_Dropdown themeDropdown;

    public Image background;     // full screen bg (optional)
    public Image topPanel;       // Winning meter panel
    public Image middlePanel;    // Bicycle select panel
    public Image bottomPanel;    // Player Name Input panel

    public Color lightPanel = Color.white;
    public Color darkPanel = new Color(0.1f, 0.1f, 0.1f);

    public Camera mainCamera;
    public Color darkBrown;

    void Start()
    {
        themeDropdown.onValueChanged.AddListener(ChangeTheme);

        // Always reset to Light on fresh start
        themeDropdown.value = 0;
        ChangeTheme(0);
    }

    void ChangeTheme(int index)
    {
        bool isLight = index == 0;

        // OPTIONAL UI Background image
        if (background != null)
            background.color = isLight ? Color.white : Color.black;

        // Panels
        if (topPanel != null) topPanel.color = isLight ? lightPanel : darkPanel;
        if (middlePanel != null) middlePanel.color = isLight ? lightPanel : darkPanel;
        if (bottomPanel != null) bottomPanel.color = isLight ? lightPanel : darkPanel;

        //  CAMERA SWITCH
        if (mainCamera == null)
            mainCamera = Camera.main;   // auto find if not assigned

        if (mainCamera != null)
        {
            if (isLight)
            {
                mainCamera.clearFlags = CameraClearFlags.Skybox;
            }
            else
            {
                mainCamera.clearFlags = CameraClearFlags.SolidColor;
                mainCamera.backgroundColor = darkBrown;
            }
        }
    }
}