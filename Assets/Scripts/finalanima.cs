using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collider2DTrigger : MonoBehaviour
{
    public LayerMask targetLayer;
    public Animator animator;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayCollisionAnimation();
        }
    }
    void PlayCollisionAnimation()
    {
        Debug.Log("Collision detected");
        if (animator != null)
        {
            animator.SetBool("Collision", true); 
        }
    }
}
