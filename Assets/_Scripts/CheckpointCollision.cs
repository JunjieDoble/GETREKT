using UnityEngine;

public class CheckpointCollision : MonoBehaviour
{
    public Checkpoint checkpoint;
    private bool isReached = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isReached)
        {
            isReached = true;

            if (checkpoint != null)
            {
                checkpoint.SetCheckpoint(transform.position);
            }
        }
    }
}
