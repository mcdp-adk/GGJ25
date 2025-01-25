using UnityEngine;

public class BouncingBubble : MonoBehaviour
{
    public Vector2 initialVelocity;
    public float airResistance; // 风阻
    public GameObject player; // 玩家对象
    public float moveDuration = 2f; // 移动持续时间

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = initialVelocity;

        // 根据玩家对象的位置生成气泡
        if (player != null)
        {
            Vector2 playerPosition = player.transform.position;
            transform.position = playerPosition;
        }
    }

    void FixedUpdate()
    {
        // 根据玩家对象的位置生成气泡
        if (player != null)
        {
            Vector2 playerPosition = player.transform.position;
            float distance = Vector2.Distance(transform.position, playerPosition);

            // 使用SmoothStep函数动态调整速度
            float speed = Mathf.SmoothStep(0, distance, Time.deltaTime / moveDuration);

            transform.position = Vector2.MoveTowards(transform.position, playerPosition, speed * Time.deltaTime);
        }
        else
        {
            // 应用风阻
            Vector2 velocity = rb.velocity;
            Vector2 airResistanceForce = new Vector2(-velocity.x, 0).normalized * airResistance * velocity.sqrMagnitude;
            rb.AddForce(airResistanceForce);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // 反弹逻辑
        Vector2 normal = collision.contacts[0].normal;
        Vector2 newVelocity = Vector2.Reflect(rb.velocity, normal);
        rb.velocity = newVelocity;
    }
}