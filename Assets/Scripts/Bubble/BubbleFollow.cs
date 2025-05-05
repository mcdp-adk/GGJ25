using System.Collections;
using UnityEngine;

public class BubbleFollow : MonoBehaviour
{
    private GameObject player;
    private bool isFollowing = false;
    public float launchForce = 10f;
    private Rigidbody2D rb;
    public float airResistance;

    private float bounceSpeed = 5f;
    private bool canCollide = true;

    void Start()
    {
        player = GameObject.FindWithTag("Player");
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;

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
            transform.position = player.transform.position;
        }
    }
    
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject == player)
        {
            isFollowing = false;
        }
    }

    void Update()
    {
        player = GameObject.FindWithTag("Player");
        if (isFollowing && player != null)
        {
            transform.position = player.transform.position;
        }

        if (isFollowing && (Input.GetKeyDown(KeyCode.K)|| Input.GetKeyDown(KeyCode.X)))
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            if (horizontal != 0 || vertical != 0)
            {
                Vector2 direction = new Vector2(horizontal, vertical).normalized;
                LaunchBubble(-direction);
            }
        }
    }

    void FixedUpdate()
    {
        Vector2 velocity = rb.velocity;
        Vector2 airResistanceForce = new Vector2(-velocity.x, 0).normalized * airResistance * velocity.sqrMagnitude;
        rb.AddForce(airResistanceForce);
    }

    void LaunchBubble(Vector2 direction)
    {
        isFollowing = false;

        if (rb != null)
        {
            rb.velocity = direction * launchForce;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (canCollide)
        {
            Vector2 normal = collision.contacts[0].normal;
            Vector2 currentVelocity = rb.velocity;
            float speed = currentVelocity.magnitude;
            
            float angle = Vector2.Angle(currentVelocity, -normal);
            
            Vector2 newVelocity;
            
            if (angle <= 5f || angle >= 175f)
            {
                newVelocity = -currentVelocity.normalized;
            }
            else
            {
                Vector2 right = new Vector2(normal.y, -normal.x);
                float direction = Vector2.Dot(currentVelocity, right);
                
                if (direction > 0)
                {
                    newVelocity = Quaternion.Euler(0, 0, -45) * normal;
                }
                else
                {
                    newVelocity = Quaternion.Euler(0, 0, 45) * normal;
                }
            }

            rb.velocity = newVelocity * speed;

            Debug.DrawRay(transform.position, normal * 2f, Color.green, 1f);
            Debug.DrawRay(transform.position, currentVelocity.normalized * 2f, Color.red, 1f);
            Debug.DrawRay(transform.position, newVelocity * 2f, Color.blue, 1f);

            StartCoroutine(CollisionCooldown());
        }
    }

    IEnumerator CollisionCooldown()
    {
        canCollide = false;
        yield return new WaitForSeconds(0.1f);
        canCollide = true;
    }
}
