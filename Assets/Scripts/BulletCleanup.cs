using UnityEngine;

public class BulletCleanup : MonoBehaviour
{
    void Update()
    {
        Vector3 screenPos = Camera.main.WorldToViewportPoint(transform.position);
        if (screenPos.x < 0f || screenPos.x > 1f || screenPos.y < 0f || screenPos.y > 1f) Destroy(gameObject);
    }
}
