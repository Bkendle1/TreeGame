using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private Image fill;

    public void SetMaxStamina(int stamina)
    {
        slider.maxValue = stamina;
        slider.value = stamina;
    }
    
    public void DecreaseStamina(int rateValue)
    {
        slider.value -= rateValue;
    }

    public void IncreaseStamina(int rateValue)
    {
        slider.value += rateValue;
    }

}
