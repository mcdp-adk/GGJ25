using UnityEngine;

public class BubbleController : MonoBehaviour
{
    private Rigidbody2D _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    public void initialPlayerState(PlayerState state)
    {
        // Handle the initial state
    }

    public void initialBubbleVelosity(Vector2 velocity)
    {
        if (_rb != null)
        {
            _rb.velocity = velocity;
        }
    }
}