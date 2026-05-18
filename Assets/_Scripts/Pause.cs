using UnityEngine;
using UnityEngine.InputSystem;

public class Pause : MonoBehaviour
{
    private GameObject player;
    private bool isPaused = false;

    public GameObject pauseMenu;
    public Checkpoint checkpoint;

    private void Start()
    {
        player = GameObject.Find("Player");

        if (pauseMenu != null)
        {
            pauseMenu.SetActive(false);
        }
        else 
        {
            Debug.LogWarning("Pause menu GameObject is not assigned.");
        }
    }

    public void OnEsc(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            TogglePause();
        }
    }

    public void LoadLastCheckpoint()
    {
        if (checkpoint != null)
        {
            player.transform.position = checkpoint.GetCheckpointPosition();
            TogglePause();

        }
        else
        {
            Debug.LogWarning("Checkpoint component not found on the Checkpoint GameObject.");
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        if (!isPaused) 
        {
            Time.timeScale = 1f;
            pauseMenu.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
        }
        else 
        {
            Time.timeScale = 0f;
            pauseMenu.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
