using System.Collections;
using UnityEngine;

public class BubbleFollow : MonoBehaviour
{
    private GameObject player; // 玩家对象
    private bool isFollowing = false; // 标记圆圈是否在跟随玩家
    public float launchForce = 10f; // 发射力
    private Rigidbody2D rb;
    //public Vector2 initialVelocity;
    public float airResistance; // 风阻
    private bool canCollide = true; // 标记是否可以碰撞

    void Start()
    {
        // 查找带有“Player”标签的对象
        player = GameObject.FindWithTag("Player");
        rb = GetComponent<Rigidbody2D>();
        //rb.velocity = initialVelocity;

        // 确保玩家和圆圈在同一个Layer或Layer之间的碰撞检测是启用的
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
            transform.position = player.transform.position; // 将圆圈移动到玩家中心
        }
    }


    void Update()
    {
        player = GameObject.FindWithTag("Player");
        if (isFollowing && player != null)
        {
            transform.position = player.transform.position; // 让圆圈跟随玩家移动
        }

        // 检测K键和方向键的输入
        if (Input.GetKeyDown(KeyCode.K))
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            if (horizontal != 0 || vertical != 0)
            {
                Vector2 direction = new Vector2(horizontal, vertical).normalized;
                LaunchBubble(-direction); // 按反方向发射
            }
        }
    }

    void FixedUpdate()
    {
        // 应用风阻
        Vector2 velocity = rb.velocity;
        Vector2 airResistanceForce = new Vector2(-velocity.x, 0).normalized * airResistance * velocity.sqrMagnitude;
        rb.AddForce(airResistanceForce);
    }

    void LaunchBubble(Vector2 direction)
    {
        isFollowing = false; // 停止跟随玩家

        if (rb != null)
        {
            rb.velocity = direction * launchForce; // 按反方向发射
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (canCollide)
        {
            // 反弹逻辑
            Vector2 normal = collision.contacts[0].normal;
            Vector2 newVelocity = Vector2.Reflect(rb.velocity, normal);
            rb.velocity = newVelocity;

            // 启动冷却协程
            StartCoroutine(CollisionCooldown());
        }
    }

    IEnumerator CollisionCooldown()
    {
        canCollide = false;
        yield return new WaitForSeconds(0.1f); // 冷却时间为0.1秒
        canCollide = true;
    }
}
