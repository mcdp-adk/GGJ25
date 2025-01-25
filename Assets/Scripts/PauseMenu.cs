using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI; // Õœ»Î‘›Õ£≤Àµ•Panel
    public Button resumeButton;
    public Button exitButton;

    private bool isPaused = false;

    void Start()
    {
        resumeButton.onClick.AddListener(Resume);
        exitButton.onClick.AddListener(ExitGame);
        pauseMenuUI.SetActive(false); // ≥ı º“˛≤ÿ‘›Õ£≤Àµ•
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true); // œ‘ æ‘›Õ£≤Àµ•
        Time.timeScale = 0f; // ‘›Õ£”Œœ∑
        isPaused = true;
    }

    void Resume()
    {
        pauseMenuUI.SetActive(false); // “˛≤ÿ‘›Õ£≤Àµ•
        Time.timeScale = 1f; // ª÷∏¥”Œœ∑
        isPaused = false;
    }

    void ExitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // ±‡º≠∆˜÷–ÕÀ≥ˆ≤•∑≈
#endif
    }
}