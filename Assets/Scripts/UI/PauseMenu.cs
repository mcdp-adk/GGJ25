using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{

    [SerializeField] private GameObject pauseMenuUI; // 拖入暂停菜单Panel
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button exitButton;

    private bool isPaused = false;

    void Start()
    {
        resumeButton.onClick.AddListener(Resume);
        exitButton.onClick.AddListener(ExitGame);
        pauseMenuUI.SetActive(false); // 初始隐藏暂停菜单
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
        pauseMenuUI.SetActive(true); // 显示暂停菜单
        Time.timeScale = 0f; // 暂停游戏
        isPaused = true;
    }

    void Resume()
    {
        pauseMenuUI.SetActive(false); // 隐藏暂停菜单
        Time.timeScale = 1f; // 恢复游戏
        isPaused = false;
    }

    void ExitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // 编辑器中退出播放
#endif
    }
}