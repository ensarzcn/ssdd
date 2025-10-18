using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class PlayerJetpack : MonoBehaviour
{
    [Header("Jetpack Ayarları")]
    public float jetpackForce = 8f;
    public float jetpackAcceleration = 5f;
    public float maxJetpackSpeed = 10f;
    public float staminaMax = 5f;
    public float staminaRegenRate = 1f;
    public float staminaCooldownAfterUse = 2f;
    public Slider staminaSlider;

    [Header("Kontroller")]
    public KeyCode jetpackKey = KeyCode.Space;

    [Header("Sesler")]
    public AudioSource audioSource;
    public AudioClip jetClip;        // 10 sn’lik mp3
    public AudioClip emptyFuelClip;

    [Header("Particle")]
    public ParticleSystem jetpackParticlesPrefab;
    public float particleSpawnInterval = 0.5f;

    private CharacterController cc;
    private Vector3 verticalVelocity;
    private bool isUsingJetpack = false;
    private float currentStamina;
    private bool canUseJetpack = true;
    private ParticleSystem jetpackParticlesInstance;
    private float particleTimer = 0f;

    void Start()
    {
        cc = GetComponent<CharacterController>();
        currentStamina = staminaMax;

        if (staminaSlider != null)
        {
            staminaSlider.maxValue = staminaMax;
            staminaSlider.value = currentStamina;
        }

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        // Jet clip loop yap
        if(jetClip != null)
        {
            audioSource.clip = jetClip;
            audioSource.loop = true;
        }

        // Particle spawn
        if(jetpackParticlesPrefab != null)
        {
            jetpackParticlesInstance = Instantiate(jetpackParticlesPrefab, transform.position + Vector3.down * 1f, Quaternion.identity, transform);
            jetpackParticlesInstance.Stop();
        }
    }

    void Update()
    {
        HandleJetpack();
        ApplyVerticalMovement();
        UpdateUI();
        ManageParticles();
    }

    void HandleJetpack()
    {
        if (Input.GetKey(jetpackKey))
        {
            if(cc.isGrounded)
            {
                verticalVelocity.y = jetpackForce; // normal jump
            }
            else if(currentStamina > 0f && canUseJetpack)
            {
                isUsingJetpack = true;
                StartJetSound(); // Basılı tuttuğunda loop ile oynat
            }
            else if(currentStamina <= 0f)
            {
                PlayEmptyFuelSound();
            }
        }

        if(Input.GetKey(jetpackKey) && !cc.isGrounded && currentStamina > 0f && canUseJetpack)
        {
            isUsingJetpack = true;

          
        }
        else
        {
            isUsingJetpack = false;

            // Space bırakıldığında veya stamina bittiğinde sesi durdur
            if(audioSource.isPlaying && audioSource.clip == jetClip)
                audioSource.Stop();
        }

        // Stamina yönetimi
        if(isUsingJetpack)
        {
            currentStamina -= Time.deltaTime;
            currentStamina = Mathf.Max(currentStamina, 0f);

            if(currentStamina <= 0f)
            {
                isUsingJetpack = false;
                StartCoroutine(JetpackCooldown());
                if(audioSource.isPlaying && audioSource.clip == jetClip)
                    audioSource.Stop();
            }
        }
        else if(!isUsingJetpack && cc.isGrounded)
        {
            if(canUseJetpack)
            {
                currentStamina += staminaRegenRate * Time.deltaTime;
                currentStamina = Mathf.Min(currentStamina, staminaMax);
            }
        }
    }

    IEnumerator JetpackCooldown()
    {
        canUseJetpack = false;
        yield return new WaitForSeconds(staminaCooldownAfterUse);
        canUseJetpack = true;
    }

    void ApplyVerticalMovement()
    {
        if(isUsingJetpack)
        {
            verticalVelocity.y += jetpackAcceleration * Time.deltaTime;
            verticalVelocity.y = Mathf.Clamp(verticalVelocity.y, 0f, maxJetpackSpeed);
        }

        if(!cc.isGrounded && !isUsingJetpack)
            verticalVelocity.y += Physics.gravity.y * Time.deltaTime;
        else if(cc.isGrounded && !isUsingJetpack)
            verticalVelocity.y = -0.5f;

        cc.Move(verticalVelocity * Time.deltaTime);
    }

    void UpdateUI()
    {
        if(staminaSlider != null)
            staminaSlider.value = currentStamina;
    }

    void StartJetSound()
    {
        if(!audioSource.isPlaying)
            audioSource.Play(); // clip zaten loop, tekrar başlatılmaz
    }

    void PlayEmptyFuelSound()
    {
        if(audioSource.clip != emptyFuelClip || !audioSource.isPlaying)
        {
            audioSource.clip = emptyFuelClip;
            audioSource.loop = false; // boş yakıt sesi tek seferlik
            audioSource.Play();
        }
    }

    void ManageParticles()
    {
        if(jetpackParticlesInstance == null) return;

        if(isUsingJetpack && currentStamina > 0f)
        {
            particleTimer += Time.deltaTime;
            if(particleTimer >= particleSpawnInterval)
            {
                if(!jetpackParticlesInstance.isPlaying)
                    jetpackParticlesInstance.Play();
                particleTimer = 0f;
            }

            var emission = jetpackParticlesInstance.emission;
            emission.rateOverTime = Mathf.Lerp(10, 50, verticalVelocity.y / maxJetpackSpeed);
        }
        else
        {
            if(jetpackParticlesInstance.isPlaying)
                jetpackParticlesInstance.Stop();
        }

        jetpackParticlesInstance.transform.position = transform.position + Vector3.down * 1f;
    }
}
