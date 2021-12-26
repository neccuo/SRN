using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider;
    public Gradient barGradient;
    public Gradient heartGradient;
    public Image fill;
    public Image heart;

    public void SetMaxHealth(float health)
    {
        slider.maxValue = health;
        slider.value = health;

        SetImageColors(1f);
    }

    public void SetHealth(float health)
    {
        slider.value = health;
        SetImageColors(health / slider.maxValue); // ratio
    }

    public void SetImageColors(float value)
    {
        fill.color = barGradient.Evaluate(value);
        heart.color = heartGradient.Evaluate(value);
    }
}
