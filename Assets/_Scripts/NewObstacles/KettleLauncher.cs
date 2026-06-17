using System.Collections;
using UnityEngine;

public class KettleLauncher : MonoBehaviour
{
    [Header("Configuración de Lanzamiento")]
    public Transform launchDirectionReference;
    public float launchForce = 15f;
    public float upwardForce = 8f;

    [Header("Control del Jugador")]
    public float flightStunDuration = 2f;

    [Header("Efectos Visuales")]
    public ParticleSystem steamParticles;

    private void OnTriggerEnter(Collider other)
    {
        PlayerMovement player = other.GetComponentInParent<PlayerMovement>();

        // Comprobamos que el jugador pueda ser controlado antes de lanzarlo
        if (player != null && player.canControl)
        {
            Rigidbody playerRb = player.GetComponent<Rigidbody>();
            if (playerRb != null)
            {
                Vector3 direction = launchDirectionReference != null ?
                    launchDirectionReference.forward : transform.forward;

                direction.y = 0;
                direction.Normalize();

                Vector3 finalVelocity = (direction * launchForce) + (Vector3.up * upwardForce);

                playerRb.linearVelocity = Vector3.zero;
                playerRb.AddForce(finalVelocity, ForceMode.Impulse);

                if (steamParticles != null) steamParticles.Play();

                // Iniciamos la pérdida de control mediante la variable
                StartCoroutine(StunPlayer(player));
            }
        }
    }

    private IEnumerator StunPlayer(PlayerMovement player)
    {
        player.canControl = false; // El jugador ya no puede moverse

        yield return new WaitForSeconds(flightStunDuration);

        if (player != null)
        {
            player.canControl = true; // Recupera el control al aterrizar/pasar el tiempo
        }
    }
}