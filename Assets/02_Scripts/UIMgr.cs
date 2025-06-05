using UnityEngine;
using UnityEngine.UI;

public class UIMgr : MonoBehaviour
{
    public static UIMgr Instance { get; private set; }
    public Text TimeText;
    public Text SurfaceSpeedText;
    public Text CarSpeedText;
    public Text CurrentTimeText;
    public Text FastestTimeText;
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
    public void GameRestart()
    {
        GameMgr.Instance.GameStart(); // 게임 시작
    }
}
