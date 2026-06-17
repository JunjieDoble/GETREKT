using System.Collections;
using UnityEngine;
using TMPro; // Es obligatorio incluir este namespace para usar TextMeshPro

public class MicrowaveInteraction : MonoBehaviour
{
    [Header("Referencias de UI")]
    [Tooltip("Arrastra aquí el componente TextMeshPro que está dentro de tu Canvas.")]
    public TextMeshProUGUI dialogueText;

    [Header("Configuración del Diálogo")]
    [Tooltip("El tiempo de espera entre cada línea de texto.")]
    public float delayBetweenLines = 2.5f;

    [Tooltip("Si está marcado, este trigger solo se activará la primera vez que la salchicha lo toque.")]
    public bool triggerOnlyOnce = true;

    [Header("Líneas de Texto")]
    [TextArea(2, 5)] public string line1 = "¡Vaya, esa hervidora se ve peligrosa!";
    [TextArea(2, 5)] public string line2 = "Si calculo bien el salto del vapor...";
    [TextArea(2, 5)] public string line3 = "¡Podré llegar a las estanterías de arriba!";

    private bool hasBeenTriggered = false;
    private Coroutine dialogueCoroutine;

    private void Start()
    {
        // Nos aseguramos de que el texto empiece vacío al iniciar el nivel
        if (dialogueText != null)
        {
            dialogueText.text = "";
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Si ya se activó y estaba configurado para un solo uso, no hace nada
        if (triggerOnlyOnce && hasBeenTriggered) return;

        // Comprobamos si el objeto que entra tiene el script de movimiento de tu salchicha
        PlayerMovement player = other.GetComponentInParent<PlayerMovement>();

        if (player != null && dialogueText != null)
        {
            hasBeenTriggered = true;

            // Si por alguna razón ya había otro diálogo corriendo en este mismo script, lo detenemos primero
            if (dialogueCoroutine != null)
            {
                StopCoroutine(dialogueCoroutine);
            }

            // Iniciamos la secuencia de las 3 líneas
            dialogueCoroutine = StartCoroutine(PlayDialogueSequence());
        }
    }

    private IEnumerator PlayDialogueSequence()
    {
        // Línea 1
        dialogueText.text = line1;
        yield return new WaitForSeconds(delayBetweenLines);

        // Línea 2
        dialogueText.text = line2;
        yield return new WaitForSeconds(delayBetweenLines);

        // Línea 3
        dialogueText.text = line3;
        yield return new WaitForSeconds(delayBetweenLines);

        // Desaparece el texto al terminar la secuencia
        dialogueText.text = "";
        dialogueCoroutine = null;
    }
}