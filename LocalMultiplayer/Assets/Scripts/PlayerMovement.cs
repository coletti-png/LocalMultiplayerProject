using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerClass))]
[RequireComponent(typeof(PlayerStats))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Look Settings")]
    [SerializeField] private float mouseSensitivity = 150f;
    [SerializeField] private Transform cameraTransform;

    [Header("Jump Settings")]
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float gravity = -9.81f;

    [Header("Shooting Settings")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;

    [Header("UI")]
    [SerializeField] private TMP_Text ammoText;
    [SerializeField] private TMP_Text classText;
    [SerializeField] private TMP_Text weaponText;

    private CharacterController controller;
    private PlayerStats playerStats;
    private PlayerClass playerClass;

    private Vector2 moveInput;
    private Vector2 lookInput;
    private float xRotation = 0f;

    private Vector3 velocity;
    private bool isGrounded;

    private float shotCooldownTimer = 0f;
    private bool isReloading = false;
    private float reloadTimer = 0f;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        playerStats = GetComponent<PlayerStats>();
        playerClass = GetComponent<PlayerClass>();
    }

    void Update()
    {
        // Update cooldown
        if (shotCooldownTimer > 0)
            shotCooldownTimer -= Time.deltaTime;

        // Handle reload
        if (isReloading)
        {
            reloadTimer -= Time.deltaTime;
            if (reloadTimer <= 0f)
            {
                isReloading = false;
                playerStats.bulletsInMag = playerStats.maxBulletsInMag; // refill mag
            }
        }

        // Ground check
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        // Movement
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        controller.Move(move * playerStats.moveSpeed * Time.deltaTime);

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // Look
        float mouseX = lookInput.x * mouseSensitivity * Time.deltaTime;
        float mouseY = lookInput.y * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);

        // Update UI
        UpdateAmmoUI();
        UpdateStatUI();
    }

    // Input System Callbacks
    public void OnMove(InputValue input) => moveInput = input.Get<Vector2>();
    public void OnLook(InputValue input) => lookInput = input.Get<Vector2>();

    public void OnJump(InputValue input)
    {
        if (input.isPressed && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    public void OnShoot(InputValue input)
    {
        if (input.isPressed && !isReloading && shotCooldownTimer <= 0f && playerStats.bulletsInMag > 0)
        {
            Shoot();
        }
    }

    public void OnReload(InputValue input)
    {
        if (input.isPressed && !isReloading && playerStats.bulletsInMag < playerStats.maxBulletsInMag)
        {
            StartReload();
        }
    }

    private void Shoot()
    {
        if (bulletPrefab == null || firePoint == null) return;

        // Instantiate bullet
        GameObject bulletObj = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        if (bulletObj.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.linearVelocity = firePoint.forward * 30f;
        }

        // Assign damage to bullet
        if (bulletObj.TryGetComponent<Bullet>(out var bullet))
        {
            bullet.damage = playerStats.damagePerHit;
        }

        // Ammo + cooldown
        playerStats.bulletsInMag--;
        shotCooldownTimer = playerStats.shotCooldown;

        // Auto-reload if mag empty
        if (playerStats.bulletsInMag <= 0)
        {
            StartReload();
        }
    }

    private void StartReload()
    {
        if (playerStats.bulletsInMag < playerStats.maxBulletsInMag)
        {
            isReloading = true;
            reloadTimer = playerStats.reloadTime;
        }
    }


    private void UpdateAmmoUI()
    {
        if (ammoText == null) return;

        if (isReloading)
        {
            ammoText.text = $"Reloading...";
        }
        else
        {
            ammoText.text = $"{playerStats.bulletsInMag} / {playerStats.maxBulletsInMag}";
        }
    }

    private void UpdateStatUI()
    {
        classText.text = "" + playerClass.currentClass;
        weaponText.text = "" + playerClass.currentWeapon;
    }
}

