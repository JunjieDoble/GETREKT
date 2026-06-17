using UnityEngine;

public class DirtSurface : MonoBehaviour
{
    public float dirtPerSecond = 10f;

    private void OnCollisionStay(Collision collision)
    {
        DirtyMeter tracker = collision.gameObject.GetComponentInParent<DirtyMeter>();
        if (tracker != null)
        {
            tracker.IncreaseDirt(dirtPerSecond * Time.deltaTime);
        }
    }
}