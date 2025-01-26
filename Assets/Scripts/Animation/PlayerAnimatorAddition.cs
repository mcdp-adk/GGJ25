using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Windows;
using static Unity.VisualScripting.Member;
//using static UnityEditor.Experimental.GraphView.GraphView;

public class PlayerAnimatorAddition : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private Animator _anim;

    [SerializeField] private SpriteRenderer _sprite;

    [Header("Settings")]
    [SerializeField, Range(1f, 3f)]
    private float _maxIdleSpeed = 2;

    [SerializeField] private float _maxTilt = 5;
    [SerializeField] private float _tiltSpeed = 20;

    [Header("Audio Clips")]
    [SerializeField]
    private AudioClip[] _footsteps;

    [Header("Statue")]
    Dictionary<string, object> playerStateDict;
    public Vector2 frameVelocity;
    public Vector2 inputVelocity;
    private float x_Speed = 0f;
    private float y_Speed = 0f;
    private float x_Input = 0f;
    private float y_Input = 0f;
    private bool isGrounded = false;  // 是否接触地面
    private bool isInBubble = false;  // 是否在球内
    private bool isDashStateActive = false;
    private bool isLeftWallContactActive = false;
    private bool isRightWallContactActive = false;

    private AudioSource _source;
    private IPlayerController _player;

    private bool _grounded;
    private ParticleSystem.MinMaxGradient _currentGradient;

    private void Awake()
    {
        _source = GetComponent<AudioSource>();
        _player = GetComponentInParent<IPlayerController>();
    }

    private void OnEnable()
    {
        _player.Jumped += OnJumped;

    }

    private void OnDisable()
    {
        _player.Jumped -= OnJumped;

    }

    private void Update()
    {
        //if (_player == null) return;

        Dictionary<string, object> playerStateDict = GameManager.Instance.GetPlayer().GetComponent<PlayerController>().GetPlayerStateDictionary();

        x_Speed = (float)playerStateDict["x_Speed"];
        y_Speed = (float)playerStateDict["y_Speed"];
        x_Input = (float)playerStateDict["x_Input"];
        y_Input = (float)playerStateDict["y_Input"];
        isGrounded = (bool)playerStateDict["IsGrounded"];
        isInBubble = (bool)playerStateDict["IsInBubble"];
        isDashStateActive = (bool)playerStateDict["IsDashing"];
        isLeftWallContactActive = (bool)playerStateDict["IsTouchingLeftWall"];
        isRightWallContactActive = (bool)playerStateDict["IsTouchingRightWall"];

        // 你可以在这里使用这些变量
        HandleSpriteFlip();
        UpdateAnimation();
    }

    private void UpdateAnimation()
    {
        _anim.SetFloat(xSpeed, x_Speed);
        _anim.SetFloat(ySpeed, y_Speed);
        _anim.SetFloat(xInput, x_Input);
        _anim.SetFloat(yInput, y_Input);
        _anim.SetBool(grounded, isGrounded);
        _anim.SetBool(isInTheBubble, isInBubble);
        _anim.SetBool(dashStateActive, isDashStateActive);
        _anim.SetBool(leftWallContactActive, isLeftWallContactActive);
        _anim.SetBool(rightWallContactActive, isRightWallContactActive);
    }

    private void HandleSpriteFlip()
    {
        if (x_Input != 0)
        {
            Debug.Log("test:" + _sprite.flipX);
            //GameManager.Instance.GetPlayer().GetComponent<PlayerController>().transform.Find("Visual").Find("Sprite").GetComponent<SpriteRenderer>().flipX = x_Input < 0;
        }
    }

    private void OnJumped()
    {
        _anim.SetTrigger(JumpKey);  // 传输信号

        // 在下一帧重置触发器
        StartCoroutine(ResetJumpTrigger());
    }

    private IEnumerator ResetJumpTrigger()
    {
        yield return null;  // 等待一帧
        _anim.ResetTrigger(JumpKey);
    }

    private static readonly int xSpeed = Animator.StringToHash("X_Speed");
    private static readonly int ySpeed = Animator.StringToHash("Y_Speed");
    private static readonly int xInput = Animator.StringToHash("X_Input");
    private static readonly int yInput = Animator.StringToHash("Y_Input");
    private static readonly int grounded = Animator.StringToHash("Is_Grounded");
    private static readonly int isInTheBubble = Animator.StringToHash("Is_In_Bubble");
    private static readonly int dashStateActive = Animator.StringToHash("DashStateActive");
    private static readonly int leftWallContactActive = Animator.StringToHash("LeftWallContactActive");
    private static readonly int rightWallContactActive = Animator.StringToHash("RightWallContactActive");
    private static readonly int JumpKey = Animator.StringToHash("JumpTrigger");
}
