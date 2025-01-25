using UnityEngine;

public class BubbleController : MonoBehaviour
{
    [SerializeField] private LayerMask DeathLayer;    // 碰撞后销毁的图层
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
