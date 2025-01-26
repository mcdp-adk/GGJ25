using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collider2DTrigger : MonoBehaviour
{
    // ʹ��LayerMask��ѡ���
    public LayerMask targetLayer;
    public Animator animator;

    // ��Collider2D���봥����ʱ����
    private void OnTriggerEnter2D(Collider2D other)
    {
        // �����ײ�������Ƿ���Ŀ��㲢��tagΪPlayer
        if (other.CompareTag("Player"))
        {
            // ���ٸ�������
            PlayCollisionAnimation();
        }
    }
    void PlayCollisionAnimation()
    {
        Debug.Log("Collision detected");
        if (animator != null)
        {
            animator.SetBool("Collision", true); // ���趯���Ĵ�������Ϊ "Collision"
        }
    }
}
