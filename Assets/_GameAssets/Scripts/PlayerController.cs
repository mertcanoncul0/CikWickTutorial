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
    [SerializeField] private bool _canJump = true;

    [Header("Sliding Settings")]
    [SerializeField] private KeyCode _slideKey;
    [SerializeField] private float _slideMultiplier;
    [SerializeField] private float _slideDrag;

    [Header("Ground Check Settings")]
    [SerializeField] private float _playerHeight;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private float _groundDrag;

    private Rigidbody _playerRigibody;
    private float _horizontalInput, _verticalInput;
    private Vector3 _movementDirection;
    private bool _isSliding;


    private void Awake()
    {
        _playerRigibody = GetComponent<Rigidbody>();
        _playerRigibody.freezeRotation = true;
    }

    void Start()
    {

    }

    void Update()
    {
        SetInputs();
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

    private void SetPlayerMovement()
    {
        _movementDirection = _orientationTransform.forward * _verticalInput + _orientationTransform.right * _horizontalInput;

        if (_isSliding)
        {
            _playerRigibody.AddForce(_movementDirection.normalized * _movementSpeed * _slideMultiplier, ForceMode.Force);
        }
        else
        {
            _playerRigibody.AddForce(_movementDirection.normalized * _movementSpeed, ForceMode.Force);
        }
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
        if (_isSliding)
        {
            _playerRigibody.linearDamping = _slideDrag;
        }
        else
        {
            _playerRigibody.linearDamping = _groundDrag;

        }
    }

    private void ResetJumping()
    {
        _canJump = true;
    }

    private bool IsGrounded() => Physics.Raycast(transform.position, Vector3.down, _playerHeight * 0.5f + 0.2f, _groundLayer);
}
