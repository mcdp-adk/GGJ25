using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI; // ������ͣ�˵�Panel
    public Button resumeButton;
    public Button exitButton;

    private bool isPaused = false;

    void Start()
    {
        resumeButton.onClick.AddListener(Resume);
        exitButton.onClick.AddListener(ExitGame);
        pauseMenuUI.SetActive(false); // ��ʼ������ͣ�˵�
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
        pauseMenuUI.SetActive(true); // ��ʾ��ͣ�˵�
        Time.timeScale = 0f; // ��ͣ��Ϸ
        isPaused = true;
    }

    void Resume()
    {
        pauseMenuUI.SetActive(false); // ������ͣ�˵�
        Time.timeScale = 1f; // �ָ���Ϸ
        isPaused = false;
    }

    void ExitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // �༭�����˳�����
#endif
    }
}