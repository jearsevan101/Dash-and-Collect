using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject player;
    [SerializeField] private Transform cameraTransform;
    private CharacterController controller;
    private Animator animator;

    [Header("Player Stats Reference")]
    [SerializeField] private PlayerStatsSO playerStats;

    private float gravity = -9.81f;
    private Vector3 velocity;
    private bool isGrounded;
    private float xRotation = 0f;

    private bool isDashing = false;
    private bool dashAvailable = true;

    private float dashTime;
    private float cooldownRemaining;

    private void OnEnable()
    {
        GameEvents.OnGamePaused += HandlePause;
    }

    private void OnDisable()
    {
        GameEvents.OnGamePaused -= HandlePause;
    }

    void Start()
    {
        if (player == null)
        {
            Debug.LogError("Player reference not set in PlayerMovement!");
            enabled = false;
            return;
        }

        controller = player.GetComponent<CharacterController>();
        if (controller == null)
        {
            Debug.LogError("Player object needs a CharacterController component!");
            enabled = false;
            return;
        }

        animator = player.GetComponentInChildren<Animator>();
        if (animator == null)
        {
            Debug.LogWarning("No Animator found in Player children!");
        }

        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (GameManager.Instance.IsGameStopped) return;

        HandleMouseLook();

        if (isDashing)
            HandleDash();
        else
            HandleMovement();

        HandleDashInput();
        HandleDashCooldown();
    }

    private void HandlePause(bool isPaused)
    {
        Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;
    }
    private void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * playerStats.mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * playerStats.mouseSensitivity * Time.deltaTime;

        player.transform.Rotate(Vector3.up * mouseX);

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    private void HandleMovement()
    {
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = player.transform.right * x + player.transform.forward * z;
        controller.Move(move * playerStats.moveSpeed * Time.deltaTime);

        bool isMoving = move.magnitude > 0.1f;
        if (animator != null)
        {
            animator.SetBool("IsRunning", isMoving);
        }

        if (Input.GetButtonDown("Jump") && isGrounded)
            velocity.y = Mathf.Sqrt(playerStats.jumpHeight * -2f * gravity);

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void HandleDashInput()
    {
        if (Input.GetKeyDown(KeyCode.E) && dashAvailable && !isDashing)
        {
            GameEvents.PlaySFX(SFXType.PlayerDash);

            isDashing = true;
            dashAvailable = false;

            dashTime = Time.time + playerStats.dashDuration;
            cooldownRemaining = playerStats.dashCooldown;

            // Fire cooldown start event
            GameEvents.DashCooldownChanged(cooldownRemaining, playerStats.dashCooldown);

            Vector3 dashDirection = player.transform.forward;
            velocity = dashDirection * playerStats.dashSpeed;
        }
    }

    private void HandleDash()
    {
        controller.Move(velocity * Time.deltaTime);

        if (Time.time >= dashTime)
        {
            isDashing = false;
            velocity = Vector3.zero;
        }
    }

    private void HandleDashCooldown()
    {
        if (!dashAvailable)
        {
            cooldownRemaining -= Time.deltaTime;

            // Fire update event
            GameEvents.DashCooldownChanged(Mathf.Max(0, cooldownRemaining), playerStats.dashCooldown);

            if (cooldownRemaining <= 0f)
            {
                dashAvailable = true;
                cooldownRemaining = 0f;

                // Fire final event (ready again)
                GameEvents.DashCooldownChanged(0, playerStats.dashCooldown);
            }
        }
    }
}
