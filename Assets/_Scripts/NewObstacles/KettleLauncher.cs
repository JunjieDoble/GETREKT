using UnityEngine;

public class KettleLauncher : MonoBehaviour
{
    [Header("Configuración de Lanzamiento")]
    [Tooltip("Dirección del lanzamiento. Si se deja vacío, usará el frente (forward) del objeto.")]
    public Transform launchDirectionReference;
    public float launchForce = 15f;

    [Header("Efectos Visuales/Partículas (Opcional)")]
    public ParticleSystem steamParticles;

    private void OnTriggerEnter(Collider other)
    {
        // Verificamos si es el jugador mediante su componente de movimiento
        PlayerMovement player = other.GetComponentInParent<PlayerMovement>();

        if (player != null)
        {
            Rigidbody playerRb = player.GetComponent<Rigidbody>();
            if (playerRb != null)
            {
                // Determinar dirección
                Vector3 direction = launchDirectionReference != null ?
                    launchDirectionReference.forward : transform.forward;

                direction.Normalize();

                // Forzamos un pequeño reseteo de velocidad para que el lanzamiento sea consistente
                playerRb.linearVelocity = Vector3.zero;

                // Aplicamos el impulso físico masivo
                playerRb.AddForce(direction * launchForce, ForceMode.Impulse);

                // Activar partículas de vapor si existen
                if (steamParticles != null)
                {
                    steamParticles.Play();
                }

                Debug.Log("¡Salchicha eyectada por el vapor de la hervidora!");
            }
        }
    }
}