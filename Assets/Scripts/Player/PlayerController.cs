using System;
using UnityEngine;

namespace TarodevController
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public class PlayerController : MonoBehaviour, IPlayerController
    {
        [Header("Stats")]
        [SerializeField] private ScriptableStats outsideBubbleStats;  // 默认配置
        [SerializeField] private ScriptableStats insideBubbleStats; // 在 Bubble 中的配置

        [Header("Layer")]
        public LayerMask EntityLayer;   // 玩家可以交互的图层
        public LayerMask TriggerLayer;  // 用于触发的图层

        private ScriptableStats _stats;
        private Rigidbody2D _rb;
        private CapsuleCollider2D _col;
        private FrameInput _frameInput;
        private Vector2 _frameVelocity;
        private bool _cachedQueryStartInColliders;

        private PlayerState playerState = PlayerState.OutsideBubble;  // 角色状态

        #region Interface

        public Vector2 FrameInput => _frameInput.Move;
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

        private void GatherInput()
        {
            _frameInput = new FrameInput
            {
                JumpDown = Input.GetButtonDown("Jump"),
                JumpHeld = Input.GetButton("Jump"),
                Move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"))
            };

            if (_stats.SnapInput)
            {
                _frameInput.Move.x = Mathf.Abs(_frameInput.Move.x) < _stats.HorizontalDeadZoneThreshold ? 0 : Mathf.Sign(_frameInput.Move.x);
                _frameInput.Move.y = Mathf.Abs(_frameInput.Move.y) < _stats.VerticalDeadZoneThreshold ? 0 : Mathf.Sign(_frameInput.Move.y);
            }

            if (_frameInput.JumpDown)
            {
                _jumpToConsume = true;
                _timeJumpWasPressed = _time;
            }
        }

        private void FixedUpdate()
        {
            CheckCollisions();

            HandleJump();
            HandleDirection();
            HandleGravity();

            ApplyMovement();
        }

        #endregion


        #region Trigger

        // 进入 Trigger Layer 后，切换配置
        private void OnTriggerEnter2D(Collider2D other)
        {
            if ((TriggerLayer.value & (1 << other.gameObject.layer)) != 0)
            {
                playerState = PlayerState.InsideBubble;
                _stats = insideBubbleStats;
            }
        }

        // 离开 Trigger Layer 后，切换配置
        private void OnTriggerExit2D(Collider2D other)
        {
            if ((TriggerLayer.value & (1 << other.gameObject.layer)) != 0)
            {
                playerState = PlayerState.OutsideBubble;
                _stats = outsideBubbleStats;
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
            }
            else if (_isTouchingLeftWall && !leftWallHit)
            {
                _isTouchingLeftWall = false;
                _frameLeftWall = _time;
            }

            if (!_isTouchingRightWall && rightWallHit)
            {
                _isTouchingRightWall = true;
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

            if (!_jumpToConsume && !HasBufferedJump) return;

            if (_grounded || CanUseCoyote) ExecuteJump();

            _jumpToConsume = false;
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
            }
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


        #region Others

        private void ApplyMovement() => _rb.velocity = _frameVelocity;
    }

    #endregion


    #region Defines

    public struct FrameInput
    {
        public bool JumpDown;
        public bool JumpHeld;
        public Vector2 Move;
    }

    public interface IPlayerController
    {
        public event Action<bool, float> GroundedChanged;

        public event Action Jumped;
        public Vector2 FrameInput { get; }
    }

    public enum PlayerState
    {
        OutsideBubble,
        InsideBubble
    }

    #endregion
}