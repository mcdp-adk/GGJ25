using System.Collections;
using UnityEngine;

public class ButtonCallDoor : MonoBehaviour
{
    public bool button_activate = true;  // �Ƿ��ڼ���״̬
    public float shrinkDuration = 1.0f;  // ��С����ʱ��

    public void ButtonActivate()
    {
        // ������ʵ�ֺ������߼�
        Debug.Log("Button activated!");

        // ��ʼ��������Э��
        StartCoroutine(ShrinkAndDestroy());
    }

    private IEnumerator ShrinkAndDestroy()
    {
        float elapsedTime = 0f;
        Vector3 originalScale = transform.localScale;

        while (elapsedTime < shrinkDuration)
        {
            float progress = elapsedTime / shrinkDuration;
            transform.localScale = new Vector3(originalScale.x, Mathf.Lerp(originalScale.y, 0, progress), originalScale.z);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // ȷ��������С��0
        transform.localScale = new Vector3(originalScale.x, 0, originalScale.z);

        // ɾ������
        Destroy(gameObject);
    }
}
