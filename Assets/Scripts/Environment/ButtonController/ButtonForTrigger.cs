using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonForDoor : MonoBehaviour
{
    public GameObject targetObject; // 需要触发脚本变化的目标物体
    private ButtonCallDoor targetScript; // 目标物体上的脚本
    public Sprite newSprite; // 需要被替换为的图片

    void Start()
    {
        if (targetObject != null)
        {
            targetScript = targetObject.GetComponent<ButtonCallDoor>();
            if (targetScript == null)
            {
                Debug.LogError("Target object does not have a ButtonCallDoor component.");
            }
        }
        else
        {
            Debug.LogError("Target object is not assigned.");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("OnTriggerEnter2D called with: " + other.name); // 添加调试信息

        if (other.CompareTag("Player")) // 确认碰撞的物体标签
        {
            if (targetScript != null)
            {
                targetScript.ButtonActivate(); // 调用ButtonActivate函数
                Debug.Log("ButtonActivate called on targetScript."); // 添加调试信息

                // 更改SpriteRenderer中的sprite为BUTTON_0
                SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
                if (spriteRenderer != null)
                {
              
                    if (newSprite != null)
                    {
                        spriteRenderer.sprite = newSprite;
                        Debug.Log("Sprite changed to BUTTON_0."); // 添加调试信息
                    }
                    else
                    {
                        Debug.LogError("Failed to load sprite BUTTON_0.");
                    }
                }
                else
                {
                    Debug.LogError("SpriteRenderer component not found.");
                }
            }
            else
            {
                Debug.LogWarning("ButtonCallDoor is not assigned or missing.");
            }
        }
    }
}