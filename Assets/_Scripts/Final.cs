using System.Collections;
using TMPro;
using UnityEngine;

public class Final : MonoBehaviour
{
    public TextMeshProUGUI checkpointText;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            checkpointText.SetText("Congratulations! You've reached the end of the level!");
            StartCoroutine(HideTextCoroutine());
        }
    }

    IEnumerator HideTextCoroutine()
    {
        yield return new WaitForSeconds(2f);
        checkpointText.SetText("");
    }
}
