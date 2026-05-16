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
            Debug.Log("Checkpoint reached at position: " + transform.position);

            if (checkpoint != null)
            {
                checkpoint.SetCheckpoint(transform.position);
            }
        }
    }
}
