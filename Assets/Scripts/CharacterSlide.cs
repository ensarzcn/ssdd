using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class TrueIndependentDashWithCamera : MonoBehaviour
{
    [Header("Dash Settings")]
    public float dashForce = 20f;             // Dash hızı
    public float dashDuration = 0.2f;         // Dash süresi
    public float dashCooldown = 1f;           // Dash tekrar süresi

    [Header("Camera Effect")]
    public Transform playerCamera;            // Kamera referansı
    public float cameraBackOffset = 0.5f;     // Dash sırasında kamera geriye çekilme miktarı
    public float cameraReturnSpeed = 5f;      // Kamera dönüş hızı

    [Header("Dash FX & Sound")]
    public AudioSource audioSource;           // Ses kaynağı
    public AudioClip dashSound;               // Dash sesi (örnek: "whoosh.mp3")
    public GameObject dashVFXPrefab;          // Dash VFX prefabı (örnek: particle effect)
    public Transform vfxSpawnPoint;           // Efektin çıkacağı nokta (boş obje olarak karakterin arkasına koy)

    private CharacterController controller;
    private bool isDashing = false;
    private float dashTimer = 0f;
    private float cooldownTimer = 0f;

    private Vector3 externalVelocity = Vector3.zero;
    private Vector3 defaultCamLocalPos;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        if (playerCamera != null)
            defaultCamLocalPos = playerCamera.localPosition;

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Update()
    {
        // Cooldown işlemi
        if (cooldownTimer > 0f)
            cooldownTimer -= Time.deltaTime;

        // Dash inputu
        if (Input.GetKeyDown(KeyCode.LeftAlt) && !isDashing && cooldownTimer <= 0f)
        {
            StartDash();
        }

        // Dash aktifse
        if (isDashing)
        {
            dashTimer -= Time.deltaTime;
            if (dashTimer <= 0f)
            {
                StopDash();
            }
        }

        // Dash kuvvetini uygula
        if (externalVelocity != Vector3.zero)
        {
            controller.Move(externalVelocity * Time.deltaTime);
            externalVelocity = Vector3.Lerp(externalVelocity, Vector3.zero, Time.deltaTime * 10f);
        }

        // Kamera geri çekilme efekti
        if (playerCamera != null)
        {
            Vector3 targetPos = defaultCamLocalPos;
            if (isDashing)
                targetPos -= Vector3.forward * cameraBackOffset;

            playerCamera.localPosition = Vector3.Lerp(
                playerCamera.localPosition,
                targetPos,
                Time.deltaTime * cameraReturnSpeed
            );
        }
    }

    void StartDash()
    {
        isDashing = true;
        dashTimer = dashDuration;
        cooldownTimer = dashCooldown;

        Vector3 dashDir = transform.forward;
        dashDir.y = 0;
        dashDir.Normalize();
        externalVelocity = dashDir * dashForce;

        // 🎧 Dash sesi
        if (dashSound != null)
        {
            audioSource.PlayOneShot(dashSound);
        }

        // 💥 Dash VFX
        if (dashVFXPrefab != null)
        {
            Vector3 spawnPos = (vfxSpawnPoint != null) ? vfxSpawnPoint.position : transform.position;
            GameObject vfx = Instantiate(dashVFXPrefab, spawnPos, transform.rotation);
            Destroy(vfx, 2f); // 2 saniye sonra otomatik sil
        }
    }

    void StopDash()
    {
        isDashing = false;
    }
}
