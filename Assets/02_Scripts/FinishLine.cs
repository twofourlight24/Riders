using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishLine : MonoBehaviour
{
    public ParticleSystem FinishEffect;
    public AudioSource audioSource;
    public float reloadDelay = 2f;
    bool isFinished = false;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isFinished)
        {
            isFinished = true;
            FinishEffect.Play();
            Invoke(nameof(StopGame), reloadDelay);
            audioSource.Play();
        }
    }
    private void StopGame()
    {
        GameMgr.Instance.GameStop(true);
    }
}
