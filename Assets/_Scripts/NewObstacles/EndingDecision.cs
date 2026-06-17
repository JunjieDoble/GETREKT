using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingDecision : MonoBehaviour
{
    [Header("Umbrales")]
    [Range(0f, 100f)] public float maxDirtAllowedToWin = 50f;

    [Header("Referencias de Fallo (Basura)")]
    public Transform trashCanTarget; // Destino o dirección donde está la basura
    public float rejectionImpulseForce = 14f;

    private void OnTriggerEnter(Collider other)
    {
        PlayerMovement player = other.GetComponentInParent<PlayerMovement>();
        if (player != null)
        {
            DirtyMeter dirtTracker = player.GetComponent<DirtyMeter>();
            if (dirtTracker != null)
            {
                Debug.Log($"¡Salchicha llegó a la parrilla! Nivel de suciedad: {dirtTracker.currentDirt}");

                if (dirtTracker.currentDirt <= maxDirtAllowedToWin)
                {
                    WinLevel();
                }
                else
                {
                    FailAndThrowToTrash(player.GetComponent<Rigidbody>());
                }
            }
        }
    }

    void WinLevel()
    {
        Debug.Log("¡VICTORIA! La salchicha está deliciosa, limpia y lista para ser comida en la parrilla.");
        // Aquí puedes activar tu panel de victoria UI o cargar el siguiente nivel:
        // SceneManager.LoadScene("WinScene");
    }

    void FailAndThrowToTrash(Rigidbody playerRb)
    {
        if (playerRb == null) return;

        Debug.Log("¡Asco! Salchicha demasiado sucia para la parrilla. ¡A la basura!");

        // Desactivamos momentáneamente el control del jugador para que no contrarreste el lanzamiento
        PlayerMovement movementScript = playerRb.GetComponent<PlayerMovement>();
        if (movementScript != null) movementScript.enabled = false;

        // Calcular dirección hacia el tacho de basura
        Vector3 throwDirection;
        if (trashCanTarget != null)
        {
            throwDirection = (trashCanTarget.position - playerRb.transform.position).normalized;
            // Le damos una parábola hacia arriba en el lanzamiento
            throwDirection.y += 0.5f;
        }
        else
        {
            // Dirección por defecto (atrás y arriba) si no pones un Transform objetivo
            throwDirection = (-transform.forward + Vector3.up).normalized;
        }

        // Aplicamos el empuje físico de rechazo
        playerRb.linearVelocity = Vector3.zero;
        playerRb.AddForce(throwDirection.normalized * rejectionImpulseForce, ForceMode.Impulse);

        // Opcional: Podrías reiniciar el nivel tras 2 segundos llamando a una corrutina.
    }
}