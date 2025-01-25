using UnityEngine;


/// <summary>
/// VERY primitive animator example.
/// </summary>
public class PlayerAnimator : MonoBehaviour
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

    [Header("Particles")][SerializeField] private ParticleSystem _jumpParticles;
    [SerializeField] private ParticleSystem _launchParticles;
    [SerializeField] private ParticleSystem _moveParticles;
    [SerializeField] private ParticleSystem _landParticles;

    [Header("Audio Clips")]
    [SerializeField]
    private AudioClip[] _footsteps;

    private AudioSource _source;
    private IPlayerController _player;
    private bool _grounded;
    private ParticleSystem.MinMaxGradient _currentGradient;

    // Animation parameter hashes - update these to match your Animator Controller parameters
    private readonly int _horizontalSpeedHash = Animator.StringToHash("HorizontalSpeed");
    private readonly int _verticalSpeedHash = Animator.StringToHash("VerticalSpeed");
    private readonly int _isGroundedHash = Animator.StringToHash("IsGrounded");
    private readonly int _idleSpeedHash = Animator.StringToHash("IdleSpeed");
    private readonly int _jumpHash = Animator.StringToHash("Jump");
    private readonly int _wallSlidingHash = Animator.StringToHash("WallSliding");

    private void Awake()
    {
        _source = GetComponent<AudioSource>();
        _player = GetComponentInParent<IPlayerController>();
    }

    private void OnEnable()
    {
        _player.Jumped += OnJumped;
        _player.GroundedChanged += OnGroundedChanged;

        _moveParticles.Play();
    }

    private void OnDisable()
    {
        _player.Jumped -= OnJumped;
        _player.GroundedChanged -= OnGroundedChanged;

        _moveParticles.Stop();
    }

    private void Update()
    {
        if (_player == null) return;

        DetectGroundColor();
        HandleSpriteFlipping();
        HandleAnimationState();
        HandleIdleSpeed();
        HandleCharacterTilt();
    }

    private void HandleSpriteFlipping()
    {
        //if (_player.FrameInput.x != 0)
        //{
        //    GameManager.Instance.GetPlayer().GetComponent<PlayerController>().transform.Find("Visual").Find("Sprite").GetComponent<SpriteRenderer>().flipX = x_Input < 0;

        //    Debug.Log("test2:" + _sprite.flipX);
        //}

    }

    private void HandleIdleSpeed()
    {
        var inputStrength = Mathf.Abs(_player.FrameInput.x);
        _anim.SetFloat(_idleSpeedHash, Mathf.Lerp(1, _maxIdleSpeed, inputStrength));
        _moveParticles.transform.localScale = Vector3.MoveTowards(_moveParticles.transform.localScale, Vector3.one * inputStrength, 2 * Time.deltaTime);
    }

    private void HandleCharacterTilt()
    {
        var runningTilt = _grounded ? Quaternion.Euler(0, 0, _maxTilt * _player.FrameInput.x) : Quaternion.identity;
        _anim.transform.up = Vector3.RotateTowards(_anim.transform.up, runningTilt * Vector2.up, _tiltSpeed * Time.deltaTime, 0f);
    }

    private void HandleAnimationState()
    {
        // Set horizontal and vertical speed parameters
        _anim.SetFloat(_horizontalSpeedHash, Mathf.Abs(_player.Velocity.x));
        _anim.SetFloat(_verticalSpeedHash, _player.Velocity.y);
        
        // Set grounded state
        _anim.SetBool(_isGroundedHash, _grounded);

        // Check if wall sliding (requires accessing player controller component)
        var controller = transform.parent.GetComponent<PlayerController>();
        if (controller != null)
        {
            var isWallSliding = !_grounded && (
                controller.GetIsTouchingLeftWall() && _player.FrameInput.x < 0 ||
                controller.GetIsTouchingRightWall() && _player.FrameInput.x > 0
            );
            _anim.SetBool(_wallSlidingHash, isWallSliding);
        }
    }

    private void OnJumped()
    {
        _anim.SetTrigger(_jumpHash);
        _anim.ResetTrigger(_isGroundedHash);

        if (_grounded) // Avoid coyote
        {
            SetColor(_jumpParticles);
            SetColor(_launchParticles);
            _jumpParticles.Play();
        }
    }

    private void OnGroundedChanged(bool grounded, float impact)
    {
        _grounded = grounded;

        if (grounded)
        {
            DetectGroundColor();
            SetColor(_landParticles);

            _anim.SetTrigger(_isGroundedHash);
            _source.PlayOneShot(_footsteps[Random.Range(0, _footsteps.Length)]);
            _moveParticles.Play();

            _landParticles.transform.localScale = Vector3.one * Mathf.InverseLerp(0, 40, impact);
            _landParticles.Play();
        }
        else
        {
            _moveParticles.Stop();
        }
    }

    private void DetectGroundColor()
    {
        var hit = Physics2D.Raycast(transform.position, Vector3.down, 2);

        if (!hit || hit.collider.isTrigger || !hit.transform.TryGetComponent(out SpriteRenderer r)) return;
        var color = r.color;
        _currentGradient = new ParticleSystem.MinMaxGradient(color * 0.9f, color * 1.2f);
        SetColor(_moveParticles);
    }

    private void SetColor(ParticleSystem ps)
    {
        var main = ps.main;
        main.startColor = _currentGradient;
    }
}
