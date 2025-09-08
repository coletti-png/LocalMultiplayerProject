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
    [SerializeField] private TMP_Text healthText;

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
    private bool fireHeld = false;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        playerStats = GetComponent<PlayerStats>();
        playerClass = GetComponent<PlayerClass>();
    }

    void Update()
    {
        // Update health UI
        healthText.text = playerStats.currentHealth.ToString();

        // Cooldowns
        if (shotCooldownTimer > 0f)
            shotCooldownTimer -= Time.deltaTime;

        if (isReloading)
        {
            fireHeld = false;
            reloadTimer -= Time.deltaTime;
            if (reloadTimer <= 0f)
            {
                isReloading = false;
                playerStats.bulletsInMag = playerStats.maxBulletsInMag;
            }
        }

        // Auto-fire for SMG
        if (fireHeld && playerStats.autoFire && !isReloading && shotCooldownTimer <= 0f && playerStats.bulletsInMag > 0)
        {
            Shoot();
        }

        if (Gamepad.current != null && Gamepad.current.rightTrigger.wasReleasedThisFrame)
        {
            fireHeld = false;
        }

        // Movement & gravity
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0f)
            velocity.y = -2f;

        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        controller.Move(move * playerStats.moveSpeed * Time.deltaTime);

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // Look rotation
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

    #region Input System Callbacks
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
        // R2 trigger or button input returns a float (0 to 1)
        float triggerValue = input.Get<float>();
        fireHeld = triggerValue > 0.1f; // consider pressed if partially pulled

        // Semi-auto fire: fire ONCE per press
        if (fireHeld && !playerStats.autoFire && !isReloading && shotCooldownTimer <= 0f && playerStats.bulletsInMag > 0)
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
    #endregion

    #region Shooting Methods
    private void Shoot()
    {
        if (bulletPrefab == null || firePoint == null) return;

        switch (playerClass.currentWeapon)
        {
            case PlayerClass.Weapon.SMG:
                ShootSingleBullet();
                break;

            case PlayerClass.Weapon.Shotgun:
                ShootShotgun();
                break;

            case PlayerClass.Weapon.Sniper:
                ShootSingleBullet();
                break;
        }

        shotCooldownTimer = playerStats.shotCooldown;
        playerStats.bulletsInMag--;

        if (playerStats.bulletsInMag <= 0)
            StartReload();
    }

    private void ShootSingleBullet()
    {
        GameObject bulletObj = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        if (bulletObj.TryGetComponent<Rigidbody>(out var rb))
            rb.linearVelocity = firePoint.forward * 30f;

        if (bulletObj.TryGetComponent<Bullet>(out var bullet))
            bullet.damage = playerStats.damagePerHit;
    }

    private void ShootShotgun()
    {
        int pelletCount = 7;
        float spreadAngle = 7f;

        for (int i = 0; i < pelletCount; i++)
        {
            Quaternion spreadRotation = firePoint.rotation *
                Quaternion.Euler(Random.Range(-spreadAngle, spreadAngle),
                                 Random.Range(-spreadAngle, spreadAngle),
                                 0);

            GameObject pellet = Instantiate(bulletPrefab, firePoint.position, spreadRotation);

            if (pellet.TryGetComponent<Rigidbody>(out var rb))
                rb.linearVelocity = spreadRotation * Vector3.forward * 30f;

            if (pellet.TryGetComponent<Bullet>(out var bullet))
                bullet.damage = playerStats.damagePerHit / pelletCount;
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
    #endregion

    #region UI Methods
    private void UpdateAmmoUI()
    {
        if (ammoText == null) return;

        ammoText.text = isReloading
            ? "Reloading..."
            : $"{playerStats.bulletsInMag} / {playerStats.maxBulletsInMag}";
    }

    private void UpdateStatUI()
    {
        classText.text = playerClass.currentClass.ToString();
        weaponText.text = playerClass.currentWeapon.ToString();
    }
    #endregion
}
