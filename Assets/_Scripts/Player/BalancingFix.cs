using UnityEngine;

public class BalancingFix : MonoBehaviour
{
    public float bridgeForce = 10f;

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider.CompareTag("Balancin"))
        {
            Rigidbody rb = hit.collider.attachedRigidbody;
            if (rb != null)
            {
                rb.AddForceAtPosition(-hit.normal * bridgeForce, hit.point);
            }
        }
    }
}
