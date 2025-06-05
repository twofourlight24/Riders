using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class KillZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameMgr.Instance.GameStop(); // 최고 기록 갱신
    }
}
