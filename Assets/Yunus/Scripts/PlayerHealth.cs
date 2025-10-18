using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Player Health Ayarlarý")]
    public float maxHealth = 100f;
    private float currentHealth;

    public GameObject deathEffect;         // Ölüm efekti prefab'ý
    public AudioClip hitSound;             // Vurulma sesi
    public AudioClip deathSound;           // Ölüm sesi
    public float restartDelay = 2f;        // Sahne restart süresi (2 saniye sonra)

    public GameObject hand2Object;         // Hand 2 GameObject referansý
    public CanvasGroup hitEffectCanvas;    // Kýrmýzý Hit Effect Canvas

    private AudioSource audioSource;

    void Start()
    {
        currentHealth = maxHealth;
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Hand 2 GameObject'ini sahnede bul
        if (hand2Object == null)
        {
            hand2Object = GameObject.Find("Hand 2");
        }

        // HitEffect Canvas'ý bul
        if (hitEffectCanvas == null)
        {
            hitEffectCanvas = GameObject.Find("HitEffect").GetComponent<CanvasGroup>();
        }
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        Debug.Log($"Player vuruldu! Kalan Can: {currentHealth}");

        if (hitSound != null)
        {
            audioSource.PlayOneShot(hitSound);
        }

        if (hitEffectCanvas != null)
        {
            StartCoroutine(ShowHitEffect());
        }

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        if (deathEffect != null)
        {
            GameObject effect = Instantiate(deathEffect, transform.position, Quaternion.identity);
            Destroy(effect, 2f);
        }

        if (deathSound != null)
        {
            AudioSource.PlayClipAtPoint(deathSound, transform.position);
        }

        Debug.Log("Player öldü!");

        if (hand2Object != null)
        {
            hand2Object.SetActive(false);
        }

        Time.timeScale = 0;

        StartCoroutine(WaitAndRestart());
    }

    System.Collections.IEnumerator WaitAndRestart()
    {
        yield return new WaitForSecondsRealtime(restartDelay);

        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    System.Collections.IEnumerator ShowHitEffect()
    {
        // CanvasGroup görünür hale gelir
        hitEffectCanvas.alpha = 0.5f;

        // 0.2 saniye bekle (hafif bir yanýp sönme efekti)
        yield return new WaitForSeconds(0.2f);

        // Þeffaflaþtýr
        float fadeDuration = 0.5f;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            hitEffectCanvas.alpha = Mathf.Lerp(0.5f, 0f, elapsed / fadeDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        hitEffectCanvas.alpha = 0f;
    }
}
