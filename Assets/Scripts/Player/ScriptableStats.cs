using UnityEngine;

namespace TarodevController
{
    [CreateAssetMenu]
    public class ScriptableStats : ScriptableObject
    {
        #region Input

        [Header("INPUT")] [Tooltip("Makes all Input snap to an integer. Prevents gamepads from walking slowly. Recommended value is true to ensure gamepad/keybaord parity.")]
        public bool SnapInput = true;

        [Tooltip("Minimum input required before you mount a ladder or climb a ledge. Avoids unwanted climbing using controllers"), Range(0.01f, 0.99f)]
        public float VerticalDeadZoneThreshold = 0.3f;

        [Tooltip("Minimum input required before a left or right is recognized. Avoids drifting with sticky controllers"), Range(0.01f, 0.99f)]
        public float HorizontalDeadZoneThreshold = 0.1f;

        #endregion


        #region Movement

        [Header("MOVEMENT")] [Tooltip("The top horizontal movement speed")]
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

        [Header("JUMP")] [Tooltip("The immediate velocity applied when jumping")]
        public float JumpPower = 36;

        [Tooltip("最大垂直移动速度")]
        public float MaxFallSpeed = 40;

        [Tooltip("重力加速度")]
        public float FallAcceleration = 110;

        [Tooltip("墙面滑行速度")]
        public float WallSlideSpeed = 10;

        [Tooltip("The gravity multiplier added when jump is released early")]
        public float JumpEndEarlyGravityModifier = 3;

        [Tooltip("The time before coyote jump becomes unusable. Coyote jump allows jump to execute even after leaving a ledge")]
        public float CoyoteTime = .15f;

        [Tooltip("The amount of time we buffer a jump. This allows jump input before actually hitting the ground")]
        public float JumpBuffer = .2f;

        [Tooltip("墙面检测距离"), Range(0f, 0.5f)]
        public float WallerDistance = 0.05f;

        #endregion
    }
}