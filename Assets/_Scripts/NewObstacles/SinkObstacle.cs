using System.Collections;
using UnityEngine;

public class SinkObstacle : MonoBehaviour
{
    [Header("Tiempos del Grifo")]
    public float waterOnDuration = 2f;
    public float waterOffDuration = 3f;

    [Header("Fuerzas y Efectos")]
    public Vector3 pushDirection = Vector3.down;
    public float pushForce = 8f;
    public float dirtIncreasePerSecond = 15f; // "Moja" y ensucia a la salchicha

    [Header("Referencias Visuales")]
    public GameObject waterVisuals; // El cilindro/partículas de agua chorreando

    private bool isWaterFlowing = false;

    void Start()
    {
        StartCoroutine(FaucetCycle());
    }

    IEnumerator FaucetCycle()
    {
        while (true)
        {
            // Agua Apagada
            isWaterFlowing = false;
            if (waterVisuals != null) waterVisuals.SetActive(false);
            yield return new WaitForSeconds(waterOffDuration);

            // Agua Encendida
            isWaterFlowing = true;
            if (waterVisuals != null) waterVisuals.SetActive(true);
            yield return new WaitForSeconds(waterOnDuration);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!isWaterFlowing) return;

        PlayerMovement player = other.GetComponentInParent<PlayerMovement>();
        if (player != null)
        {
            Rigidbody rb = player.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // Empujamos constantemente a la salchicha hacia abajo/lados
                rb.AddForce(pushDirection.normalized * pushForce, ForceMode.Acceleration);
            }

            // Accedemos al sistema de suciedad para penalizarlo por mojado/manchado
            /*PlayerDirtTracker dirtTracker = player.GetComponent<PlayerDirtTracker>();
            if (dirtTracker != null)
            {
                dirtTracker.IncreaseDirt(dirtIncreasePerSecond * Time.deltaTime);
            }*/
        }
    }
}