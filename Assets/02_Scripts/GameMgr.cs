using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameMgr : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static GameMgr Instance { get; private set; }

    public float elapsedTime = 0f; // 경과 시간 
    public static float fasetTime = 0f; // 최대 시간
    public int g_Score = 0; // 현재 점수
    public static int MaxScore = 0; // 최고 점수
    int g_Coin = 0;
    public Text timerText; // UI 텍스트 컴포넌트

    // === 배경음악 추가 ===
    public AudioClip bgmClip; // 인스펙터에서 할당할 배경음악
    private AudioSource bgmSource;

    // 중복 인스턴스 방지
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // === 배경음악 AudioSource 설정 ===
            bgmSource = gameObject.AddComponent<AudioSource>();
            bgmSource.clip = bgmClip;
            bgmSource.loop = true;
            bgmSource.playOnAwake = false;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    // Start는 MonoBehaviour가 생성된 후 첫 Update 전에 한 번 호출됩니다.
    void Start()
    {
        // === 배경음악 재생 ===
        if (bgmSource != null && bgmClip != null && !bgmSource.isPlaying)
        {
            bgmSource.Play();
        }
    }

    // Update는 매 프레임마다 호출됩니다.
    void Update()
    {
        elapsedTime += Time.deltaTime; // 경과 시간 업데이트
        UIMgr.Instance.UpdateTime(FormatTime(elapsedTime)); // UI 업데이트
        if (g_Coin >= 100) // 코인이 100 이상이면 게임 클리어
        {
            g_Score += 100; // 점수 100점 추가
        }
    }
    public void GameStop(bool isClear = false)
    {
        // 게임 중지
        Time.timeScale = 0f;

        // currrent,fasetTime 저장 및 비교
        if (elapsedTime < fasetTime)
        {
            fasetTime = elapsedTime; // 새로운 최고 기록 갱신
        }
        if (g_Score > MaxScore)
        {
            MaxScore = g_Score; // 새로운 최고 점수 갱신
        }
        UIMgr.Instance.UpdateCurrentTimeText("Current Time : " + FormatTime(elapsedTime)); // 현재 시간 UI 업데이트
        UIMgr.Instance.UpdateFastestTimeText("Fastest Time : " + FormatTime(fasetTime)); // 최고 기록 UI 업데이트
        UIMgr.Instance.UpdatePanelScore("Score : " + g_Score + "  BestScore : " + MaxScore); // 패널 점수 업데이트
        UIMgr.Instance.UpdatePanelCoin("Coin : 100/ " + g_Coin); // 패널 코인 업데이트

        // 패널 타이틀 업데이트
        if (isClear)
            UIMgr.Instance.UpdatePanelTitle("Game Clear");
        else
            UIMgr.Instance.UpdatePanelTitle("Game Over");

        elapsedTime = 0f; // 경과 시간 초기화

        // 패널 활성화 
        UIMgr.Instance.ShowPanel();
    }
    public void GameStart()
    {
        // 게임 시작
        UIMgr.Instance.HidePanel(); // 패널 숨김
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        elapsedTime = 0f; // 경과 시간 초기화
        g_Score = 0;
        g_Coin = 0; // 코인 초기화
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        int milliseconds = Mathf.FloorToInt((time * 100) % 1000);
        return string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
    }
    public void Score()
    {
        g_Score += 1; // 점수 증가
        UIMgr.Instance.UpdateScore("" + g_Score); // UI 업데이트
    }
    public void Coin()
    {
        if (g_Coin < 100) // 코인이 100을 초과하면 100으로 제한
        {
            g_Coin += 1;
        }
        UIMgr.Instance.UpdateCoin("100 / " + g_Coin); // UI 업데이트
    }

}
