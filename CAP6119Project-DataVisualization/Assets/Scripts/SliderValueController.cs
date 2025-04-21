using System;
using System.Globalization;
using Unity.VisualScripting;
using UnityEngine;

public class SliderValueController : MonoBehaviour
{
    [SerializeField] public UnityEngine.UI.Slider slider;
    [SerializeField] private TMPro.TMP_Text _maxValueText;
    [SerializeField] private TMPro.TMP_Text _selectedValueText;

    private void Start()
    {
        slider.onValueChanged.AddListener(UpdateSliderText);
    }

    void UpdateSliderText(float value)
    {
        _selectedValueText.text = value.ToString();
    }

    public void UpdateMaxValue(float value)
    {
        slider.maxValue = value;
        _maxValueText.text = value.ToString();
    }
}
