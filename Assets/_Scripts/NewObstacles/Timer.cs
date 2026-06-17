using UnityEngine;
using TMPro; // Obligatorio para usar TextMeshPro

public class Timer : MonoBehaviour
{
    [Header("Referencias de UI")]
    [Tooltip("Arrastra aquí el componente TextMeshProUGUI de tu Canvas.")]
    public TextMeshProUGUI timerText;

    [Header("Configuración")]
    [Tooltip("¿El cronómetro debe empezar a correr inmediatamente al iniciar la escena?")]
    public bool autoStart = true;

    private float currentTime;
    private bool isRunning = false;

    void Start()
    {
        ResetChronometer();

        if (autoStart)
        {
            StartChronometer();
        }
    }

    void Update()
    {
        if (!isRunning) return;

        // Al ser un cronómetro, sumamos el tiempo que pasa en cada frame
        currentTime += Time.deltaTime;

        UpdateTimerDisplay();
    }

    /// <summary>
    /// Actualiza el texto en el Canvas con el formato exacto min:seg:milisegundos
    /// </summary>
    private void UpdateTimerDisplay()
    {
        if (timerText == null) return;

        // Desglosamos el tiempo matemático
        int minutes = Mathf.FloorToInt(currentTime / 60f);
        int seconds = Mathf.FloorToInt(currentTime % 60f);

        // Multiplicamos por 1000 para obtener los 3 dígitos de los milisegundos (0 a 999)
        int milliseconds = Mathf.FloorToInt((currentTime % 1f) * 1000f);

        // Formato: 00:00:000 (minutos:segundos:milisegundos)
        // Nota: Si prefieres separar los milisegundos con un punto, cambia el último ':' por un '.'
        timerText.text = string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
    }

    // ==========================================
    // MÉTODOS DE CONTROL (Para usar desde otros scripts)
    // ==========================================

    public void StartChronometer()
    {
        isRunning = true;
    }

    public void PauseChronometer()
    {
        isRunning = false;
    }

    public void ResetChronometer()
    {
        currentTime = 0f;
        UpdateTimerDisplay();
    }

    public float GetFinalTime()
    {
        return currentTime;
    }
}