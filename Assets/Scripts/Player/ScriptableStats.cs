using UnityEngine;


[CreateAssetMenu]
public class ScriptableStats : ScriptableObject
{
    #region Input

    [Header("INPUT")]
    [Tooltip("Makes all Input snap to an integer. Prevents gamepads from walking slowly. Recommended value is true to ensure gamepad/keybaord parity.")]
    public bool SnapInput = true;

    [Tooltip("Minimum input required before you mount a ladder or climb a ledge. Avoids unwanted climbing using controllers"), Range(0.01f, 0.99f)]
    public float VerticalDeadZoneThreshold = 0.3f;

    [Tooltip("Minimum input required before a left or right is recognized. Avoids drifting with sticky controllers"), Range(0.01f, 0.99f)]
    public float HorizontalDeadZoneThreshold = 0.1f;

    #endregion


    #region Movement

    [Header("MOVEMENT")]
    [Tooltip("The top horizontal movement speed")]
    public float MaxSpeed = 14;

    [Tooltip("水平加速度")]
    public float Acceleration = 120;

    [Tooltip("地面阻力")]
    public float GroundDeceleration = 60;

    [Tooltip("空气阻力")]
    public float AirDeceleration = 30;

    [Tooltip("接地状态下的地面下压力，对斜坡有帮助"), Range(0f, -10f)]
    public float GroundingForce = -1.5f;

    [Tooltip("碰撞检测距离"), Range(0f, 0.5f)]
    public float GrounderDistance = 0.05f;

    #endregion


    #region Jump

    [Header("JUMP")]
    [Tooltip("The immediate velocity applied when jumping")]
    public float JumpPower = 36;

    [Tooltip("最大垂直移动速度")]
    public float MaxFallSpeed = 40;

    [Tooltip("重力加速度")]
    public float FallAcceleration = 110;

    [Tooltip("The gravity multiplier added when jump is released early")]
    public float JumpEndEarlyGravityModifier = 3;

    [Tooltip("The time before coyote jump becomes unusable. Coyote jump allows jump to execute even after leaving a ledge")]
    public float CoyoteTime = .15f;

    [Tooltip("The amount of time we buffer a jump. This allows jump input before actually hitting the ground")]
    public float JumpBuffer = .2f;

    #endregion


    #region Wall

    [Header("WALL")]
    [Tooltip("墙面检测距离"), Range(0f, 0.5f)]
    public float WallerDistance = 0.05f;

    [Tooltip("墙面滑行速度")]
    public float WallSlideSpeed = 5;

    [Tooltip("墙跳水平力度")]
    public float WallJumpPowerX = 20f;

    [Tooltip("墙跳垂直力度")]
    public float WallJumpPowerY = 25f;

    [Tooltip("离开墙面后还能跳跃的时间")]
    public float WallCoyoteTime = 0.15f;

    [Tooltip("墙跳输入缓冲时间")]
    public float WallJumpBuffer = 0.2f;

    [Tooltip("墙跳后禁用输入的时间")]
    public float WallJumpControlDisableTime = 0.2f;

    #endregion


    #region Dash

    [Header("DASH")]
    [Tooltip("冲刺初速度")]
    public float DashSpeed = 30f;

    [Tooltip("冲刺减速度")]
    public float DashDeceleration = 60f;

    [Tooltip("冲刺持续时间")]
    public float DashDuration = 0.2f;

    #endregion


    #region Bubble Generation

    [Header("BUBBLE GENERATION")]
    [Tooltip("生成泡泡的水平距离")]
    public float BubbleGenerateDistanceX = 2f;

    [Tooltip("生成泡泡的垂直距离")]
    public float BubbleGenerateDistanceY = 1f;

    [Tooltip("泡泡初始水平速度")]
    public float BubbleGenerateVelosityX = 5f;

    [Tooltip("泡泡初始垂直速度")]
    public float BubbleGenerateVelosityY = 3f;

    [Tooltip("长按生成泡泡所需时间")]
    public float LongPressThreshold = 0.5f;

    #endregion
}
