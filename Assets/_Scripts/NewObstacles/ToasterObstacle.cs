using System.Collections;
using UnityEngine;

public class ToasterObstacle : MonoBehaviour
{
    [Header("Referencias")]
    public Rigidbody toasterPlatformRb; // La rejilla/tostada que sube y baja física

    [Header("Tiempos")]
    public float idleTime = 3f;         // Tiempo abajo antes de saltar
    public float CooldownBeforeReset = 1.5f; // Tiempo arriba antes de volver a bajar suavemente

    [Header("Fuerzas")]
    public float popForce = 12f;        // Fuerza del salto vertical

    private Vector3 initialLocalPosition;
    private bool isPopping = false;

    void Start()
    {
        if (toasterPlatformRb != null)
        {
            initialLocalPosition = toasterPlatformRb.transform.localPosition;
        }
        StartCoroutine(ToasterLoop());
    }

    IEnumerator ToasterLoop()
    {
        while (true)
        {
            // Esperar en estado de reposo
            yield return new WaitForSeconds(idleTime);

            // ¡Saltar!
            isPopping = true;
            toasterPlatformRb.isKinematic = false;
            toasterPlatformRb.AddForce(Vector3.up * popForce, ForceMode.Impulse);

            // Mantener arriba o esperar a que la física actúe
            yield return new WaitForSeconds(CooldownBeforeReset);

            // Resetear suavemente la plataforma a su base de manera cinemática para evitar descontrol de físicas
            isPopping = false;
            toasterPlatformRb.isKinematic = true;

            float elapsed = 0f;
            Vector3 currentLocalPos = toasterPlatformRb.transform.localPosition;

            while (elapsed < 0.5f)
            {
                elapsed += Time.deltaTime;
                toasterPlatformRb.transform.localPosition = Vector3.Lerp(currentLocalPos, initialLocalPosition, elapsed / 0.5f);
                yield return null;
            }
            toasterPlatformRb.transform.localPosition = initialLocalPosition;
        }
    }
}