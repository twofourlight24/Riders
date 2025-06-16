using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class UIMgr : MonoBehaviour
{
    public static UIMgr Instance { get; private set; }
    public Text TimeText;
    public Text SurfaceSpeedText;
    public Text CarSpeedText;
    public Text CurrentTimeText;
    public Text FastestTimeText;
    public Text ScoreText;
    public Text CoinText; // 코인 텍스트
    public Text PanelScoreText;
    public Text PanelCoinText; // 패널 코인 텍스트
    public Text PanelTitleText;
    public GameObject Panel;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void UpdateTime(string time)
    {
        if (TimeText != null)
        {
            TimeText.text = time;
        }
    }

    public void UpdateSurfaceSpeedText(string speed)
    {
        SurfaceSpeedText.text = speed;
    }
    public void UpdateCarSpeedText(string speed)
    {
        CarSpeedText.text = speed;
    }
    public void UpdateCurrentTimeText(string time)
    {
        CurrentTimeText.text = time;
    }
    public void UpdateFastestTimeText(string time)
    {
        FastestTimeText.text = time;
    }
    public void ShowPanel()
    {
        if (Panel != null)
        {
            Panel.SetActive(true);
        }
    }
    public void HidePanel()
    {
        if (Panel != null)
        {
            Panel.SetActive(false);
        }
    }
    public void UpdateScore(string score)
    {
        ScoreText.text = score;
    }
    public void UpdateCoin(string coin)
    {
        CoinText.text = coin;
    }
    public void UpdatePanelScore(string score)
    {
        PanelScoreText.text = score;
    }
    public void UpdatePanelCoin(string coin)
    {
        PanelCoinText.text = coin;
    }
    public void UpdatePanelTitle(string title)
    {
        PanelTitleText.text = title;
    }   

    public void GameRestart()
    {
        GameMgr.Instance.GameStart(); // 게임 시작
    }
    public void GameQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
