using UnityEngine;

public class ButtonCallDoor : MonoBehaviour
{
    public bool button_activate = true;  // 是否处于激活状态

    public void ButtonActivate()
    {
        // 在这里实现函数的逻辑
        Debug.Log("Button activated!");

        // 删除自身
        Destroy(gameObject);
    }
}
