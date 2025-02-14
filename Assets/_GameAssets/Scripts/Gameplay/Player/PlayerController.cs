using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _orientationTransform;

    [Header("Movement Settings")]
    [SerializeField] private KeyCode _movementKey;
    [SerializeField] private float _movementSpeed = 30f;

    [Header("Jump Settings")]
    [SerializeField] private KeyCode _jumpKey;
    [SerializeField] private float _jumpForce;
    [SerializeField] private float _jumpCooldown;
    [SerializeField] private float _airMultiplier;
    [SerializeField] private float _airDrag;
    [SerializeField] private bool _canJump = true;

    [Header("Sliding Settings")]
    [SerializeField] private KeyCode _slideKey;
    [SerializeField] private float _slideMultiplier;
    [SerializeField] private float _slideDrag;

    [Header("Ground Check Settings")]
    [SerializeField] private float _playerHeight;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private float _groundDrag;

    private StateController _stateController;
    private Rigidbody _playerRigibody;
    private float _horizontalInput, _verticalInput;
    private Vector3 _movementDirection;
    private bool _isSliding;


    private void Awake()
    {
        _stateController = GetComponent<StateController>();
        _playerRigibody = GetComponent<Rigidbody>();
        _playerRigibody.freezeRotation = true;
    }

    void Start()
    {

    }

    void Update()
    {
        SetInputs();
        SetStates();
        SetPlayerDrag();
        LimitPlayerSpeed();
    }

    void FixedUpdate()
    {
        SetPlayerMovement();
    }

    private void SetInputs()
    {
        _horizontalInput = Input.GetAxisRaw("Horizontal");
        _verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(_slideKey))
        {
            _isSliding = true;
        }
        else if (Input.GetKeyDown(_movementKey))
        {
            _isSliding = false;
        }
        else if (Input.GetKey(_jumpKey) && _canJump && IsGrounded())
        {
            _canJump = false;
            SetPlayerJumping();
            Invoke(nameof(ResetJumping), _jumpCooldown);
        }
    }

    private void SetStates()
    {
        Vector3 movementDirection = GetMovementDirection();
        PlayerState currentState = _stateController.GetCurrentState();
        bool isGrounded = IsGrounded();

        var newState = currentState switch
        {
            _ when movementDirection == Vector3.zero && isGrounded && !_isSliding => PlayerState.Idle,
            _ when movementDirection != Vector3.zero && isGrounded && !_isSliding => PlayerState.Move,
            _ when movementDirection != Vector3.zero && isGrounded && _isSliding => PlayerState.Slide,
            _ when movementDirection == Vector3.zero && isGrounded && _isSliding => PlayerState.SlideIdle,
            _ when !_canJump && !isGrounded => PlayerState.Jump,
            _ => currentState
        };

        if (newState != currentState)
        {
            _stateController.ChangeState(newState);
        }

        Debug.Log(newState);
    }

    private void SetPlayerMovement()
    {
        _movementDirection = _orientationTransform.forward * _verticalInput + _orientationTransform.right * _horizontalInput;

        float forceMultiplier = _stateController.GetCurrentState() switch
        {
            PlayerState.Move => 1f,
            PlayerState.Slide => _slideMultiplier,
            PlayerState.Jump => _airMultiplier,
            _ => 1f
        };

        _playerRigibody.AddForce(_movementDirection.normalized * _movementSpeed * forceMultiplier, ForceMode.Force);
    }

    private void SetPlayerJumping()
    {
        _playerRigibody.linearVelocity = new Vector3(_playerRigibody.linearVelocity.x, 0f, _playerRigibody.linearVelocity.z);
        _playerRigibody.AddForce(transform.up * _jumpForce, ForceMode.Impulse);
    }

    private void LimitPlayerSpeed()
    {
        Vector3 flatVelocity = new Vector3(_playerRigibody.linearVelocity.x, 0f, _playerRigibody.linearVelocity.z);

        if (flatVelocity.magnitude > _movementSpeed)
        {
            Vector3 limitedVelocity = flatVelocity.normalized * _movementSpeed;
            _playerRigibody.linearVelocity = new Vector3(limitedVelocity.x, _playerRigibody.linearVelocity.y, limitedVelocity.z);
        }
    }

    private void SetPlayerDrag()
    {
        _playerRigibody.linearDamping = _stateController.GetCurrentState() switch
        {
            PlayerState.Move => _groundDrag,
            PlayerState.Slide => _slideDrag,
            PlayerState.Jump => _airDrag,
            _ => _playerRigibody.linearDamping
        };
    }

    private void ResetJumping()
    {
        _canJump = true;
    }

    private bool IsGrounded() => Physics.Raycast(transform.position, Vector3.down, _playerHeight * 0.5f + 0.2f, _groundLayer);

    private Vector3 GetMovementDirection() => _movementDirection.normalized;
}
