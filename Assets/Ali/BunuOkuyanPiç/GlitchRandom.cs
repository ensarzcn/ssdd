using UnityEngine;
using System.Collections;

public class GlitchRandom : MonoBehaviour
{
    [Header("Glitch Ayarları")]
    [Tooltip("Her glitch arasında geçen süre (saniye cinsinden, rastgele aralık).")]
    public Vector2 glitchIntervalRange = new Vector2(25f, 35f);

    [Tooltip("Glitch (donma) süresi aralığı (saniye cinsinden).")]
    public Vector2 glitchDurationRange = new Vector2(1f, 5f);

    [Header("Dondurulacak Scriptler")]
    [Tooltip("Player kontrol, kamera kontrol, jetpack gibi scriptleri buraya sürükle.")]
    public MonoBehaviour[] controlScripts;

    [Header("Glitch Sprite Ayarları")]
    [Tooltip("Glitch sırasında sprite'ı değişecek objeler (örneğin göz, yüz, ışık vs.)")]
    public SpriteRenderer[] targetRenderers;

    [Tooltip("Glitch sırasında geçici olarak kullanılacak sprite'lar (aynı sıra ile).")]
    public Sprite[] glitchSprites;

    private Sprite[] originalSprites; // eski sprite'ları saklamak için
    private bool isGlitched = false;

    private Rigidbody rb;
    private CharacterController cc;
    private Vector3 savedVelocity;
    private bool[] previousStates;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        cc = GetComponent<CharacterController>();

        // Orijinal sprite'ları sakla
        if (targetRenderers != null && targetRenderers.Length > 0)
        {
            originalSprites = new Sprite[targetRenderers.Length];
            for (int i = 0; i < targetRenderers.Length; i++)
            {
                if (targetRenderers[i] != null)
                    originalSprites[i] = targetRenderers[i].sprite;
            }
        }

        StartCoroutine(GlitchLoop());
    }

    IEnumerator GlitchLoop()
    {
        while (true)
        {
            float waitTime = Random.Range(glitchIntervalRange.x, glitchIntervalRange.y);
            yield return new WaitForSeconds(waitTime);

            float glitchTime = Random.Range(glitchDurationRange.x, glitchDurationRange.y);
            yield return StartCoroutine(DoGlitch(glitchTime));
        }
    }

    IEnumerator DoGlitch(float duration)
    {
        if (isGlitched) yield break;
        isGlitched = true;

        Debug.Log($"⚡ GLITCH başladı ({duration:F1} saniye)");

        // Kontrol scriptlerini kapat
        previousStates = new bool[controlScripts.Length];
        for (int i = 0; i < controlScripts.Length; i++)
        {
            if (controlScripts[i] != null)
            {
                previousStates[i] = controlScripts[i].enabled;
                controlScripts[i].enabled = false;
            }
        }

        // Rigidbody ve hareketi dondur
        if (rb != null)
        {
            savedVelocity = rb.velocity;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        if (cc != null)
            cc.Move(Vector3.zero);

        // Sprite değişimi
        ApplyGlitchSprites(true);

        // Zamanı dondur
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(duration);

        // Zamanı geri aç
        Time.timeScale = 1f;

        // Eski scriptleri geri aç
        for (int i = 0; i < controlScripts.Length; i++)
        {
            if (controlScripts[i] != null)
                controlScripts[i].enabled = previousStates[i];
        }

        // Rigidbody’yi eski haline getir
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.velocity = Vector3.zero;
        }

        // Sprite’ları eski haline getir
        ApplyGlitchSprites(false);

        Debug.Log("✅ GLITCH sona erdi, her şey normale döndü.");
        isGlitched = false;
    }

    void ApplyGlitchSprites(bool apply)
    {
        if (targetRenderers == null || targetRenderers.Length == 0) return;

        for (int i = 0; i < targetRenderers.Length; i++)
        {
            if (targetRenderers[i] == null) continue;

            if (apply)
            {
                // glitch sprite ataması
                if (i < glitchSprites.Length && glitchSprites[i] != null)
                    targetRenderers[i].sprite = glitchSprites[i];
            }
            else
            {
                // orijinal sprite'a dön
                if (i < originalSprites.Length && originalSprites[i] != null)
                    targetRenderers[i].sprite = originalSprites[i];
            }
        }
    }
}
