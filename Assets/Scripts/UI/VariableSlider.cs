using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VariableSlider : MonoBehaviour
{
    [SerializeField]private Slider slider;
    [SerializeField]private TMP_InputField inputField;
    [SerializeField]private int accuracy;

    private void Start()
    {
        slider = transform.GetChild(1).GetComponent<Slider>();
        inputField = transform.GetChild(2).GetComponent<TMP_InputField>();
    }

    public void UpdateInputField(float value)
    {
        if(accuracy != 0)
        {
            inputField.text = Math.Round(value, accuracy).ToString();
        }
        else
        {
            inputField.text = Mathf.Round(value).ToString("0");
        }
    }

    public void UpdateSlider(string value)
    {
        float parsedValue = float.Parse(value);

        if(parsedValue < slider.minValue)
        {
            parsedValue = slider.minValue;
            UpdateInputField(parsedValue);
        }
        
        if(parsedValue > slider.maxValue)
        {
            parsedValue = slider.maxValue;
            UpdateInputField(parsedValue);
        }
        
        slider.value = parsedValue;
    }
}
