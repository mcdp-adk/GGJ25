using UnityEngine;

public class Collider2DTrigger : MonoBehaviour
{
    // 使用LayerMask来选择层
    public LayerMask targetLayer;

    // 当Collider2D进入触发器时调用
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 检查碰撞的物体是否在目标层
        if (((1 << other.gameObject.layer) & targetLayer) != 0)
        {
            // 销毁父类物体
            Destroy(transform.parent.gameObject);
        }
    }
}