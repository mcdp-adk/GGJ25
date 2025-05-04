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
            // 反弹逻辑
            Vector2 normal = collision.contacts[0].normal;
            Vector2 newVelocity = Vector2.Reflect(rb.velocity, normal);
            rb.velocity = newVelocity;

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
