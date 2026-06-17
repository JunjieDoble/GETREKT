using System.Collections;
using UnityEngine;

public class Return2Checkpoint : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("Arrastra aquí el objeto que tiene tu script Checkpoint principal.")]
    public Checkpoint checkpointManager;

    [Header("Configuración")]
    [Tooltip("Tiempo en segundos que tarda en reaparecer tras caer/chocar.")]
    public float delaySeconds = 1.5f;

    // Evita que la corrutina se llame múltiples veces si el player rebota en la zona
    private bool isRespawning = false;

    private void OnTriggerEnter(Collider other)
    {
        // Usamos el mismo sistema de Tags que tienes en tu CheckpointCollision
        if (other.CompareTag("Player") && !isRespawning)
        {
            StartCoroutine(RespawnSequence(other.gameObject));
            PlayerMovement player = other.GetComponentInParent<PlayerMovement>();
            
            DirtyMeter dirtTracker = player.GetComponent<DirtyMeter>();
            if (dirtTracker != null)
            {
                dirtTracker.IncreaseDirt(5f);
            }
            Debug.Log("Jugador devuelto al último checkpoint.");
        }
    }

    private IEnumerator RespawnSequence(GameObject playerObj)
    {
        isRespawning = true;

        // 1. Buscamos el script del jugador y le quitamos el control temporalmente
        PlayerMovement movementScript = playerObj.GetComponentInParent<PlayerMovement>();
        if (movementScript != null)
        {
            movementScript.canControl = false;
        }

        // 2. Esperamos los segundos indicados por el diseñador
        yield return new WaitForSeconds(delaySeconds);

        // 3. Teletransportamos de vuelta si hay un manager asignado
        if (checkpointManager != null)
        {
            // Obtenemos la posición segura (ya sea el inicio o el último checkpoint)
            Vector3 respawnPos = checkpointManager.GetCheckpointPosition();

            // Buscamos el Rigidbody para frenarlo en seco
            Rigidbody playerRb = playerObj.GetComponentInParent<Rigidbody>();
            if (playerRb != null)
            {
                playerRb.linearVelocity = Vector3.zero;
                playerRb.angularVelocity = Vector3.zero;
            }

            // Movemos físicamente al jugador al checkpoint
            // NOTA: Si el root de tu player es el que tiene el Rigidbody, movemos el transform del Rigidbody.
            if (playerRb != null)
            {
                playerRb.transform.position = respawnPos;
            }
            else
            {
                playerObj.transform.position = respawnPos;
            }
            
        }
        else
        {
            Debug.LogWarning("¡Falta asignar el script Checkpoint en el inspector de este obstáculo!");
        }

        // 4. Le devolvemos el control al jugador al reaparecer
        if (movementScript != null)
        {
            movementScript.canControl = true;
        }

        isRespawning = false;
    }
}