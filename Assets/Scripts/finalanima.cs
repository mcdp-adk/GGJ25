using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collider2DTrigger : MonoBehaviour
{
    // 使用LayerMask来选择层
    public LayerMask targetLayer;
    public Animator animator;

    // 当Collider2D进入触发器时调用
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 检查碰撞的物体是否在目标层并且tag为Player
        if (other.CompareTag("Player"))
        {
            // 销毁父类物体
            PlayCollisionAnimation();
        }
    }
    void PlayCollisionAnimation()
    {
        Debug.Log("Collision detected");
        if (animator != null)
        {
            animator.SetBool("Collision", true); // 假设动画的触发器名为 "Collision"
        }
    }
}
