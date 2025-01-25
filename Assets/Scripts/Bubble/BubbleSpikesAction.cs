using UnityEngine;

public class Collider2DTrigger : MonoBehaviour
{
    // ʹ��LayerMask��ѡ���
    public LayerMask targetLayer;

    // ��Collider2D���봥����ʱ����
    private void OnTriggerEnter2D(Collider2D other)
    {
        // �����ײ�������Ƿ���Ŀ���
        if (((1 << other.gameObject.layer) & targetLayer) != 0)
        {
            // ���ٸ�������
            Destroy(transform.parent.gameObject);
        }
    }
}