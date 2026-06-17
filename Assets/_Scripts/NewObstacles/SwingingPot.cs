using UnityEngine;

public class SwingingPot : MonoBehaviour
{
    [Header("Configuración de Balanceo")]
    public Vector3 swingAxis = Vector3.right; // Eje X por defecto
    public float maxAngle = 45f;
    public float speed = 2f;

    [Header("Reacción al Jugador")]
    public float playerImpactSensitivity = 0.5f;

    private float currentTimer = 0f;
    private float extraSwingOffset = 0f;
    private Quaternion baseRotation;

    void Start()
    {
        baseRotation = transform.localRotation;
    }

    void Update()
    {
        // Ciclo de balanceo natural basado en el tiempo
        currentTimer += Time.deltaTime * speed;

        // Reducimos gradualmente el impacto extra recibido por el jugador
        extraSwingOffset = Mathf.MoveTowards(extraSwingOffset, 0f, Time.deltaTime * 5f);

        float angle = Mathf.Sin(currentTimer) * (maxAngle + extraSwingOffset);

        // Aplicamos la rotación local en el eje correcto
        transform.localRotation = baseRotation * Quaternion.AngleAxis(angle, swingAxis);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Si el player choca a gran velocidad, la olla reacciona balanceándose más fuerte
        PlayerMovement player = collision.gameObject.GetComponentInParent<PlayerMovement>();
        if (player != null)
        {
            Rigidbody playerRb = collision.rigidbody;
            if (playerRb != null)
            {
                float impactForce = playerRb.linearVelocity.magnitude;
                extraSwingOffset = impactForce * playerImpactSensitivity;
            }
        }
    }
}