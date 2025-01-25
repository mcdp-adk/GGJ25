using System.Collections;
using UnityEngine;

public class BubbleFollow : MonoBehaviour
{
    private GameObject player; // ��Ҷ���
    private bool isFollowing = false; // ���ԲȦ�Ƿ��ڸ������
    public float launchForce = 10f; // ������
    private Rigidbody2D rb;
    //public Vector2 initialVelocity;
    public float airResistance; // ����
    private bool canCollide = true; // ����Ƿ������ײ

    void Start()
    {
        // ���Ҵ��С�Player����ǩ�Ķ���
        player = GameObject.FindWithTag("Player");
        rb = GetComponent<Rigidbody2D>();
        //rb.velocity = initialVelocity;

        // ȷ����Һ�ԲȦ��ͬһ��Layer��Layer֮�����ײ��������õ�
        if (player != null)
        {
            Debug.Log("Player found");
        }
        else
        {
            Debug.LogError("Player not found. Make sure the player object has the 'Player' tag.");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == player)
        {
            isFollowing = true;
            transform.position = player.transform.position; // ��ԲȦ�ƶ����������
        }
    }


    void Update()
    {
        player = GameObject.FindWithTag("Player");
        if (isFollowing && player != null)
        {
            transform.position = player.transform.position; // ��ԲȦ��������ƶ�
        }

        // ���K���ͷ����������
        if (Input.GetKeyDown(KeyCode.K))
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            if (horizontal != 0 || vertical != 0)
            {
                Vector2 direction = new Vector2(horizontal, vertical).normalized;
                LaunchBubble(-direction); // ����������
            }
        }
    }

    void FixedUpdate()
    {
        // Ӧ�÷���
        Vector2 velocity = rb.velocity;
        Vector2 airResistanceForce = new Vector2(-velocity.x, 0).normalized * airResistance * velocity.sqrMagnitude;
        rb.AddForce(airResistanceForce);
    }

    void LaunchBubble(Vector2 direction)
    {
        isFollowing = false; // ֹͣ�������

        if (rb != null)
        {
            rb.velocity = direction * launchForce; // ����������
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (canCollide)
        {
            // �����߼�
            Vector2 normal = collision.contacts[0].normal;
            Vector2 newVelocity = Vector2.Reflect(rb.velocity, normal);
            rb.velocity = newVelocity;

            // ������ȴЭ��
            StartCoroutine(CollisionCooldown());
        }
    }

    IEnumerator CollisionCooldown()
    {
        canCollide = false;
        yield return new WaitForSeconds(0.1f); // ��ȴʱ��Ϊ0.1��
        canCollide = true;
    }
}
