using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinalLevelManager : MonoBehaviour
{
    [Header("Referencias de UI")]
    public TextMeshProUGUI checkpointText;

    [Header("Referencias de Escena")]
    public Transform sartenFinal;

    [Header("Umbrales de Suciedad")]
    [Range(0f, 100f)] public float maxDirtAllowedToWin = 50f;

    [Header("Configuración de Victoria (Sartén Eject)")]
    public float rotationAngle = -90f;
    public float rotationSpeed = 50f;
    public float ejectDelay = 1f;
    public float winForwardForce = 25f;
    public float winUpwardForce = 15f;

    [Header("Configuración de Fallo (Basura)")]
    public Transform trashCanTarget; // Destino o dirección donde está la basura
    public float rejectionImpulseForce = 14f;
    public float failUpwardModifier = 0.5f;

    private Quaternion targetRotation;
    private bool levelEnded = false;

    private void Start()
    {
        if (sartenFinal != null)
        {
            targetRotation = sartenFinal.localRotation * Quaternion.Euler(0f, rotationAngle, 0f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Si el nivel ya se decidió, ignoramos nuevas colisiones
        if (levelEnded) return;

        // Buscamos el componente PlayerMovement en el objeto o sus padres
        PlayerMovement player = other.GetComponentInParent<PlayerMovement>();

        if (player != null && other.CompareTag("Player"))
        {
            levelEnded = true;

            // Bloqueamos los controles del jugador usando la variable canControl que creamos
            player.canControl = false;

            // Buscamos su medidor de suciedad (DirtyMeter)
            DirtyMeter dirtTracker = player.GetComponent<DirtyMeter>();
            Rigidbody playerRb = player.GetComponent<Rigidbody>();

            if (dirtTracker != null && playerRb != null)
            {
                Debug.Log($"¡Salchicha llegó al final! Nivel de suciedad: {dirtTracker.currentDirt}");

                if (dirtTracker.currentDirt <= maxDirtAllowedToWin)
                {
                    // CASO 1: VICTORIA -> Secuencia de la sartén
                    StartCoroutine(WinSequence(player, playerRb));
                }
                else
                {
                    // CASO 2: FALLO -> Directo a la basura
                    StartCoroutine(FailSequence(player, playerRb));
                }
            }
        }
    }

    // =================================================================
    // SECUENCIA DE VICTORIA (Tu lógica de la sartén corregida)
    // =================================================================
    private IEnumerator WinSequence(PlayerMovement player, Rigidbody playerRb)
    {
        if (checkpointText != null)
        {
            checkpointText.SetText("OMG a delicious sausage! Down the hatch!!");
            StartCoroutine(HideTextCoroutine());
        }

        // Esperar antes de que la sartén empiece a girar/eyectar
        yield return new WaitForSeconds(ejectDelay);

        if (sartenFinal != null)
        {
            while (Quaternion.Angle(sartenFinal.localRotation, targetRotation) > 0.1f)
            {
                sartenFinal.localRotation = Quaternion.RotateTowards(
                    sartenFinal.localRotation,
                    targetRotation,
                    rotationSpeed * Time.deltaTime
                );
                yield return null;
            }
            sartenFinal.localRotation = targetRotation;
        }

        // Aseguramos que el Rigidbody no tenga velocidades remanentes antes del gran salto
        playerRb.linearVelocity = Vector3.zero;
        playerRb.angularVelocity = Vector3.zero;

        // Lanzamiento de victoria utilizando la dirección de la sartén
        Vector3 winLaunchForce = (sartenFinal.forward * winForwardForce) + (Vector3.up * winUpwardForce);
        playerRb.AddForce(winLaunchForce, ForceMode.Impulse);

        Debug.Log("¡VICTORIA! Eyectado hacia la gloria.");

        // Opcional: Cargar escena de victoria tras unos segundos de vuelo
        // yield return new WaitForSeconds(3f);
        // SceneManager.LoadScene("WinScene");
    }

    // =================================================================
    // SECUENCIA DE FALLO (Lanzamiento a la basura corregido)
    // =================================================================
    private IEnumerator FailSequence(PlayerMovement player, Rigidbody playerRb)
    {
        if (checkpointText != null)
        {
            checkpointText.SetText("EWWW YOU'RE DIRTY, out my sight!");
            StartCoroutine(HideTextCoroutine());
        }

        // Pequeño delay dramático antes de ser rechazado de la parrilla
        yield return new WaitForSeconds(0.5f);


        // Calcular dirección exacta hacia el tacho de basura
        Vector3 throwDirection;
        if (trashCanTarget != null)
        {
            throwDirection = (trashCanTarget.position - playerRb.transform.position).normalized;
            throwDirection.y += failUpwardModifier; // Parábola ajustable desde el inspector
        }
        else
        {
            // Dirección por defecto (atrás y arriba respecto a este trigger) si no pones un Transform objetivo
            throwDirection = (-transform.forward + Vector3.up).normalized;
        }

        // Reset de velocidades para garantizar un impulso limpio y matemático
        playerRb.linearVelocity = Vector3.zero;
        playerRb.angularVelocity = Vector3.zero;

        // Aplicamos el empuje físico de rechazo hacia la basura
        playerRb.AddForce(throwDirection.normalized * rejectionImpulseForce, ForceMode.Impulse);

        // Reiniciar el nivel o devolver al último checkpoint tras 3 segundos en el basurero
        yield return new WaitForSeconds(3f);

        // Opción A: Reiniciar la escena actual si pierde completamente
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        // Opción B: Si prefieres que vuelva a un checkpoint en vez de reiniciar el nivel entero,
        // puedes buscar tu script de checkpoints aquí y teletransportarlo.
    }

    private IEnumerator HideTextCoroutine()
    {
        yield return new WaitForSeconds(2.5f);
        if (checkpointText != null)
        {
            checkpointText.SetText("");
        }
    }
}