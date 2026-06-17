using UnityEngine;
using UnityEngine.UI;

public class DirtyMeter : MonoBehaviour
{
    [Header("Configuración de Suciedad")]
    public float currentDirt = 0f;
    public float maxDirt = 100f;

    [Header("UI (Opcional)")]
    public Slider dirtSlider; // Arrastra aquí un Slider de UI si lo tienes

    void Start()
    {
        currentDirt = 0f;
        UpdateUI();
    }

    public void IncreaseDirt(float amount)
    {
        currentDirt += amount;
        currentDirt = Mathf.Clamp(currentDirt, 0f, maxDirt);
        UpdateUI();
    }

    public void DecreaseDirt(float amount)
    {
        currentDirt -= amount;
        currentDirt = Mathf.Clamp(currentDirt, 0f, maxDirt);
        UpdateUI();
    }

    public float GetDirtPercentage()
    {
        return currentDirt / maxDirt;
    }

    private void UpdateUI()
    {
        if (dirtSlider != null)
        {
            dirtSlider.value = currentDirt / maxDirt;
        }
    }
}