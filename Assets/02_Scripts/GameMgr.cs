using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameMgr : MonoBehaviour
{
    // �̱��� �ν��Ͻ�
    public static GameMgr Instance { get; private set; }

    public float elapsedTime = 0f; // ��� �ð� 
    public float fasetTime = float.MaxValue; // �ִ� �ð�
    public Text timerText; // UI �ؽ�Ʈ ������Ʈ


    // �ߺ� �ν��Ͻ� ����
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

    // Start�� MonoBehaviour�� ������ �� ù Update ���� �� �� ȣ��˴ϴ�.
    void Start()
    {
        
    }

    // Update�� �� �����Ӹ��� ȣ��˴ϴ�.
    void Update()
    {
        elapsedTime += Time.deltaTime; // ��� �ð� ������Ʈ
       UIMgr.Instance.UpdateTime(FormatTime(elapsedTime)); // UI ������Ʈ
    }
    public void GameStop()
    {
        // ���� ����
        Time.timeScale = 0f;

        // currrent,fasetTime ���� �� ��
        if (elapsedTime < fasetTime)
        {
            fasetTime = elapsedTime; // ���ο� �ְ� ��� ����
        }
        UIMgr.Instance.UpdateCurrentTimeText("Current Time : " + FormatTime(elapsedTime)); // ���� �ð� UI ������Ʈ
        UIMgr.Instance.UpdateFastestTimeText("Fastest Time : " + FormatTime(fasetTime)); // �ְ� ��� UI ������Ʈ
        elapsedTime = 0f; // ��� �ð� �ʱ�ȭ
        
        // �г� Ȱ��ȭ 
        UIMgr.Instance.ShowPanel();


    }
    public void GameStart()
    {
        // ���� ����
        UIMgr.Instance.HidePanel(); // �г� ����
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        elapsedTime = 0f; // ��� �ð� �ʱ�ȭ
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        int milliseconds = Mathf.FloorToInt((time * 100) % 1000);
        return string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
    }
}
