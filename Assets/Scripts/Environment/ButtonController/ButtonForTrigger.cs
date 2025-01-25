using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonForDoor : MonoBehaviour
{
    public GameObject targetObject; // ��Ҫ�����ű��仯��Ŀ������
    private ButtonCallDoor targetScript; // Ŀ�������ϵĽű�

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

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTriggerEnter called with: " + other.name); // ��ӵ�����Ϣ

        if (other.CompareTag("Player")) // ȷ����ײ�������ǩ
        {
            if (targetScript != null)
            {
                targetScript.ButtonActivate(); // ����ButtonActivate����
                Debug.Log("ButtonActivate called on targetScript."); // ��ӵ�����Ϣ

                // ����SpriteRenderer�е�spriteΪBUTTON_0
                SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
                if (spriteRenderer != null)
                {
                    Sprite newSprite = Resources.Load<Sprite>("BUTTON_0");
                    if (newSprite != null)
                    {
                        spriteRenderer.sprite = newSprite;
                        Debug.Log("Sprite changed to BUTTON_0."); // ��ӵ�����Ϣ
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