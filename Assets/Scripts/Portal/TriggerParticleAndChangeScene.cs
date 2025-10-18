using UnityEngine;
using UnityEngine.SceneManagement;

public class TriggerParticleAndChangeScene : MonoBehaviour
{
    [Header("Collider ve Tag Ayarları")]
    public string targetTag = "AnimTrigger";    // Hedef collider'ın tag'ı

    [Header("Particle Ayarları")]
    public GameObject particlePrefab;           // Particle prefabı
    public float waitTime = 3f;                 // Sahne değişim süresi

    [Header("Sahne Ayarları")]
    public string nextSceneName = "NextScene";  // Sonraki sahne adı

    private bool isPlayerFrozen = false;        // Hareketi durdurma kontrolü

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            // Karakterin hareketini durdur
            FreezePlayer();

            // Partikül prefabını görünür yap
            if (particlePrefab != null)
            {
                particlePrefab.SetActive(true);
            }

            // Belirtilen süre sonra sahneyi değiştir
            Invoke(nameof(ChangeScene), waitTime);
        }
    }

    private void FreezePlayer()
    {
        // Hareketi engellemek için karakteri dondur
        isPlayerFrozen = true;
    }

    private void UnfreezePlayer()
    {
        // Karakterin hareketini geri getir
        isPlayerFrozen = false;
    }

    private void ChangeScene()
    {
        // Hareketi geri getir ve sahne değiştir
        UnfreezePlayer();
        SceneManager.LoadScene(nextSceneName);
    }

    private void Update()
    {
        // Hareketi kısıtla
        if (isPlayerFrozen)
        {
            // Hareket girişlerini sıfırla
            if (GetComponent<CharacterController>() != null)
            {
                GetComponent<CharacterController>().enabled = false;
            }
            if (GetComponent<Rigidbody>() != null)
            {
                GetComponent<Rigidbody>().velocity = Vector3.zero;
            }
        }
        else
        {
            // Hareketi geri aç
            if (GetComponent<CharacterController>() != null)
            {
                GetComponent<CharacterController>().enabled = true;
            }
        }
    }
}
