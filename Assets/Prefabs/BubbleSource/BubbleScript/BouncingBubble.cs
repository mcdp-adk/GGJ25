using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncingBubble : MonoBehaviour
{
    public Vector2 initialVelocity;
    public float airResistance; // ����
    public GameObject player; // ��Ҷ���
    public float moveDuration = 2f; // �ƶ�����ʱ��

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = initialVelocity;

        // ������Ҷ����λ����������
        if (player != null)
        {
            Vector2 playerPosition = player.transform.position;
            transform.position = playerPosition;
        }
    }

    void FixedUpdate()
    {
        // ������Ҷ����λ����������
        if (player != null)
        {
            Vector2 playerPosition = player.transform.position;
            float distance = Vector2.Distance(transform.position, playerPosition);

            // ʹ��SmoothStep������̬�����ٶ�
            float speed = Mathf.SmoothStep(0, distance, Time.deltaTime / moveDuration);

            transform.position = Vector2.MoveTowards(transform.position, playerPosition, speed * Time.deltaTime);
        }
        else
        {
            // Ӧ�÷���
            Vector2 velocity = rb.velocity;
            Vector2 airResistanceForce = new Vector2(-velocity.x, 0).normalized * airResistance * velocity.sqrMagnitude;
            rb.AddForce(airResistanceForce);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // �����߼�
        Vector2 normal = collision.contacts[0].normal;
        Vector2 newVelocity = Vector2.Reflect(rb.velocity, normal);
        rb.velocity = newVelocity;
    }
}