using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private Gradient gradient;
    [SerializeField] private Image fill;
    
    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;

        //set health fill color to the color of the gradient
        //1f is the color of the gradient all the way to the right which is green in this case
        fill.color = gradient.Evaluate(1f);
    }
    
    public void SetHealth(int health)
    {
        //set the slider to the given health 
        slider.value = health;

        fill.color = gradient.Evaluate(slider.normalizedValue);
    }
    
}
