using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class KillZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameMgr.Instance.GameStop(); 
    }
}
