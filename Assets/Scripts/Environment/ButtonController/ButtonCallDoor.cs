using System.Collections;
using UnityEngine;

public class ButtonCallDoor : MonoBehaviour
{
    public bool button_activate = true;  // 是否处于激活状态
    public float shrinkDuration = 1.0f;  // 缩小持续时间

    public void ButtonActivate()
    {
        // 在这里实现函数的逻辑
        Debug.Log("Button activated!");

        // 开始逐渐消除的协程
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

        // 确保最终缩小到0
        transform.localScale = new Vector3(originalScale.x, 0, originalScale.z);

        // 删除自身
        Destroy(gameObject);
    }
}
