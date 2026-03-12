using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GlitchRandom : MonoBehaviour
{
    [Header("Glitch Ayarları")]
    public Vector2 glitchIntervalRange = new Vector2(25f, 35f);
    public Vector2 glitchDurationRange = new Vector2(1f, 5f);

    [Header("Player Sprite'ları")]
    public SpriteRenderer solElSprite;  // Inspector'dan seç
    public SpriteRenderer sagElSprite;  // Inspector'dan seç

    [Header("UI Glitch Image'ları")]
    public Image solElUIImage;  // Canvas’taki sol el Image
    public Image sagElUIImage;  // Canvas’taki sağ el Image

    private Sprite solElOrjinal;
    private Sprite sagElOrjinal;
    private Animator sagElAnimator;

    private bool isGlitched = false;
    private CharacterController cc;
    private Rigidbody rb;
    private MonoBehaviour[] allScripts;
    private MonoBehaviour[] weaponScripts;

    void Start()
    {
        cc = GetComponent<CharacterController>();
        rb = GetComponent<Rigidbody>();
        allScripts = GetComponents<MonoBehaviour>();
        weaponScripts = GetComponentsInChildren<MonoBehaviour>();

        // Orijinal spriteları sakla
        if (solElSprite != null) solElOrjinal = solElSprite.sprite;
        if (sagElSprite != null)
        {
            sagElOrjinal = sagElSprite.sprite;
            sagElAnimator = sagElSprite.GetComponent<Animator>();
        }

        // UI imageleri kapalı başlat
        if (solElUIImage != null) solElUIImage.gameObject.SetActive(false);
        if (sagElUIImage != null) sagElUIImage.gameObject.SetActive(false);

        StartCoroutine(GlitchLoop());
    }

    IEnumerator GlitchLoop()
    {
        while (true)
        {
            float waitTime = Random.Range(glitchIntervalRange.x, glitchIntervalRange.y);
            yield return new WaitForSeconds(waitTime);

            float glitchTime = Random.Range(glitchDurationRange.x, glitchDurationRange.y);
            StartCoroutine(DoGlitch(glitchTime));
        }
    }

    IEnumerator DoGlitch(float duration)
    {
        if (isGlitched) yield break;
        isGlitched = true;

        // ----------------- Hareket ve scriptleri durdur -----------------
        foreach (var script in allScripts)
        {
            if (script != this)
                script.enabled = false;
        }

        foreach (var wScript in weaponScripts)
        {
            if (wScript != this) wScript.enabled = false;
        }

        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.isKinematic = true;
        }

        // ----------------- Player spriteları gizle -----------------
        if (solElSprite != null) solElSprite.sprite = null;
        if (sagElSprite != null)
        {
            sagElSprite.sprite = null;
            if (sagElAnimator != null) sagElAnimator.enabled = false;
        }

        // ----------------- UI imageleri göster -----------------
        if (solElUIImage != null) solElUIImage.gameObject.SetActive(true);
        if (sagElUIImage != null) sagElUIImage.gameObject.SetActive(true);

        // ----------------- Glitch süresi -----------------
        yield return new WaitForSeconds(duration);

        // ----------------- Her şeyi eski haline döndür -----------------
        foreach (var script in allScripts)
        {
            if (script != this)
                script.enabled = true;
        }

        foreach (var wScript in weaponScripts)
        {
            if (wScript != this) wScript.enabled = true;
        }

        if (rb != null) rb.isKinematic = false;

        // Player spriteları geri getir
        if (solElSprite != null) solElSprite.sprite = solElOrjinal;
        if (sagElSprite != null)
        {
            sagElSprite.sprite = sagElOrjinal;
            if (sagElAnimator != null) sagElAnimator.enabled = true;
        }

        // UI imageleri kapat
        if (solElUIImage != null) solElUIImage.gameObject.SetActive(false);
        if (sagElUIImage != null) sagElUIImage.gameObject.SetActive(false);

        isGlitched = false;
    }
}
