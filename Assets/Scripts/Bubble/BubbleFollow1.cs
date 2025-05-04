using System.Collections;
using UnityEngine;

public class BubbleFollow1 : MonoBehaviour
{
    private GameObject player; // 玩家对象
    private bool isAttached = false; // 标记玩家是否被附着
    public float launchForce = 10f; // 发射力
    private Rigidbody2D rb;
    public float airResistance; // 风阻
    private bool canCollide = true; // 标记是否可以碰撞
    private Rigidbody2D playerRb; // 玩家的Rigidbody2D组件

    void Start()
    {
        player = GameObject.FindWithTag("Player");
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;

        if (player != null)
        {
            playerRb = player.GetComponent<Rigidbody2D>();
            Debug.Log("Player found");
        }
        else
        {
            Debug.LogError("Player not found. Make sure the player object has the 'Player' tag.");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == player && !isAttached)
        {
            isAttached = true;
            if (playerRb != null)
            {
                // 禁用玩家的Rigidbody2D控制
                playerRb.simulated = false;
            }
        }
    }

    void Update()
    {
        if (isAttached && player != null)
        {
            // 让玩家跟随泡泡移动
            player.transform.position = transform.position;
        }
    }

    void FixedUpdate()
    {
        // 应用风阻
        Vector2 velocity = rb.velocity;
        Vector2 airResistanceForce = new Vector2(-velocity.x, 0).normalized * airResistance * velocity.sqrMagnitude;
        rb.AddForce(airResistanceForce);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (canCollide)
        {
            Vector2 normal = collision.contacts[0].normal;
            Vector2 currentVelocity = rb.velocity;
            float speed = currentVelocity.magnitude;

            // 计算入射方向与法线的夹角
            float angle = Vector2.Angle(currentVelocity, -normal);
            
            Vector2 newVelocity;
            
            // 判断是直射还是斜射（允许5度的误差）
            if (angle <= 5f || angle >= 175f)
            {
                // 直射情况：直接反向
                newVelocity = -currentVelocity.normalized;
            }
            else
            {
                // 斜射情况：固定45度角反射
                // 确定是从左侧还是右侧入射
                Vector2 right = new Vector2(normal.y, -normal.x); // 法线的垂直方向
                float direction = Vector2.Dot(currentVelocity, right);
                
                // 根据入射方向选择45度角的反射方向
                if (direction > 0)
                {
                    // 从左侧入射
                    newVelocity = Quaternion.Euler(0, 0, -45) * normal;
                }
                else
                {
                    // 从右侧入射
                    newVelocity = Quaternion.Euler(0, 0, 45) * normal;
                }
            }

            // 应用反弹速度，保持原有速度大小
            rb.velocity = newVelocity * speed;

            // 添加调试可视化
            Debug.DrawRay(transform.position, normal * 2f, Color.green, 1f);
            Debug.DrawRay(transform.position, currentVelocity.normalized * 2f, Color.red, 1f);
            Debug.DrawRay(transform.position, newVelocity * 2f, Color.blue, 1f);

            StartCoroutine(CollisionCooldown());
        }
    }

    void OnDestroy()
    {
        // 当泡泡被销毁时恢复玩家的控制
        if (playerRb != null)
        {
            playerRb.simulated = true;
        }
    }

    IEnumerator CollisionCooldown()
    {
        canCollide = false;
        yield return new WaitForSeconds(0.1f);
        canCollide = true;
    }
}
