using UnityEngine;

public class CarController : MonoBehaviour
{
    private SurfaceEffector2D surfaceEffector2D;
    private Rigidbody2D rb;
    public float jumpForce = 7f;
    public bool isGrounded = false;
    public float torqueAmount = 2f; // 회전 계수(최대값)
    public float torqueIncreaseSpeed = 5f; // 토크 증가 속도
    public float maxAngularVelocity = 100f; // 최고 회전 가속도 제한
    public float baseSpeed = 0f;
    public float boostSpeed = 10f;
    public float speedIncreaseRate = 20f; // 속도 증가 속도

    private float currentTorque = 0f;
    private int torqueDirection = 0; // -1: 우회전, 1: 좌회전, 0: 없음
    private float currentSurfaceSpeed = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // deathEdge는 인스펙터에서 할당
    }

    void Update()
    {
        HandleJumpInput();
        UpdateUISpeedTexts();
    }

    private void FixedUpdate()
    {
        if (surfaceEffector2D != null)
        {
            // 땅에 있을 때만 오른쪽키로 가속 (서서히 증가/감소)
            float targetSpeed = 0f;
            if (isGrounded && Input.GetKey(KeyCode.RightArrow))
            {
                targetSpeed = boostSpeed;
            }
            // Lerp로 서서히 변화
            currentSurfaceSpeed = Mathf.MoveTowards(currentSurfaceSpeed, targetSpeed, speedIncreaseRate * Time.fixedDeltaTime);
            surfaceEffector2D.speed = currentSurfaceSpeed;
        }

        HandleRotationInput();
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

        // 공중에 있을 때만 회전
        if (!isGrounded)
        {
            int inputDir = 0;
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
                inputDir = 1;
            else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
                inputDir = -1;

            if (inputDir != 0)
            {
                if (torqueDirection != inputDir)
                {
                    currentTorque = 0f;
                    torqueDirection = inputDir;
                }
                currentTorque += torqueIncreaseSpeed * Time.fixedDeltaTime;
                currentTorque = Mathf.Clamp(currentTorque, 0f, torqueAmount);
                rb.AddTorque(currentTorque * inputDir, ForceMode2D.Force);
            }
            else
            {
                currentTorque = 0f;
                torqueDirection = 0;
            }
        }
        else
        {
            currentTorque = 0f;
            torqueDirection = 0;
        }
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
        // deathEdge가 충돌에 관여했는지 확인
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
        if (collision.gameObject.CompareTag("Ground"))
        {
            GameMgr.Instance.GameStop();
            return;
        }

        if (collision.gameObject.CompareTag("Obstacle"))
        {
            GameMgr.Instance.GameStop();
        }
    }
}
