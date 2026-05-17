using System.Collections;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class Final : MonoBehaviour
{
    public TextMeshProUGUI checkpointText;
    public Transform sartenFinal;

    [Header("Pan eject")]
    public float rotationAngle = -90f;
    public float rotationSpeed = 50f;
    public float ejectDelay = 1f;

    private GameObject player;
    private Quaternion targetRotation;
    private bool hasRotated = false;

    private void Start()
    {
        player = GameObject.Find("Player");
        targetRotation = sartenFinal.localRotation * Quaternion.Euler(0f, rotationAngle, 0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasRotated) return;

        if (other.gameObject.CompareTag("Player"))
        {
            hasRotated = true;
            checkpointText.SetText("Congratulations! You've reached the end of the level!");
            StartCoroutine(HideTextCoroutine());

            StartCoroutine(RotateDoor());
        }
    }

    IEnumerator HideTextCoroutine()
    {
        yield return new WaitForSeconds(2f);
        checkpointText.SetText("");
    }

    private IEnumerator RotateDoor()
    {
        while (Quaternion.Angle(sartenFinal.transform.localRotation, targetRotation) > 0.1f)
        {
            sartenFinal.transform.localRotation = Quaternion.RotateTowards(
                sartenFinal.transform.localRotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
            yield return null;
        }

        sartenFinal.transform.localRotation = targetRotation;
        player.GetComponent<Rigidbody>().AddForce((sartenFinal.forward * 25f) + Vector3.up * 15f, ForceMode.Impulse);
    }
}
