using TMPro;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private GameObject player;
    public TextMeshProUGUI checkpointText;
    private Vector3 startPosition;
    private Vector3 checkpointPosition = Vector3.zero;

    private bool hasCheckpoint = false;

    void Start()
    {
        player = GameObject.Find("Player");
        startPosition = player.transform.position;
        checkpointPosition = player.transform.position;
    }

    public Vector3 GetStartPosition()
    {
        return startPosition;
    }

    public void SetCheckpoint(Vector3 pos)
    {
        checkpointPosition = pos;
        hasCheckpoint = true;
        Debug.Log("Checkpoint renewed");
        checkpointText.SetText("Checkpoint reached!");
    }

    public Vector3 GetCheckpointPosition()
    {
        if (hasCheckpoint) Debug.Log("returning to checkpoint"); else Debug.Log("returning to start position");
            return hasCheckpoint ? checkpointPosition : startPosition;
    }
}
