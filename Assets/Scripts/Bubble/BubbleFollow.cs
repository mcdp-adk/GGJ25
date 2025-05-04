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

    private float bounceSpeed = 5f; // 反弹速度
    private bool canCollide = true; // 标记是否可以碰撞

    void Start()
    {
        // 查找带有“Player”标签的对象
        player = GameObject.FindWithTag("Player");
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true; // 防止泡泡旋转
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
    

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject == player)
        {
            isFollowing = false; // 玩家离开时停止跟随
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
        if (isFollowing && (Input.GetKeyDown(KeyCode.K)|| Input.GetKeyDown(KeyCode.X)))
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

    IEnumerator CollisionCooldown()
    {
        canCollide = false;
        yield return new WaitForSeconds(0.1f); // 冷却时间为0.1秒
        canCollide = true;
    }
}
