using Unity.Mathematics;
using UnityEngine;

public class PlayerMove : Singleton<PlayerMove>
{
    [SerializeField] private float moveSpeed = 4;
    [HideInInspector] public Vector2 direction = Vector2.zero;
    [HideInInspector] public bool isTurningRight, isTurningLeft = false;
    private Rigidbody2D rb;
    private Animator animator;

    private const float Deadzone = 0.1f;

    void Start()
    {
        rb = PlayerConfig.Instance.Rb;
        animator = PlayerConfig.Instance.Animator;
        rb.gravityScale = 0;
    }

    void FixedUpdate()
    {
        SetRotation();
        SetVelocity();
        UpdateAnimator();
    }

    private void SetVelocity()
    {
        if (PlayerConfig.Instance.Status == PlayerStatus.Recoil) return;

        if (direction.magnitude > 1) direction.Normalize();
        Vector2 velocity = direction * moveSpeed;

        rb.linearVelocity = velocity;
    }

    private void SetRotation()
    {
        /*         if (isTurningLeft) currentAngle -= rotationSpeed * 30f * Time.deltaTime;
                if (isTurningRight) currentAngle += rotationSpeed * 30f * Time.deltaTime;
                Quaternion newRotation = Quaternion.Euler(0, 0, currentAngle);

                gameObject.GetComponent<Pixelate>().rotation = newRotation; */
    }

    private void UpdateAnimator()
    {
        if (!animator) return;

        float speed = direction.magnitude;
        animator.SetFloat("Speed", speed);

        if (speed > Deadzone)
        {
            Vector2 dir = direction.normalized;

            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            if (angle < 0) angle += 360f;

            float adjusted = (angle + 22.5f) % 360f;

            int index = Mathf.FloorToInt(adjusted / 45f);

            animator.SetFloat("DirectionIndex", (float)index);
        }
    }
}
