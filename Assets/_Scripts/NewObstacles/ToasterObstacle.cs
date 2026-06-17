using System.Collections;
using UnityEngine;

public class ToasterObstacle : MonoBehaviour
{
    [Header("Referencias de Ranuras")]
    public Rigidbody slotARb;
    public Rigidbody slotBRb;

    [Header("Tiempos")]
    public float idleTime = 4f;
    public float delayBetweenSlots = 1.2f;
    public float cooldownBeforeReset = 1.5f;

    [Header("Fuerzas")]
    public float popForce = 12f;

    // Variables para guardar posición Y rotación
    private Vector3 initialPosA;
    private Quaternion initialRotA;

    private Vector3 initialPosB;
    private Quaternion initialRotB;

    void Start()
    {
        if (slotARb != null)
        {
            initialPosA = slotARb.transform.localPosition;
            initialRotA = slotARb.transform.localRotation;
        }
        if (slotBRb != null)
        {
            initialPosB = slotBRb.transform.localPosition;
            initialRotB = slotBRb.transform.localRotation;
        }

        StartCoroutine(ToasterLoop());
    }

    IEnumerator ToasterLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(idleTime);

            if (slotARb != null)
            {
                slotARb.isKinematic = false;
                slotARb.AddForce(Vector3.up * popForce, ForceMode.Impulse);
            }

            yield return new WaitForSeconds(delayBetweenSlots);

            if (slotBRb != null)
            {
                slotBRb.isKinematic = false;
                slotBRb.AddForce(Vector3.up * popForce, ForceMode.Impulse);
            }

            yield return new WaitForSeconds(cooldownBeforeReset);

            // Volvemos a hacerlas cinemáticas para forzar su regreso ignorando físicas
            if (slotARb != null) { slotARb.isKinematic = true; slotARb.linearVelocity = Vector3.zero; slotARb.angularVelocity = Vector3.zero; }
            if (slotBRb != null) { slotBRb.isKinematic = true; slotBRb.linearVelocity = Vector3.zero; slotBRb.angularVelocity = Vector3.zero; }

            float elapsed = 0f;
            Vector3 currentPosA = slotARb != null ? slotARb.transform.localPosition : Vector3.zero;
            Quaternion currentRotA = slotARb != null ? slotARb.transform.localRotation : Quaternion.identity;

            Vector3 currentPosB = slotBRb != null ? slotBRb.transform.localPosition : Vector3.zero;
            Quaternion currentRotB = slotBRb != null ? slotBRb.transform.localRotation : Quaternion.identity;

            while (elapsed < 0.5f)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / 0.5f;

                if (slotARb != null)
                {
                    slotARb.transform.localPosition = Vector3.Lerp(currentPosA, initialPosA, t);
                    slotARb.transform.localRotation = Quaternion.Slerp(currentRotA, initialRotA, t);
                }

                if (slotBRb != null)
                {
                    slotBRb.transform.localPosition = Vector3.Lerp(currentPosB, initialPosB, t);
                    slotBRb.transform.localRotation = Quaternion.Slerp(currentRotB, initialRotB, t);
                }
                yield return null;
            }
        }
    }
}