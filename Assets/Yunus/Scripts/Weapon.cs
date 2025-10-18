using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("Silah Ayarları")]
    public float fireRate = 0.3f;            // Her atış arasındaki bekleme süresi (saniye)
    public float bulletSpeed = 50f;          // Mermi hızı

    [Header("Referanslar")]
    public Camera fpsCamera;                 // Atış yönünü belirlemek için kamera referansı
    public ParticleSystem muzzleFlash;       // Namlu ateşi efekti
    public GameObject bulletPrefab;          // Mermi prefab'ı
    public Transform firePoint;              // Merminin çıkış noktası
    public Animator animator;                // Animator referansı
    public AudioClip[] fireSounds;           // Rastgele çalınacak ses efektleri

    private AudioSource audioSource;
    private float nextTimeToFire = 0f;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        // Eğer AudioSource yoksa ekle
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void Update()
    {
        if (Input.GetMouseButton(0) && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + fireRate;
            Shoot();
        }
    }

    void Shoot()
    {
        // Animasyonu başlat
        animator.SetBool("isAttacking", true);

        // Namlu ateşi efekti
        if (muzzleFlash != null)
        {
            muzzleFlash.Play();
        }

        // Ses efekti rastgele seçilip çalınıyor
        PlayRandomFireSound();

        // Ekranın tam ortasındaki noktayı hesapla
        Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        Ray ray = fpsCamera.ScreenPointToRay(screenCenter);
        RaycastHit hit;

        // Hedef noktayı bul
        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.GetPoint(100);
        }

        // Mermiyi oluştur ve hedefe doğru fırlat
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Vector3 direction = (targetPoint - firePoint.position).normalized;

        // Rigidbody'ye hız ekle
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.velocity = direction * bulletSpeed;

        // Efekti başlat
        ParticleSystem bulletEffect = bullet.GetComponentInChildren<ParticleSystem>();
        if (bulletEffect != null)
        {
            bulletEffect.Play();
        }

        // Animasyonu bitir
        StartCoroutine(EndAttackAnimation());
    }

    void PlayRandomFireSound()
    {
        if (fireSounds.Length > 0)
        {
            int randomIndex = Random.Range(0, fireSounds.Length);
            AudioClip randomClip = fireSounds[randomIndex];
            audioSource.PlayOneShot(randomClip);
        }
    }

    System.Collections.IEnumerator EndAttackAnimation()
    {
        // Animasyonun süresini al
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        float animationDuration = stateInfo.length;

        // Animasyon süresi kadar bekle
        yield return new WaitForSeconds(animationDuration);

        // Saldırı animasyonu biter bitmez idle'a geçiş yap
        animator.SetBool("isAttacking", false);
    }
}
