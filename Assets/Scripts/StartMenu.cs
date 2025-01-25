using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button creditsButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private GameObject creditsPanel; // �����������������
    [SerializeField] private Button closeCreditsButton; // ���ùرհ�ť

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
