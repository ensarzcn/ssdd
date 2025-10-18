using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float damage = 10f;           // Merminin verdiði hasar
    public float speed = 5f;             // Merminin ilerleme hýzý
    public float lifetime = 5f;          // Merminin ömrü
    private Transform target;            // Hedef (Player)

    void Start()
    {
        // Player'ý hedef olarak buluyoruz
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            target = player.transform;
            // Merminin yönünü direkt player'a doðru ayarlýyoruz
            transform.LookAt(target);
        }

        // Belirtilen süre sonunda mermi yok olacak
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        if (target != null)
        {
            // Mermi, hedefe doðru hareket eder
            transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Player'da PlayerHealth bileþeni varsa hasar ver
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }

            // Mermi yok edilir
            Destroy(gameObject);
        }
    }
}
