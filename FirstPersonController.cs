using UnityEngine;
using Cinemachine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif
using System.Collections;

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class FirstPersonController : MonoBehaviour
    {
        [Header("Player Movement")]
        public float MoveSpeed = 4.0f;
        public float DashSpeed = 12.0f;
        public float RotationSpeed = 1.0f;
        public float SpeedChangeRate = 10.0f;
        public float DashDuration = 0.2f;
        public KeyCode DashKey = KeyCode.LeftShift;

        private float _speed;
        private bool _isDashing = false;

        [Header("Footsteps Settigns")]
        [SerializeField] private AudioClip[] footstepSounds;
        [SerializeField] private float footstepInterval = 0.4f;

        private float footstepTimer;
        [Header("Dash Cooldown")]
        public float DashCooldown = 1f;
        private float lastDashTime = -Mathf.Infinity;

        [Header("Jump Mechanics")]
        public float JumpHeight = 1.2f;
        public float Gravity = -15.0f;
        public float JumpTimeout = 0.1f;
        public float FallTimeout = 0.15f;

        [Header("Ground Check")]
        public bool Grounded = true;
        public float GroundedOffset = -0.14f;
        public float GroundedRadius = 0.5f;
        public LayerMask GroundLayers;

        [Header("Camera Settings")]
        public GameObject CinemachineCameraTarget;
        public float TopClamp = 90.0f;
        public float BottomClamp = -90.0f;

        private float _cinemachineTargetPitch;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;

        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;

#if ENABLE_INPUT_SYSTEM
        private PlayerInput _playerInput;
#endif
        private CharacterController _controller;
        private StarterAssetsInputs _input;
        private GameObject _mainCamera;

        [Header("Zoom Settings")]
        public float zoomFOV = 30f;
        public float zoomSpeed = 10f;
        private float originalFOV;
        private Camera playerCamera;
        private bool isZooming = false;

        private bool IsCurrentDeviceMouse => _playerInput?.currentControlScheme == "KeyboardMouse";
        private AudioSource audioSource;
        [SerializeField] private AudioSource DashSFX;

        [SerializeField] private CinemachineVirtualCamera virtualCamera;

        private void Awake()
        {
            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }
        }

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<StarterAssetsInputs>();
#if ENABLE_INPUT_SYSTEM
            _playerInput = GetComponent<PlayerInput>();
#endif
            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;
            audioSource = GetComponent<AudioSource>();

            if (virtualCamera != null)
            {
                virtualCamera.m_Lens.FieldOfView = 60f;
                originalFOV = 60f;
            }
        }

        private void Update()
        {
            GroundedCheck();
            Move();
            JumpAndGravity();
            HandleFootsteps();

            if (Input.GetKeyDown(DashKey) && Time.time >= lastDashTime + DashCooldown)
            {
                StartCoroutine(Dash());
                lastDashTime = Time.time;
            }

            if (Input.GetMouseButtonDown(1))
            {
                isZooming = true;
            }
            else if (Input.GetMouseButtonUp(1))
            {
                isZooming = false;
            }
            HandleZoom();
        }

        private void LateUpdate()
        {
            CameraRotation();
        }

        private void HandleZoom()
        {
            if (virtualCamera == null) return;

            float targetFOV = isZooming ? zoomFOV : originalFOV;
            virtualCamera.m_Lens.FieldOfView = Mathf.Lerp(
                virtualCamera.m_Lens.FieldOfView,
                targetFOV,
                Time.deltaTime * zoomSpeed
            );
        }

        private void GroundedCheck()
        {
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
        }

        private void CameraRotation()
        {
            if (_input.look.sqrMagnitude >= 0.01f)
            {
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;
                _cinemachineTargetPitch += _input.look.y * RotationSpeed * deltaTimeMultiplier;
                _rotationVelocity = _input.look.x * RotationSpeed * deltaTimeMultiplier;
                _cinemachineTargetPitch = Mathf.Clamp(_cinemachineTargetPitch, BottomClamp, TopClamp);
                CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f);
                transform.Rotate(Vector3.up * _rotationVelocity);
            }
        }

        private void Move()
        {
            if (_isDashing) return;

            float targetSpeed = MoveSpeed;
            if (_input.move == Vector2.zero) targetSpeed = 0.0f;

            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;
            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;
            _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);

            Vector3 inputDirection = transform.right * _input.move.x + transform.forward * _input.move.y;
            _controller.Move(inputDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
        }

        private void JumpAndGravity()
        {
            if (Grounded)
            {
                _fallTimeoutDelta = FallTimeout;

                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }

                if (_input.jump && _jumpTimeoutDelta <= 0.0f)
                {
                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
                }

                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else
            {
                _jumpTimeoutDelta = JumpTimeout;

                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }

                _input.jump = false;
            }

            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }
        }

        private IEnumerator Dash()
        {
            _isDashing = true;
            Vector3 dashDirection = transform.forward;
            float dashSpeed = DashSpeed;
            float elapsedTime = 0f;

            while (elapsedTime < DashDuration)
            {
                _controller.Move(dashDirection * dashSpeed * Time.deltaTime);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            if (DashSFX!= null)
            {
                DashSFX.Play();
            }

            _isDashing = false;
        }
        private void HandleFootsteps()
        {
            if (!Grounded || _input.move.magnitude < 0.1f || _isDashing)
            {
                footstepTimer = 0f;
                return;
            }

            footstepTimer -= Time.deltaTime;

            if (footstepTimer <= 0f)
            {
                PlayFootstep();

                // Adjust interval based on movement speed
                float adjustedInterval = _speed > MoveSpeed + 1f ? footstepInterval * 0.6f : footstepInterval;
                footstepTimer = adjustedInterval;  // ✅ Set timer here only ONCE
            }
        }


        private void PlayFootstep()
        {
            if (footstepSounds.Length == 0 || audioSource == null) return;

            int index = Random.Range(0, footstepSounds.Length);
            audioSource.PlayOneShot(footstepSounds[index]);
        }

    }
}