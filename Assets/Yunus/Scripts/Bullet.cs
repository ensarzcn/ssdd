using UnityEngine;
using System.Collections;
using UnityEditor.Experimental.GraphView;

public class Bullet : MonoBehaviour
{
    public float damage = 20f;                   // Mermi hasarı
    public GameObject impactEffect;              // Çarpma efekti
    public float destroyDelay = 2f;              // Merminin yok olma süresi
    
    

    private Rigidbody rb;


    void Start()
    {
        // Mermiyi belirli bir süre sonra yok et
        Destroy(gameObject, destroyDelay);
    }

    void OnTriggerEnter(Collider other)
    {
        // Eğer "Target" etiketi olan bir şeye çarptıysa
        Target target = other.GetComponent<Target>();
        if (target != null)
        {
            target.TakeDamage(damage);
        }

        // Çarpma efekti oluştur
        if (impactEffect != null)
        {
            GameObject effect = Instantiate(impactEffect, transform.position, Quaternion.identity);
            Destroy(effect, 1.5f); // Efekt 1.5 saniye sonra yok olsun
        }

        // Mermiyi yok et
        Destroy(gameObject);
       
    }
}
