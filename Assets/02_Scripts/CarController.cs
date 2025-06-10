using UnityEngine;

public class CarController : MonoBehaviour
{
    private SurfaceEffector2D surfaceEffector2D;
    private Rigidbody2D rb;
    public float jumpForce = 7f;
    private bool isGrounded = false;
    public float torqueAmount = 2f; // 회전 계수 낮춤
    public float maxAngularVelocity = 100f; // 최고 회전 가속도 제한
    public float baseSpeed = 5f;
    public float boostSpeed = 10f;
    private bool isBoosting = false;
    private EdgeCollider2D deathEdge; // 사망 판정용 에지 콜라이더

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        deathEdge = GetComponent<EdgeCollider2D>();
    }

    void Update()
    {
        HandleJumpInput();
        UpdateUISpeedTexts();
        HandleRotationInput();
    }

    private void FixedUpdate()
    {
        if (surfaceEffector2D != null)
        {
            surfaceEffector2D.speed = isBoosting ? boostSpeed : baseSpeed;
        }
        LimitAngularVelocity();
    }

    private void HandleJumpInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && rb != null)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            isGrounded = false;
        }
    }

    private void HandleRotationInput()
    {
        if (rb == null) return;

        float torque = 0f;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            torque = torqueAmount;
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            torque = -torqueAmount;

        if (torque != 0f)
            rb.AddTorque(torque, ForceMode2D.Force);
    }

    private void LimitAngularVelocity()
    {
        if (rb == null) return;
        rb.angularVelocity = Mathf.Clamp(rb.angularVelocity, -maxAngularVelocity, maxAngularVelocity);
    }

    private void UpdateUISpeedTexts()
    {
        if (rb != null)
        {
            UIMgr.Instance.UpdateCarSpeedText($"Car Speed: {rb.linearVelocity.magnitude:F1}");
            if (surfaceEffector2D != null)
                UIMgr.Instance.UpdateSurfaceSpeedText($"Surface Speed: {surfaceEffector2D.speed:F1}");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 내 EdgeCollider2D가 충돌에 관여했는지 확인
        // OnCollisionEnter2D 내부에서
        foreach (var contact in collision.contacts)
        {
            if (contact.collider == deathEdge)
            {
                GameMgr.Instance.GameStop();
                return;
            }
        }

        if (collision.gameObject.TryGetComponent<SurfaceEffector2D>(out var effector))
        {
            surfaceEffector2D = effector;
        }
           
        // Ground 태그 감지로 점프 가능
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }  
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            GameMgr.Instance.GameStop();
        }
    }
}
