using UnityEngine;

public class GroundTrigger : MonoBehaviour
{
    // Yere düþen objenin tag'ini kontrol edeceðiz
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Oyuncunun PlayerHealth scriptini bul ve canýný 0 yap
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(playerHealth.maxHealth); // Canýný direkt sýfýrlýyoruz
            }
        }
    }
}
