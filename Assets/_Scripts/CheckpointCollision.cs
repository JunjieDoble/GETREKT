using UnityEngine;

public class CheckpointCollision : MonoBehaviour
{
    private bool isReached = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isReached)
        {
            isReached = true;

            Checkpoint checkpoint = other.GetComponent<Checkpoint>();
            if (checkpoint != null)
            {
                checkpoint.SetCheckpoint(transform.position);
            }
        }
    }
}
