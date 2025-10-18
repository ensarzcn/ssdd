using UnityEngine;

public class Target : MonoBehaviour
{
    [Header("Target Ayarları")]
    public float health = 100f;
    public GameObject deathEffect;
    public AudioClip hitSound;
    public AudioClip deathSound;
    public AudioClip deathEffectSound;    // 💡 Ölüm efekti çıktığında çalacak ses

    [Header("Shooting Ayarları")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 2f;
    public float bulletSpeed = 10f;
    public float detectionRange = 15f;
    public Transform target;

    private AudioSource audioSource;
    private float fireCountdown = 0f;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        if (target == null && GameObject.FindGameObjectWithTag("Player") != null)
        {
            target = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    void Update()
    {
        if (target != null)
        {
            float distanceToTarget = Vector3.Distance(transform.position, target.position);

            if (distanceToTarget <= detectionRange)
            {
                transform.LookAt(target);

                if (fireCountdown <= 0f)
                {
                    Shoot();
                    fireCountdown = 1f / fireRate;
                }
            }

            fireCountdown -= Time.deltaTime;
        }
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        Debug.Log($"Vuruldum! Kalan Can: {health}");

        if (hitSound != null)
        {
            audioSource.PlayOneShot(hitSound);
        }

        if (health <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        if (deathEffect != null)
        {
            GameObject effect = Instantiate(deathEffect, transform.position, Quaternion.identity);

            // 💡 Eğer ses efekti atanmışsa, direkt pozisyonda çal
            if (deathEffectSound != null)
            {
                AudioSource.PlayClipAtPoint(deathEffectSound, transform.position);
            }

            Destroy(effect, 2f);
        }

        if (deathSound != null)
        {
            AudioSource.PlayClipAtPoint(deathSound, transform.position);
        }

        Destroy(gameObject);
    }

    void Shoot()
    {
        if (bulletPrefab != null && firePoint != null)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            rb.velocity = firePoint.forward * bulletSpeed;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
