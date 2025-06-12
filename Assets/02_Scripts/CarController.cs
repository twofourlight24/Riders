using UnityEngine;
using Unity.Cinemachine; // 네임스페이스 

public class CarController : MonoBehaviour
{
    private SurfaceEffector2D surfaceEffector2D;
    private Rigidbody2D rb;
    public float jumpForce = 7f;
    public bool isGrounded = false;
    public float torqueAmount = 2f; // 회전 계수(최대값)
    public float maxAngularVelocity = 100f; // 최고 회전 가속도 제한
    public float baseSpeed = 0f;
    public float boostSpeed = 10f;
    public float speedIncreaseRate = 20f; // 속도 증가 속도

    private float originalBoostSpeed;
    private float originalSpeedIncreaseRate;
    private float boostTimer = 0f;
    private bool isBoosting = false;
    public BoostController currentBoostController = null;

    private float currentSurfaceSpeed = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        originalBoostSpeed = boostSpeed;
        originalSpeedIncreaseRate = speedIncreaseRate;
    }

    void Update()
    {
        HandleJumpInput();
        UpdateUISpeedTexts();

        // 부스트 타이머 관리
        if (isBoosting)
        {
            boostTimer -= Time.deltaTime;
            if (boostTimer <= 0f)
            {
                boostSpeed = originalBoostSpeed;
                speedIncreaseRate = originalSpeedIncreaseRate;
                isBoosting = false;
                if (currentBoostController != null)
                    currentBoostController.SetCameraSize(false);
                currentBoostController = null;
            }
        }
    }

    private void FixedUpdate()
    {
        if (surfaceEffector2D != null)
        {
            float targetSpeed = 0f;

            // 부스트 중이면 무조건 boostSpeed로 즉시 가속
            if (isBoosting)
            {
                targetSpeed = boostSpeed;
            }
            // 아니면 평소처럼 오른쪽키+땅에서만 가속
            else if (isGrounded && Input.GetKey(KeyCode.RightArrow))
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

    public void ApplyBoost(float newBoostSpeed, float newRate, float duration, BoostController boostController)
    {
        boostSpeed = newBoostSpeed;
        speedIncreaseRate = newRate;
        boostTimer = duration;
        isBoosting = true;
        currentBoostController = boostController;
        if (currentBoostController != null)
            currentBoostController.SetCameraSize(true);
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
                rb.AddTorque(torqueAmount * inputDir, ForceMode2D.Force);
            }
            else
            {
                // 감쇠 적용: 회전 속도를 점점 줄임
                rb.angularVelocity *= 0.9f; // 0.9~0.95 정도가 자연스러움, 값은 취향에 따라 조절
                if (Mathf.Abs(rb.angularVelocity) < 0.1f)
                    rb.angularVelocity = 0f; // 완전히 멈추면 0으로
            }
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

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
             isGrounded = true;
            
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
        // Boost 관련 코드는 BoostController에서 처리
    }
}
