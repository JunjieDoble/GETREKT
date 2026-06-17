using System.Collections.Generic;
using UnityEngine;

public class SwingingPot : MonoBehaviour
{
    [Header("Configuración de Balanceo")]
    public Vector3 swingAxis = Vector3.right;
    public float maxAngle = 45f;
    public float speed = 2f;

    [Header("Reacción al Jugador")]
    public float playerImpactSensitivity = 0.5f;

    private float currentTimer = 0f;
    private float extraSwingOffset = 0f;
    private Quaternion baseRotation;

    // Rastrear la salchicha encima para moverla sin emparentar
    private List<Rigidbody> passengers = new List<Rigidbody>();
    private Dictionary<Rigidbody, Vector3> lastLocalPositions = new Dictionary<Rigidbody, Vector3>();

    void Start()
    {
        baseRotation = transform.localRotation;
    }

    void Update()
    {
        currentTimer += Time.deltaTime * speed;
        extraSwingOffset = Mathf.MoveTowards(extraSwingOffset, 0f, Time.deltaTime * 5f);

        float angle = Mathf.Sin(currentTimer) * (maxAngle + extraSwingOffset);
        transform.localRotation = baseRotation * Quaternion.AngleAxis(angle, swingAxis);
    }

    void FixedUpdate()
    {
        // Mover a la salchicha sincronizadamente con la rotación de la olla
        for (int i = passengers.Count - 1; i >= 0; i--)
        {
            Rigidbody rb = passengers[i];
            if (rb != null && lastLocalPositions.ContainsKey(rb))
            {
                // Convertimos la posición local guardada del frame anterior a la nueva posición global modificada por la olla
                Vector3 currentWorldPosFromLocal = transform.TransformPoint(lastLocalPositions[rb]);
                Vector3 deltaMovement = currentWorldPosFromLocal - rb.position;

                // Movemos el Rigidbody de la salchicha anulando el freno estático de su FixedUpdate
                rb.position += deltaMovement;

                // Actualizamos para el siguiente frame
                lastLocalPositions[rb] = transform.InverseTransformPoint(rb.position);
            }
            else
            {
                passengers.RemoveAt(i);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        PlayerMovement player = collision.gameObject.GetComponentInParent<PlayerMovement>();
        if (player != null)
        {
            Rigidbody playerRb = player.GetComponent<Rigidbody>();
            if (playerRb != null)
            {
                float impactForce = playerRb.linearVelocity.magnitude;
                extraSwingOffset = impactForce * playerImpactSensitivity;

                if (!passengers.Contains(playerRb))
                {
                    passengers.Add(playerRb);
                    lastLocalPositions[playerRb] = transform.InverseTransformPoint(playerRb.position);
                }
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        Rigidbody playerRb = collision.gameObject.GetComponentInParent<Rigidbody>();
        if (playerRb != null && passengers.Contains(playerRb))
        {
            passengers.Remove(playerRb);
            lastLocalPositions.Remove(playerRb);
        }
    }
}