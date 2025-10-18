using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartLevelOnCollision : MonoBehaviour
{
    // Oyuncunun temas ettiği collider ile tetiklenir
    private void OnTriggerEnter(Collider other)
    {
        // Eğer temas eden oyuncu ise (etiketle kontrol ediyoruz)
        if (other.CompareTag("Player"))
        {
            // Mevcut sahneyi yeniden yükle
            Scene currentScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(currentScene.name);
        }
    }
}
