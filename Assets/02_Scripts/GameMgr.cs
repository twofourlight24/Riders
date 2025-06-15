using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameMgr : MonoBehaviour
{
    // �̱��� �ν��Ͻ�
    public static GameMgr Instance { get; private set; }

    public float elapsedTime = 0f; // ��� �ð� 
    public static float fasetTime = 0f; // �ִ� �ð�
    public int g_Score = 0; // ���� ����
    public static int MaxScore = 0; // �ְ� ����
    int g_Coin = 0;
    public Text timerText; // UI �ؽ�Ʈ ������Ʈ

    // === ������� �߰� ===
    public AudioClip bgmClip; // �ν����Ϳ��� �Ҵ��� �������
    private AudioSource bgmSource;

    // �ߺ� �ν��Ͻ� ����
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // === ������� AudioSource ���� ===
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

    // Start�� MonoBehaviour�� ������ �� ù Update ���� �� �� ȣ��˴ϴ�.
    void Start()
    {
        // === ������� ��� ===
        if (bgmSource != null && bgmClip != null && !bgmSource.isPlaying)
        {
            bgmSource.Play();
        }
    }

    // Update�� �� �����Ӹ��� ȣ��˴ϴ�.
    void Update()
    {
        elapsedTime += Time.deltaTime; // ��� �ð� ������Ʈ
        UIMgr.Instance.UpdateTime(FormatTime(elapsedTime)); // UI ������Ʈ
        if (g_Coin >= 100) // ������ 100 �̻��̸� ���� Ŭ����
        {
            g_Score += 100; // ���� 100�� �߰�
        }
    }
    public void GameStop(bool isClear = false)
    {
        // ���� ����
        Time.timeScale = 0f;

        // currrent,fasetTime ���� �� ��
        if (elapsedTime < fasetTime)
        {
            fasetTime = elapsedTime; // ���ο� �ְ� ��� ����
        }
        if (g_Score > MaxScore)
        {
            MaxScore = g_Score; // ���ο� �ְ� ���� ����
        }
        UIMgr.Instance.UpdateCurrentTimeText("Current Time : " + FormatTime(elapsedTime)); // ���� �ð� UI ������Ʈ
        UIMgr.Instance.UpdateFastestTimeText("Fastest Time : " + FormatTime(fasetTime)); // �ְ� ��� UI ������Ʈ
        UIMgr.Instance.UpdatePanelScore("Score : " + g_Score + "  BestScore : " + MaxScore); // �г� ���� ������Ʈ
        UIMgr.Instance.UpdatePanelCoin("Coin : 100/ " + g_Coin); // �г� ���� ������Ʈ

        // �г� Ÿ��Ʋ ������Ʈ
        if (isClear)
            UIMgr.Instance.UpdatePanelTitle("Game Clear");
        else
            UIMgr.Instance.UpdatePanelTitle("Game Over");

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
        g_Score = 0;
        g_Coin = 0; // ���� �ʱ�ȭ
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
        g_Score += 1; // ���� ����
        UIMgr.Instance.UpdateScore("" + g_Score); // UI ������Ʈ
    }
    public void Coin()
    {
        if (g_Coin < 100) // ������ 100�� �ʰ��ϸ� 100���� ����
        {
            g_Coin += 1;
        }
        UIMgr.Instance.UpdateCoin("100 / " + g_Coin); // UI ������Ʈ
    }

}
