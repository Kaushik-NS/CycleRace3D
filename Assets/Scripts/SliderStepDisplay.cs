using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SliderStepDisplay : MonoBehaviour
{
    public Slider slider;
    public TMP_Text valueText;

    int step = 50;

    void Start()
    {
        slider.onValueChanged.AddListener(UpdateValue);
        UpdateValue(slider.value);
    }

    void UpdateValue(float val)
    {
        // Snap to nearest 50
        int steppedValue = Mathf.RoundToInt(val / step) * step;

        // Clamp between 50 and 500
        steppedValue = Mathf.Clamp(steppedValue, 50, 500);

        // Assign snapped value back to slider
        slider.SetValueWithoutNotify(steppedValue);

        // Update text
        valueText.text = steppedValue.ToString();
    }
}