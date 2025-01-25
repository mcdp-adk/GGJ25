using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button creditsButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private GameObject creditsPanel; // 引用制作组名单面板
    [SerializeField] private Button closeCreditsButton; // 引用关闭按钮

    void Start()
    {
        startButton.onClick.AddListener(StartGame);
        creditsButton.onClick.AddListener(ShowCredits);
        exitButton.onClick.AddListener(ExitGame);
        closeCreditsButton.onClick.AddListener(HideCredits); // 关闭按钮的监听事件
        creditsPanel.SetActive(false); // 初始状态隐藏制作组名单面板
    }

    void StartGame()
    {
        SceneManager.LoadScene("MainGame"); // 替换为您的游戏场景名称
    }

    void ShowCredits()
    {
        creditsPanel.SetActive(true); // 显示制作组名单面板
    }

    void HideCredits()
    {
        creditsPanel.SetActive(false); // 隐藏制作组名单面板
    }

    void ExitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // 编辑器中退出播放
#endif
    }
}
