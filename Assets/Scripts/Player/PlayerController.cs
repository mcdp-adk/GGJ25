using System;
using Unity.VisualScripting;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class PlayerController : MonoBehaviour, IPlayerController
{
    [Header("Stats")]
    [SerializeField] private ScriptableStats outsideBubbleStats;  // 默认配置
    [SerializeField] private ScriptableStats insideBubbleStats; // 在 Bubble 中的配置

    [Header("Layer")]
    [SerializeField] private LayerMask EntityLayer;   // 玩家可以交互的图层
    [SerializeField] private LayerMask TriggerLayer;  // 用于触发的图层
    [SerializeField] private LayerMask DeathLayer;    // 碰撞后 Player 死亡的图层

    [Header("Bubble")]
    [SerializeField] private GameObject bubblePrefab;

    private ScriptableStats _stats;
    private Rigidbody2D _rb;
    private CapsuleCollider2D _col;
    private FrameInput _frameInput;
    private Vector2 _frameVelocity;
    private bool _cachedQueryStartInColliders;

    private PlayerState playerState = PlayerState.OutsideBubble;  // 角色状态

    private float _wallJumpStartTime;
    private bool _wallJumpToConsume;
    private bool _wallCoyoteUsable;

    private bool _isDashing;
    private float _dashStartTime;
    private Vector2 _dashDirection;

    private float _bubbleKeyPressStartTime;
    private bool _isBubbleKeyPressed;

    public int _currentBubbleCount = 0;
    private GameObject _currentBubble;  // Track current bubble player is in

    private bool _isFacingRight = true;  // 新增朝向变量

    #region Interface

    public Vector2 FrameInput => _frameInput.Move;
    public Vector2 Velocity => _rb.velocity;
    public event Action<bool, float> GroundedChanged;
    public event Action Jumped;

    #endregion


    #region Unity Callbacks

    private float _time;

    private void Awake()
    {
        _stats = outsideBubbleStats;

        _rb = GetComponent<Rigidbody2D>();
        _col = GetComponent<CapsuleCollider2D>();

        _cachedQueryStartInColliders = Physics2D.queriesStartInColliders;
    }

    private void Update()
    {
        _time += Time.deltaTime;
        GatherInput();
    }

    #region Input

    private void GatherInput()
    {
        bool dashPressed = Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.K);

        // Only gather normal input if not dashing
        bool inputEnabled = !_isDashing && _time > _wallJumpStartTime + _stats.WallJumpControlDisableTime;

        _frameInput = new FrameInput
        {
            JumpDown = Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.J),
            JumpHeld = Input.GetButton("Jump") || Input.GetKey(KeyCode.C) || Input.GetKey(KeyCode.J),
            Move = inputEnabled ? new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")) : Vector2.zero
        };

        // Handle dash input
        if (dashPressed && playerState == PlayerState.InsideBubble && !_isDashing)
        {
            StartDash();
        }

        if (_stats.SnapInput)
        {
            _frameInput.Move.x = Mathf.Abs(_frameInput.Move.x) < _stats.HorizontalDeadZoneThreshold ? 0 : Mathf.Sign(_frameInput.Move.x);
            _frameInput.Move.y = Mathf.Abs(_frameInput.Move.y) < _stats.VerticalDeadZoneThreshold ? 0 : Mathf.Sign(_frameInput.Move.y);
        }

        if (_frameInput.JumpDown)
        {
            _jumpToConsume = true;
            _wallJumpToConsume = true;
            _timeJumpWasPressed = _time;
        }

        // Handle bubble destruction input when inside bubble
        bool bubbleKeyDown = Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.L);
        if (bubbleKeyDown && playerState == PlayerState.InsideBubble && _currentBubble != null)
        {
            GameManager.Instance.DestroyGameObject(_currentBubble);
            _currentBubbleCount = Mathf.Max(0, _currentBubbleCount - 1);
            _currentBubble = null;
            return;
        }

        // Only handle bubble generation when outside bubble
        if (playerState != PlayerState.OutsideBubble) return;

        bool bubbleKeyUp = Input.GetKeyUp(KeyCode.Z) || Input.GetKeyUp(KeyCode.L);
        bool bubbleKeyHeld = Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.L);

        if (bubbleKeyDown)
        {
            _bubbleKeyPressStartTime = _time;
            _isBubbleKeyPressed = true;
        }
        else if (bubbleKeyUp && _isBubbleKeyPressed)
        {
            float pressDuration = _time - _bubbleKeyPressStartTime;
            if (pressDuration < _stats.LongPressThreshold)
            {
                GenerateShortPressBubble();
            }
            _isBubbleKeyPressed = false;
        }

        // Check for long press
        if (_isBubbleKeyPressed && bubbleKeyHeld)
        {
            float pressDuration = _time - _bubbleKeyPressStartTime;
            if (pressDuration >= _stats.LongPressThreshold)
            {
                GenerateLongPressBubble();
                _isBubbleKeyPressed = false;
            }
        }
    }

    #endregion


    #region Dash

    private void StartDash()
    {
        _isDashing = true;
        _dashStartTime = _time;

        // Normalize input direction for diagonal dash
        _dashDirection = _frameInput.Move.normalized;

        // If no direction is pressed, dash in facing direction
        if (_dashDirection == Vector2.zero)
        {
            _dashDirection = Vector2.right * Mathf.Sign(transform.localScale.x);
        }

        // Set initial dash velocity
        _frameVelocity = _dashDirection * _stats.DashSpeed;
    }

    private void HandleDash()
    {
        if (!_isDashing) return;

        if (_time > _dashStartTime + _stats.DashDuration)
        {
            _isDashing = false;
            return;
        }

        // Apply deceleration during dash
        float dashSpeedMultiplier = 1 - ((_time - _dashStartTime) / _stats.DashDuration);
        _frameVelocity = _dashDirection * (_stats.DashSpeed * dashSpeedMultiplier);
    }

    #endregion

    private void FixedUpdate()
    {
        CheckCollisions();

        if (_isDashing)
        {
            HandleDash();
        }
        else
        {
            HandleJump();
            HandleDirection();
            HandleGravity();
        }

        ApplyMovement();
    }

    #endregion


    #region Trigger

    // 进入 Trigger 的逻辑，根据 Layer 检测
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check for death layer collision
        if ((DeathLayer.value & (1 << other.gameObject.layer)) != 0)
        {
            GameManager.Instance.RespawnPlayer();
            return;
        }

        if ((TriggerLayer.value & (1 << other.gameObject.layer)) != 0)
        {
            playerState = PlayerState.InsideBubble;
            _stats = insideBubbleStats;
            _currentBubble = other.gameObject;  // Store reference to current bubble
        }
    }

    // 离开 Trigger 时的逻辑
    private void OnTriggerExit2D(Collider2D other)
    {
        if ((TriggerLayer.value & (1 << other.gameObject.layer)) != 0)
        {
            playerState = PlayerState.OutsideBubble;
            _stats = outsideBubbleStats;
            _currentBubble = null;  // Clear reference when exiting
        }
    }

    #endregion


    #region Collisions

    private float _frameLeftGrounded = float.MinValue;
    private float _frameLeftWall = float.MinValue;
    private bool _grounded;
    private bool _isTouchingLeftWall;
    private bool _isTouchingRightWall;

    private void CheckCollisions()
    {
        Physics2D.queriesStartInColliders = false;

        // Ground and Ceiling
        bool groundHit = Physics2D.CapsuleCast(_col.bounds.center, _col.size, _col.direction, 0, Vector2.down, _stats.GrounderDistance, EntityLayer);
        bool ceilingHit = Physics2D.CapsuleCast(_col.bounds.center, _col.size, _col.direction, 0, Vector2.up, _stats.GrounderDistance, EntityLayer);

        // 墙面检测
        bool leftWallHit = Physics2D.CapsuleCast(_col.bounds.center, _col.size, _col.direction, 0, Vector2.left, _stats.WallerDistance, EntityLayer);
        bool rightWallHit = Physics2D.CapsuleCast(_col.bounds.center, _col.size, _col.direction, 0, Vector2.right, _stats.WallerDistance, EntityLayer);

        // Hit a Ceiling
        if (ceilingHit) _frameVelocity.y = Mathf.Min(0, _frameVelocity.y);

        // 处理墙面碰撞
        if (!_isTouchingLeftWall && leftWallHit)
        {
            _isTouchingLeftWall = true;
            _wallCoyoteUsable = true;
        }
        else if (_isTouchingLeftWall && !leftWallHit)
        {
            _isTouchingLeftWall = false;
            _frameLeftWall = _time;
        }

        if (!_isTouchingRightWall && rightWallHit)
        {
            _isTouchingRightWall = true;
            _wallCoyoteUsable = true;
        }
        else if (_isTouchingRightWall && !rightWallHit)
        {
            _isTouchingRightWall = false;
            _frameLeftWall = _time;
        }

        // Landed on the Ground
        if (!_grounded && groundHit)
        {
            _grounded = true;
            _coyoteUsable = true;
            _bufferedJumpUsable = true;
            _endedJumpEarly = false;
            GroundedChanged?.Invoke(true, Mathf.Abs(_frameVelocity.y));
        }
        // Left the Ground
        else if (_grounded && !groundHit)
        {
            _grounded = false;
            _frameLeftGrounded = _time;
            GroundedChanged?.Invoke(false, 0);
        }

        Physics2D.queriesStartInColliders = _cachedQueryStartInColliders;
    }

    #endregion


    #region Jumpimg

    private bool _jumpToConsume;
    private bool _bufferedJumpUsable;
    private bool _endedJumpEarly;
    private bool _coyoteUsable;
    private float _timeJumpWasPressed;

    private bool HasBufferedJump => _bufferedJumpUsable && _time < _timeJumpWasPressed + _stats.JumpBuffer;
    private bool CanUseCoyote => _coyoteUsable && !_grounded && _time < _frameLeftGrounded + _stats.CoyoteTime;

    private void HandleJump()
    {
        if (!_endedJumpEarly && !_grounded && !_frameInput.JumpHeld && _rb.velocity.y > 0) _endedJumpEarly = true;

        // 先处理普通跳跃
        if (_jumpToConsume || HasBufferedJump)
        {
            if (_grounded || CanUseCoyote)
            {
                ExecuteJump();
                _jumpToConsume = false;
                _wallJumpToConsume = false;
                return;
            }
        }

        // 再处理墙跳
        if (_wallJumpToConsume && CanWallJump())
        {
            ExecuteWallJump();
            _wallJumpToConsume = false;
            _jumpToConsume = false;
            return;
        }

        _jumpToConsume = false;
    }

    private bool CanWallJump()
    {
        bool hasWallJumpBuffer = _time < _timeJumpWasPressed + _stats.WallJumpBuffer;
        bool canUseWallCoyote = _wallCoyoteUsable && _time < _frameLeftWall + _stats.WallCoyoteTime;
        return hasWallJumpBuffer && ((_isTouchingLeftWall || _isTouchingRightWall) || canUseWallCoyote);
    }

    private void ExecuteWallJump()
    {
        _wallJumpStartTime = _time;
        _endedJumpEarly = false;
        _wallCoyoteUsable = false;

        float directionX = _isTouchingLeftWall ? 1 : -1;
        _frameVelocity.x = directionX * _stats.WallJumpPowerX;
        _frameVelocity.y = _stats.WallJumpPowerY;

        Jumped?.Invoke();
    }

    private void ExecuteJump()
    {
        _endedJumpEarly = false;
        _timeJumpWasPressed = 0;
        _bufferedJumpUsable = false;
        _coyoteUsable = false;
        _frameVelocity.y = _stats.JumpPower;
        Jumped?.Invoke();
    }

    #endregion

    #region  Horizonal

    private void HandleDirection()
    {
        if (_frameInput.Move.x == 0)
        {
            var deceleration = _grounded ? _stats.GroundDeceleration : _stats.AirDeceleration;
            _frameVelocity.x = Mathf.MoveTowards(_frameVelocity.x, 0, deceleration * Time.fixedDeltaTime);
        }
        else
        {
            _frameVelocity.x = Mathf.MoveTowards(_frameVelocity.x, _frameInput.Move.x * _stats.MaxSpeed, _stats.Acceleration * Time.fixedDeltaTime);
            // 更新朝向
            if (_frameInput.Move.x > 0 && !_isFacingRight)
            {
                Flip();
            }
            else if (_frameInput.Move.x < 0 && _isFacingRight)
            {
                Flip();
            }
        }
    }

    private void Flip()
    {
        _isFacingRight = !_isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    #endregion


    #region Gravity

    private void HandleGravity()
    {
        if (_grounded && _frameVelocity.y <= 0f)
        {
            _frameVelocity.y = _stats.GroundingForce;
        }
        // Add wall sliding logic
        else if (IsWallSliding() && _frameVelocity.y < 0)
        {
            _frameVelocity.y = -_stats.WallSlideSpeed;
        }
        else
        {
            var inAirGravity = _stats.FallAcceleration;
            if (_endedJumpEarly && _frameVelocity.y > 0) inAirGravity *= _stats.JumpEndEarlyGravityModifier;
            _frameVelocity.y = Mathf.MoveTowards(_frameVelocity.y, -_stats.MaxFallSpeed, inAirGravity * Time.fixedDeltaTime);
        }
    }

    private bool IsWallSliding()
    {
        return !_grounded && (
            (_isTouchingLeftWall && _frameInput.Move.x < 0) ||
            (_isTouchingRightWall && _frameInput.Move.x > 0)
        );
    }

    #endregion


    #region Public Methods

    public PlayerState GetPlayerState()
    {
        return playerState;
    }

    public bool GetIsInBubble()
    {
        return playerState == PlayerState.InsideBubble;
    }

    public bool GetIsGrounded()
    {
        return _grounded;
    }

    public bool GetIsTouchingLeftWall()
    {
        return _isTouchingLeftWall;
    }

    public bool GetIsTouchingRightWall()
    {
        return _isTouchingRightWall;
    }

    public bool GetIsDashing()
    {
        return _isDashing;
    }

    public Vector2 GetFrameInput()
    {
        return _frameInput.Move;
    }

    public Vector2 GetFrameVelocity()
    {
        return _frameVelocity;
    }

    #endregion


    #region Others

    private void ApplyMovement() => _rb.velocity = _frameVelocity;

private void GenerateShortPressBubble()
{
    if (bubblePrefab == null || _currentBubbleCount >= _stats.MaxBubbleCount) return;

    float direction = _isFacingRight ? 1f : -1f;
    Vector3 spawnPosition = transform.position + new Vector3(
        direction * _stats.BubbleGenerateDistanceX,
        _stats.BubbleGenerateDistanceY,
        0
    );

    GameObject bubble = Instantiate(bubblePrefab, spawnPosition, Quaternion.identity);
    
    // 设置气泡的tag
    bubble.tag = "GeneratedBubble";

    BubbleController controller = bubble.GetComponent<BubbleController>();
    
    if (controller != null)
    {
        controller.initialPlayerState(PlayerState.OutsideBubble);
        
        Vector2 bubbleVelocity = new Vector2(
            direction * _stats.BubbleGenerateVelosityX + _frameVelocity.x * 0.5f,
            _stats.BubbleGenerateVelosityY + _frameVelocity.y * 0.3f
        );
        controller.initialBubbleVelosity(bubbleVelocity);
        
        _currentBubbleCount++;
        StartCoroutine(DestroyBubbleAfterDelay(bubble));
    }
}

private void GenerateLongPressBubble()
{
    if (bubblePrefab == null || _currentBubbleCount >= _stats.MaxBubbleCount) return;

    Vector3 spawnPosition = transform.position + new Vector3(0, _stats.BubbleGenerateDistanceY, 0);
    GameObject bubble = Instantiate(bubblePrefab, spawnPosition, Quaternion.identity);
    
    // 设置气泡的tag
    bubble.tag = "GeneratedBubble";

    BubbleController controller = bubble.GetComponent<BubbleController>();
    if (controller != null)
    {
        controller.initialPlayerState(PlayerState.InsideBubble);
        _currentBubbleCount++;
        StartCoroutine(DestroyBubbleAfterDelay(bubble));
    }
}

    private System.Collections.IEnumerator DestroyBubbleAfterDelay(GameObject bubble)
    {
        yield return new WaitForSeconds(_stats.BubbleLifetime);
        if (bubble != null)
        {
            GameManager.Instance.DestroyGameObject(bubble);
            //_currentBubbleCount = Mathf.Max(0, _currentBubbleCount - 1);
           
        }
        _currentBubbleCount = GameObject.FindGameObjectsWithTag("GeneratedBubble").Length;
    }
}

#endregion


#region Defines

public struct FrameInput
{
    public bool JumpDown;
    public bool JumpHeld;
    public Vector2 Move;
}

public enum PlayerState
{
    OutsideBubble,
    InsideBubble
}

#endregion
