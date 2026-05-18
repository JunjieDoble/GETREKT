using UnityEngine;

public class Moving : MonoBehaviour
{
    [Header("Waypoints")]
    public Transform[] points;

    [Header("Movement")]
    public float speed = 3f;
    public float reachDistance = 0.1f;

    private int currentIndex = 0;

    void Update()
    {
        if (points == null || points.Length == 0) return;

        Transform target = points[currentIndex];

        Vector3 direction = (target.position - transform.position).normalized;

        transform.position += direction * speed * Time.deltaTime;

        float distance = Vector3.Distance(transform.position, target.position);

        if (distance <= reachDistance)
        {
            currentIndex = (currentIndex + 1) % points.Length;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            collision.transform.SetParent(transform, true);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            collision.transform.SetParent(null);
        }
    }
}
