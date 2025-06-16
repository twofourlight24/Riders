using Unity.Cinemachine; // 네임스페이스 
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI; // 추가된 네임스페이스

public class CarController : MonoBehaviour
{
    private SurfaceEffector2D surfaceEffector2D;
    private Rigidbody2D rb;
    public float jumpForce = 7f;
    public bool isGrounded = false;
    public float torqueAmount = 2f; // 회전 계수(최대값)
    public float maxAngularVelocity = 100f; // 최고 회전 가속도 제한
    public float baseSpeed = 15f; // 기본 속도(원하는 값으로 초기화)
    private float currentBoostSpeed = 0f; // 부스트 시 속도, 0이면 부스트 아님
    private float boostTimer = 0f;
    private float currentSurfaceSpeed = 0f;

    private float lastAirborneAngle = 0f;
    private float airborneRotation = 0f;
    private bool wasAirborne = false;
    private int pendingScore = 0;

    private Text airScoreText; // UI 텍스트 컴포넌트
    public float reloadDelay = 2f; // 재시작 지연 시간
    private AudioSource audioSource;
    public AudioClip crashSound; // 충돌 사운드
    public AudioClip roadSound;
    public float maxRoadSoundVolume = 1f;
    public ParticleSystem CrashEffect;

    private float roadSoundFadeTime = 1.5f; // 볼륨이 커지는 시간(초)
    private float roadSoundFadeTimer = 0f;
    private bool isRoadSoundFadingIn = false;
    private bool isRoadSoundPlaying = false; // 멤버 변수 추가

    public float boostSpeed = 20f;
    public float speedIncreaseRate = 20f;
    private float originalSpeedIncreaseRate;
    private bool isBoosting = false;
    private BoostController currentBoostController = null;

    private bool hasCrashed = false; 

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        airScoreText = GameObject.Find("AirScoreText").GetComponent<Text>();
        audioSource = GetComponent<AudioSource>();
        originalSpeedIncreaseRate = speedIncreaseRate;
    }

    void Update()
    {
        HandleJumpInput();
        UpdateUISpeedTexts();

        if (isBoosting)
        {
            boostTimer -= Time.deltaTime;
            if (boostTimer <= 0f)
            {
                boostSpeed = baseSpeed;
                speedIncreaseRate = originalSpeedIncreaseRate;
                isBoosting = false;
                if (currentBoostController != null)
                    currentBoostController.SetCameraSize(false);
                currentBoostController = null;
            }
        }

        // 부스트 타이머 관리
        if (boostTimer > 0f)
        {
            boostTimer -= Time.deltaTime;
            if (boostTimer <= 0f)
            {
                currentBoostSpeed = 0f; // 부스트 종료
            }
        }

        // --- 도로 사운드: 땅에 있고, 속도가 0.1 이상일 때만 점진적으로 커지며 재생 ---
        if (isGrounded && rb != null && rb.linearVelocity.magnitude > 0.1f && Time.timeScale ==1f)
        {
            if (!isRoadSoundPlaying)
            {
                audioSource.clip = roadSound;
                audioSource.volume = 0f;
                audioSource.loop = true;
                audioSource.Play();
                isRoadSoundFadingIn = true;
                roadSoundFadeTimer = 0f;
                isRoadSoundPlaying = true;
            }
            // 볼륨 점진적 증가
            if (isRoadSoundFadingIn)
            {
                roadSoundFadeTimer += Time.deltaTime;
                float t = Mathf.Clamp01(roadSoundFadeTimer / roadSoundFadeTime);
                audioSource.volume = Mathf.Lerp(0f, maxRoadSoundVolume, t);
                if (t >= 1f)
                {
                    isRoadSoundFadingIn = false;
                }
            }
        }
        else
        {
            if (isRoadSoundPlaying)
            {
                audioSource.Stop();
                isRoadSoundPlaying = false;
                isRoadSoundFadingIn = false;
                roadSoundFadeTimer = 0f;
            }
        }

        // --- 360도 회전 체크 및 임시 점수 UI 할당 ---
        if (!isGrounded)
        {
            if (!wasAirborne)
            {
                lastAirborneAngle = rb.rotation;
                airborneRotation = 0f;
                pendingScore = 0;
                UpdateAirScoreText(""); // 시작 시 초기화
                wasAirborne = true;
            }
            else
            {
                float currentAngle = rb.rotation;
                float deltaAngle = Mathf.DeltaAngle(lastAirborneAngle, currentAngle);
                airborneRotation += deltaAngle;
                lastAirborneAngle = currentAngle;

                int beforeScore = pendingScore;
                while (Mathf.Abs(airborneRotation) >= 360f)
                {
                    pendingScore += 1;
                    airborneRotation -= 360f * Mathf.Sign(airborneRotation);
                }
                if (pendingScore != beforeScore)
                {
                    UpdateAirScoreText($"+{pendingScore}");
                }
            }
            UpdateAirScoreTextPosition();
        }
        else
        {
            if (wasAirborne)
            {
                for (int i = 0; i < pendingScore; i++)
                {
                    GameMgr.Instance.Score();
                }
                UpdateAirScoreText(""); // 텍스트 숨김
                pendingScore = 0;
                airborneRotation = 0f;
                wasAirborne = false;
            }
        }
    }

    private void FixedUpdate()
    {
        if (surfaceEffector2D != null)
        {
            float targetSpeed = 0f;

            if (isBoosting)
            {
                targetSpeed = boostSpeed;
            }
            else if (isGrounded && Input.GetKey(KeyCode.RightArrow))
            {
                targetSpeed = baseSpeed;
            }
            else
            {
                targetSpeed = 0f; // 가속키를 안누르면 멈춤
            }

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

    private void UpdateAirScoreTextPosition()
    {
        if (airScoreText != null)
        {
            Camera cam = Camera.main;
            if (cam != null)
            {
                // 자동차의 오른쪽(월드 기준)으로 1.5만큼 이동
                Vector3 worldPos = transform.position + Vector3.right * 3f + Vector3.up*1f;
                Vector3 screenPos = cam.WorldToScreenPoint(worldPos);
                airScoreText.transform.position = screenPos;
            }
        }
    }

    public void UpdateAirScoreText(string text) // 추가된 메서드
    {
        airScoreText.text = text;
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
            PlayRoadSound();
        }
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            HandleCrash();
            return;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
            audioSource.Stop();
            isRoadSoundFadingIn = false;
            roadSoundFadeTimer = 0f;
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
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Obstacle"))
        {
            HandleCrash();
            return;
        }
    }

    private void PlayRoadSound()
    {
        audioSource.clip = roadSound;
        audioSource.volume = 0f;
        audioSource.loop = false;
        audioSource.Play();
        isRoadSoundFadingIn = true;
        roadSoundFadeTimer = 0f;
    }

    private void HandleCrash()
    {
        if (hasCrashed) return;
        hasCrashed = true;
        CrashEffect.Play();
        audioSource.PlayOneShot(crashSound); // 충돌 사운드 재생
        Invoke(nameof(StopGame), reloadDelay); // 지연 후 재시작
    }

    private void StopGame()
    {
        GameMgr.Instance.GameStop(false);
    }
}
