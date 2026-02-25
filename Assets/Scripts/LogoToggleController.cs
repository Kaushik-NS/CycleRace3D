using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LogoToggleController : MonoBehaviour
{
    public Toggle toggle;

    // Parent that contains image + both texts
    public RectTransform logoGroup;

    public TMP_Dropdown alignmentDropdown;

    void Start()
    {
        toggle.onValueChanged.AddListener(OnToggleChanged);
        alignmentDropdown.onValueChanged.AddListener(SetAlignment);

        // Apply initial state
        OnToggleChanged(toggle.isOn);
        SetAlignment(alignmentDropdown.value);
    }

    void OnToggleChanged(bool isOn)
    {
        if (logoGroup != null)
            logoGroup.gameObject.SetActive(isOn);
    }

    void SetAlignment(int index)
    {
        if (logoGroup == null) return;

        RectTransform parent = logoGroup.parent as RectTransform;
        if (parent == null) return;

        float halfParent = parent.rect.width / 2f;
        float halfLogo = logoGroup.rect.width / 2f;

        Vector2 pos = logoGroup.anchoredPosition;

        if (index == 0) // LEFT
        {
            pos.x = -halfParent + halfLogo + 20f;   // 20 = margin
        }
        else if (index == 1) // CENTER
        {
            pos.x = 0f;
        }
        else // RIGHT
        {
            pos.x = halfParent - halfLogo - 20f;
        }

        logoGroup.anchoredPosition = pos;
    }
}