using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParaMasa : MonoBehaviour
{
    [Header("Ruta de Patrulla (Posición y Rotación Fija)")]
    [Tooltip("Lista de Transforms. El rodillo adoptará la posición Y la rotación exacta de cada punto.")]
    public List<Transform> patrolPoints;
    public float moveSpeed = 2.5f;
    [Tooltip("Velocidad con la que el rodillo gira sobre sí mismo para encarar la rotación del siguiente punto.")]
    public float turnToPointSpeed = 4f;
    public float waitTime = 1.5f;
    public float reachDistance = 0.2f;

    [Header("Efecto Rodillo (Giro Continuo)")]
    [Tooltip("El modelo del cilindro/rodillo que gira sobre sí mismo. Debe ser un hijo de este objeto.")]
    public Transform rollingMesh;
    public Vector3 rollAxis = Vector3.right; // Eje X local por defecto para el giro de un rodillo
    public float rollSpeed = 250f; // Grados por segundo de giro
    [Tooltip("Fuerza tangencial que empuja a la salchicha para hacerla rodar y caerse si se sube encima.")]
    public float rollPushForce = 8f;

    private int currentTargetIndex = 0;
    private Vector3 lastPosition;

    // Rastreador de pasajeros (salchicha)
    private List<Rigidbody> passengers = new List<Rigidbody>();

    void Start()
    {
        if (patrolPoints.Count > 0)
        {
            transform.position = patrolPoints[0].position;
            transform.rotation = patrolPoints[0].rotation;
        }
        lastPosition = transform.position;
        StartCoroutine(PatrolLoop());
    }

    void Update()
    {
        // El rodillo visual gira constantemente sobre su propio eje local para simular que rueda
        if (rollingMesh != null)
        {
            rollingMesh.Rotate(rollAxis * rollSpeed * Time.deltaTime, Space.Self);
        }
    }

    void FixedUpdate()
    {
        // 1. Mover a los pasajeros con la traslación de la plataforma (mismo sistema anti-escalas de la Roomba)
        Vector3 platformDelta = transform.position - lastPosition;
        lastPosition = transform.position;

        for (int i = passengers.Count - 1; i >= 0; i--)
        {
            if (passengers[i] != null)
            {
                passengers[i].position += platformDelta;

                // 2. EFECTO RODILLO: Empujamos físicamente a la salchicha en la dirección del giro del rodillo
                // Calculamos la dirección del empuje basándonos en el eje del mundo real perpendicular al eje de giro
                Vector3 pushDirection = Vector3.Cross(transform.TransformDirection(rollAxis), Vector3.up).normalized;

                // Aplicamos una fuerza continua para obligar al jugador a moverse si no quiere caerse
                passengers[i].AddForce(pushDirection * rollPushForce, ForceMode.Force);
            }
            else
            {
                passengers.RemoveAt(i);
            }
        }
    }

    IEnumerator PatrolLoop()
    {
        if (patrolPoints.Count < 2) yield break;

        while (true)
        {
            Transform targetPoint = patrolPoints[currentTargetIndex];

            // Mientras no hayamos llegado al punto objetivo
            while (Vector3.Distance(transform.position, targetPoint.position) > reachDistance)
            {
                // Mover posición hacia el punto
                Vector3 dir = (targetPoint.position - transform.position).normalized;
                transform.position += dir * moveSpeed * Time.deltaTime;

                // Rotar suavemente el objeto raíz hacia la rotación fija que tiene asignada ese patrol point
                transform.rotation = Quaternion.Slerp(transform.rotation, targetPoint.rotation, Time.deltaTime * turnToPointSpeed);

                yield return null;
            }

            // Asegurar posición y rotación fija exacta al llegar
            transform.position = targetPoint.position;
            transform.rotation = targetPoint.rotation;

            // Esperar en el punto antes de ir al siguiente
            yield return new WaitForSeconds(waitTime);

            // Indexar el siguiente punto de la ruta
            currentTargetIndex = (currentTargetIndex + 1) % patrolPoints.Count;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Rigidbody rb = collision.gameObject.GetComponentInParent<Rigidbody>();
        if (rb != null && !passengers.Contains(rb))
        {
            passengers.Add(rb);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        Rigidbody rb = collision.gameObject.GetComponentInParent<Rigidbody>();
        if (rb != null && passengers.Contains(rb))
        {
            passengers.Remove(rb);
        }
    }
}