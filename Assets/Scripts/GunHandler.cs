using UnityEngine;
using UnityEngine.InputSystem;

public class GunHandler : MonoBehaviour
{
    public SpriteRenderer bulletTemplate;
    private bool hasAutoRotated = false;

    void Update()
    {
        if (GamePlayer.instance.pausePanel.activeSelf) return;

        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(
            new Vector3(mouseScreenPos.x, mouseScreenPos.y, -Camera.main.transform.position.z)
        );

        Vector2 direction = mouseWorldPos - transform.position;
        bool flipped = transform.parent.localScale.x > 0f;

        if (flipped)
        {
            direction.x = -direction.x;
            direction.y = -direction.y;
        }

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        float minAngle = flipped ? -70f : -75f;
        float maxAngle = flipped ? 75f : 70f;
        float clamped = Mathf.Clamp(angle, minAngle, maxAngle);

        if (angle < minAngle || angle > maxAngle)
        {
            if (!hasAutoRotated)
            {
                hasAutoRotated = true;
                Vector3 scale = transform.parent.localScale;
                scale.x *= -1;
                transform.parent.localScale = scale;
                flipped = !flipped;

                direction.x = -direction.x;
                direction.y = -direction.y;
                clamped = Mathf.Clamp(Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg, flipped ? -70f : -75f, flipped ? 75f : 70f);
            }
        }
        else if (hasAutoRotated)
        {
            hasAutoRotated = false;
        }

        transform.rotation = Quaternion.Euler(0f, 0f, clamped);

        if (Keyboard.current.leftShiftKey.wasPressedThisFrame || Keyboard.current.rightShiftKey.wasPressedThisFrame || Keyboard.current.fKey.wasPressedThisFrame || Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector3 offset = new(0.6932073f, 0.352302f, 0f);
            if (flipped) offset.x = -offset.x;
            Vector3 spawnPos = transform.position + transform.rotation * offset;
            var bullet = Instantiate(bulletTemplate, spawnPos, Quaternion.identity);
            bullet.transform.rotation = Quaternion.Euler(0f, 0f, transform.rotation.eulerAngles.z + (flipped ? 0 : 180f));
            bullet.gameObject.name = "Bullet";
            bullet.gameObject.SetActive(true);

            float angleRad = bullet.transform.eulerAngles.z * Mathf.Deg2Rad;
            Vector2 dir = new(-Mathf.Cos(angleRad), -Mathf.Sin(angleRad));
            bullet.GetComponent<Rigidbody2D>().linearVelocity = dir * 10f;
        }
    }
}
