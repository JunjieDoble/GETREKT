using System.Collections;
using UnityEngine;

public class UtensilAttack : MonoBehaviour
{
    [Header("Tiempos del Ciclo")]
    public float idleTime = 4f;
    public float slamSpeed = 15f;
    public float returnSpeed = 2f;

    [Header("Ángulos de Rotación (Eje Z o X local)")]
    public Vector3 rotationAxis = Vector3.forward;
    public float restAngle = 0f;
    public float pullBackAngle = -30f;
    public float slamAngle = 60f;

    private Quaternion baseRotation;

    void Start()
    {
        baseRotation = transform.localRotation;
        StartCoroutine(SlamCycle());
    }

    IEnumerator SlamCycle()
    {
        while (true)
        {
            yield return new WaitForSeconds(idleTime);

            // 1. Cargar hacia atrás lentamente
            float elapsed = 0f;
            while (elapsed < 1f)
            {
                elapsed += Time.deltaTime * returnSpeed;
                float angle = Mathf.Lerp(restAngle, pullBackAngle, elapsed);
                transform.localRotation = baseRotation * Quaternion.AngleAxis(angle, rotationAxis);
                yield return null;
            }

            // Esperar cargado un instante antes del golpe
            yield return new WaitForSeconds(0.3f);

            // 2. ¡GOLPEAR! (Rápido)
            elapsed = 0f;
            while (elapsed < 1f)
            {
                elapsed += Time.deltaTime * slamSpeed;
                float angle = Mathf.Lerp(pullBackAngle, slamAngle, elapsed);
                transform.localRotation = baseRotation * Quaternion.AngleAxis(angle, rotationAxis);
                yield return null;
            }

            // 3. Regresar al estado de reposo inicial
            elapsed = 0f;
            while (elapsed < 1f)
            {
                elapsed += Time.deltaTime * returnSpeed;
                float angle = Mathf.Lerp(slamAngle, restAngle, elapsed);
                transform.localRotation = baseRotation * Quaternion.AngleAxis(angle, rotationAxis);
                yield return null;
            }
        }
    }
}