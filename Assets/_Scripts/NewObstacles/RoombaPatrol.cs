using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoombaPatrol : MonoBehaviour
{
    [Header("Ruta de Patrulla")]
    public List<Transform> patrolPoints;
    public float speed = 2.5f;
    public float waitTime = 1.5f;
    public float reachDistance = 0.2f;

    private int currentTargetIndex = 0;
    private bool isWaiting = false;
    private Vector3 lastPosition;

    // Lista para rastrear rigidbodies que están subidos a la Roomba
    private List<Rigidbody> passengers = new List<Rigidbody>();

    void Start()
    {
        if (patrolPoints.Count > 0)
        {
            transform.position = patrolPoints[0].position;
        }
        lastPosition = transform.position;
        StartCoroutine(PatrolLoop());
    }

    void FixedUpdate()
    {
        // Calcular cuánto se movió la Roomba en este frame físico
        Vector3 platformVelocity = transform.position - lastPosition;
        lastPosition = transform.position;

        // Mover a los pasajeros de forma segura sin cambiar su jerarquía de escala
        for (int i = passengers.Count - 1; i >= 0; i--)
        {
            if (passengers[i] != null)
            {
                passengers[i].position += platformVelocity;
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

            while (Vector3.Distance(transform.position, targetPoint.position) > reachDistance)
            {
                Vector3 dir = (targetPoint.position - transform.position).normalized;
                transform.position += dir * speed * Time.deltaTime;
                yield return null;
            }

            // Llegamos al punto, esperar
            isWaiting = true;
            yield return new WaitForSeconds(waitTime);
            isWaiting = false;

            // Siguiente punto indexado
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