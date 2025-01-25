using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Button startButton;
    public Button creditsButton;
    public Button exitButton;
    public GameObject creditsPanel; // �����������������
    public Button closeCreditsButton; // ���ùرհ�ť

    void Start()
    {
        startButton.onClick.AddListener(StartGame);
        creditsButton.onClick.AddListener(ShowCredits);
        exitButton.onClick.AddListener(ExitGame);
        closeCreditsButton.onClick.AddListener(HideCredits); // �رհ�ť�ļ����¼�
        creditsPanel.SetActive(false); // ��ʼ״̬�����������������
    }

    void StartGame()
    {
        SceneManager.LoadScene("TarodevTest"); // �滻Ϊ������Ϸ��������
    }

    void ShowCredits()
    {
        creditsPanel.SetActive(true); // ��ʾ�������������
    }

    void HideCredits()
    {
        creditsPanel.SetActive(false); // �����������������
    }

    void ExitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // �༭�����˳�����
#endif
    }
}
