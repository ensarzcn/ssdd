using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Transform cameraTransform;

    void Start()
    {
        // Ana kamerayı bulma
        cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        // Kameradan sprite'a doğru olan yönü bul
        Vector3 direction = (transform.position - cameraTransform.position).normalized;

        // Y eksenini sabit tutarak rotasyonu kameraya bakacak şekilde ayarla
        direction.y = 0;

        // Kameraya bakacak şekilde dönüşü ayarla
        transform.rotation = Quaternion.LookRotation(-direction);
    }
}
