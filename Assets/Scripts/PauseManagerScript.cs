using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenuUI;
    private bool isPaused = false;

    void Update()
    {
        if (Keyboard.current != null &&
            Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        pauseMenuUI.transform.SetAsLastSibling();
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    // 🔥 RESTART CURRENT LEVEL
    public void RestartLevel()
    {
        Time.timeScale = 1f; // VERY IMPORTANT
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // 🔥 RESTART GAME → MAIN MENU
    public void RestartGame()
    {
        Time.timeScale = 1f; // ⭐ THIS FIXES YOUR ISSUE
        SceneManager.LoadScene("MainMenu");
    }
}
