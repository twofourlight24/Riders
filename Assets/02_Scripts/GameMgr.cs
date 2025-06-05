using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameMgr : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static GameMgr Instance { get; private set; }

    public float elapsedTime = 0f; // 경과 시간 
    public float fasetTime = float.MaxValue; // 최대 시간
    public Text timerText; // UI 텍스트 컴포넌트


    // 중복 인스턴스 방지
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    // Start는 MonoBehaviour가 생성된 후 첫 Update 전에 한 번 호출됩니다.
    void Start()
    {
        
    }

    // Update는 매 프레임마다 호출됩니다.
    void Update()
    {
        elapsedTime += Time.deltaTime; // 경과 시간 업데이트
       UIMgr.Instance.UpdateTime(FormatTime(elapsedTime)); // UI 업데이트
    }
    public void GameStop()
    {
        // 게임 중지
        Time.timeScale = 0f;

        // currrent,fasetTime 저장 및 비교
        if (elapsedTime < fasetTime)
        {
            fasetTime = elapsedTime; // 새로운 최고 기록 갱신
        }
        UIMgr.Instance.UpdateCurrentTimeText("Current Time : " + FormatTime(elapsedTime)); // 현재 시간 UI 업데이트
        UIMgr.Instance.UpdateFastestTimeText("Fastest Time : " + FormatTime(fasetTime)); // 최고 기록 UI 업데이트
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
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        int milliseconds = Mathf.FloorToInt((time * 100) % 1000);
        return string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
    }
}
