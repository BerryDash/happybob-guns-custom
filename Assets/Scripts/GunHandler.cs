using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class GunHandler : MonoBehaviour
{
    public static GunHandler Instance;
    public SpriteRenderer bulletTemplate;
    private bool hasAutoRotated = false;
    private bool tracking = true;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (GamePlayer.instance.pausePanel.activeSelf) return;

        Vector2 inputPos = Vector2.zero;
        bool pressedNow = false;
        bool isMobile = Touchscreen.current != null;

        if (isMobile)
        {
            var touch = Touchscreen.current.primaryTouch;
            bool touchingUI = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(touch.touchId.ReadValue());
            if (touch.press.wasPressedThisFrame && !touchingUI) tracking = true;
            if (touch.press.wasReleasedThisFrame) tracking = false;
            if (tracking && !touchingUI) inputPos = touch.position.ReadValue();
        }
        else
        {
            inputPos = Mouse.current.position.ReadValue();
            pressedNow = Mouse.current.leftButton.wasPressedThisFrame
                || Keyboard.current.leftShiftKey.wasPressedThisFrame
                || Keyboard.current.rightShiftKey.wasPressedThisFrame
                || Keyboard.current.fKey.wasPressedThisFrame;
        }

        if (!tracking) return;

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(
            new Vector3(inputPos.x, inputPos.y, -Camera.main.transform.position.z)
        );

        Vector2 direction = worldPos - transform.position;
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
                var scale = transform.parent.localScale;
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

        if (pressedNow) ShootGun();
    }

    internal void ShootGun()
    {
        bool flipped = transform.parent.localScale.x > 0f;
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
