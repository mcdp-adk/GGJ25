using UnityEngine;

public class ButtonCallDoor : MonoBehaviour
{
    public bool button_activate = true;  // �Ƿ��ڼ���״̬

    public void ButtonActivate()
    {
        // ������ʵ�ֺ������߼�
        Debug.Log("Button activated!");

        // ɾ������
        Destroy(gameObject);
    }
}
