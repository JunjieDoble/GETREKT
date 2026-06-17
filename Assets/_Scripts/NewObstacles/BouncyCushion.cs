using UnityEngine;

public class BouncyCushion : MonoBehaviour
{
    [Header("Configuración de Rebote")]
    public float bounceForce = 18f;

    [Header("Efecto de Deformación Visual (Escala)")]
    public Transform visualModel; // El modelo visual para aplastarse un poco al rebotar

    private void OnCollisionEnter(Collision collision)
    {
        PlayerMovement player = collision.gameObject.GetComponentInParent<PlayerMovement>();

        if (player != null)
        {
            Rigidbody playerRb = player.GetComponent<Rigidbody>();
            if (playerRb != null)
            {
                // Forzamos que la dirección de rebote apunte siempre hacia arriba del cojín
                Vector3 bounceDirection = transform.up;

                // Limpiar velocidad actual en Y para evitar que caídas altas mitiguen el salto
                Vector3 currentVel = playerRb.linearVelocity;
                currentVel.y = 0;
                playerRb.linearVelocity = currentVel;

                // Aplicar el súper rebote
                playerRb.AddForce(bounceDirection * bounceForce, ForceMode.Impulse);

                // Pequeño feedback visual cosmético si asignaste el modelo
                if (visualModel != null)
                {
                    StopAllCoroutines();
                    StartCoroutine(AnimateCushion());
                }

                Debug.Log("¡Salchicha rebotada por el cojín!");
            }
        }
    }

    System.Collections.IEnumerator AnimateCushion()
    {
        Vector3 originalScale = visualModel.localScale;
        Vector3 squashedScale = new Vector3(originalScale.x * 1.2f, originalScale.y * 0.5f, originalScale.z * 1.2f);

        // Aplastado instantáneo
        visualModel.localScale = squashedScale;

        // Recuperación progresiva
        float elapsed = 0f;
        while (elapsed < 1f)
        {
            elapsed += Time.deltaTime * 4f;
            visualModel.localScale = Vector3.Lerp(squashedScale, originalScale, elapsed);
            yield return null;
        }
        visualModel.localScale = originalScale;
    }
}