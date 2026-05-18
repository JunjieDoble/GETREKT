using System.Collections;
using TMPro;
using UnityEngine;

public class Trash : MonoBehaviour
{
    public TextMeshProUGUI checkpointText;
    public float exitDelay = 5f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            checkpointText.SetText("You are dirty, I don't want to cook you.");
            StartCoroutine(ExitInSeconds());
        }
    }

    private IEnumerator ExitInSeconds()
    {
        float remaining = exitDelay;
        while (remaining > 0f)
        {
            yield return new WaitForSeconds(1f);
            remaining -= 1f;
        }

        Application.Quit();
    }
}
