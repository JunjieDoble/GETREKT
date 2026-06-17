using System.Collections;
using UnityEngine;

public class UtensilAttack : MonoBehaviour
{
    [Header("Tiempos del Ciclo")]
    public float idleTime = 4f;
    public float slamSpeed = 15f;
    public float returnSpeed = 2f;

    [Header("Ángulos de Rotación")]
    public Vector3 rotationAxis = Vector3.forward;
    public float restAngle = 0f;
    public float pullBackAngle = -30f;
    public float slamAngle = 60f;

    [Header("Fuerza y Dirección de Impacto")]
    [Tooltip("Fuerza física con la que saldrá volando el player.")]
    public float slamHitForce = 20f;

    [Tooltip("Dirección de empuje LOCAL de la cuchara. Si empuja al revés, cambia la Z a -1.")]
    public Vector3 localHitDirection = new Vector3(0, 0, 1); // Prueba (0,0,-1) si te empuja hacia la pared

    private Quaternion baseRotation;
    private bool isSlammingActive = false;

    void Start()
    {
        baseRotation = transform.localRotation;
        StartCoroutine(SlamCycle());
    }

    IEnumerator SlamCycle()
    {
        while (true)
        {
            yield return new WaitForSeconds(idleTime);

            // 1. Cargar hacia atrás lentamente
            float elapsed = 0f;
            while (elapsed < 1f)
            {
                elapsed += Time.deltaTime * returnSpeed;
                float angle = Mathf.Lerp(restAngle, pullBackAngle, elapsed);
                transform.localRotation = baseRotation * Quaternion.AngleAxis(angle, rotationAxis);
                yield return null;
            }

            yield return new WaitForSeconds(0.3f);

            // 2. ¡GOLPEAR! (Se activa el daño físico por impacto)
            isSlammingActive = true;
            elapsed = 0f;
            while (elapsed < 1f)
            {
                elapsed += Time.deltaTime * slamSpeed;
                float angle = Mathf.Lerp(pullBackAngle, slamAngle, elapsed);
                transform.localRotation = baseRotation * Quaternion.AngleAxis(angle, rotationAxis);
                yield return null;
            }
            isSlammingActive = false; // Termina el golpe violento

            // 3. Regresar al estado de reposo inicial suavemente
            elapsed = 0f;
            while (elapsed < 1f)
            {
                elapsed += Time.deltaTime * returnSpeed;
                float angle = Mathf.Lerp(slamAngle, restAngle, elapsed);
                transform.localRotation = baseRotation * Quaternion.AngleAxis(angle, rotationAxis);
                yield return null;
            }
        }
    }

    // Al colisionar con la salchicha mientras baja con velocidad:
    private void OnCollisionEnter(Collision collision)
    {
        if (!isSlammingActive) return;

        PlayerMovement player = collision.gameObject.GetComponentInParent<PlayerMovement>();
        if (player != null)
        {
            Rigidbody playerRb = player.GetComponent<Rigidbody>();
            if (playerRb != null)
            {
                // Convertimos tu dirección local elegida en una dirección global del mundo real
                Vector3 worldHitDirection = transform.TransformDirection(localHitDirection);

                // Sumamos un poco de altura para que el golpe la eleve ligeramente
                Vector3 launchDir = (worldHitDirection + (Vector3.up * 0.3f)).normalized;

                playerRb.linearVelocity = Vector3.zero;
                playerRb.AddForce(launchDir * slamHitForce, ForceMode.Impulse);
            }
        }
    }


}