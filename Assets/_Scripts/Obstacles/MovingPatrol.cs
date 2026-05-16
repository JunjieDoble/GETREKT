using System.Collections;
using UnityEngine;

public class MovingPatrol : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;

    public float speed = 3f;
    public float waitTime = 1f;
    public float reachDistance = 0.1f;

    private Transform target;
    private bool waiting;

    void Start()
    {
        target = pointB;
    }

    void Update()
    {
        if (waiting || pointA == null || pointB == null) return;

        Move();
    }

    void Move()
    {
        Vector3 dir = (target.position - transform.position).normalized;
        transform.position += dir * speed * Time.deltaTime;

        float distance = Vector3.Distance(transform.position, target.position);

        if (distance <= reachDistance)
        {
            StartCoroutine(SwitchTarget());
        }
    }

    IEnumerator SwitchTarget()
    {
        waiting = true;

        yield return new WaitForSeconds(waitTime);

        target = (target == pointA) ? pointB : pointA;

        waiting = false;
    }
}