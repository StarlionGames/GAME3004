using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    Slider s => GetComponent<Slider>();
    
    private void OnEnable()
    {
        PlayerBehaviour.onHealthChanged += ChangeValue;
    }
    private void OnDisable()
    {
        PlayerBehaviour.onHealthChanged -= ChangeValue;
    }

    public void ChangeValue(float value)
    {
        s.value = value;
    }
}
