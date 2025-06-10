using UnityEngine;

public class Coin : MonoBehaviour
{
    private ParticleSystem particleSystem;
    private AudioSource audioSource;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 같은 게임 오브젝트에 있는 ParticleSystem, AudioSource 컴포넌트 가져오기
        particleSystem = GetComponent<ParticleSystem>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 트리거에 닿았을 때 호출됨
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (particleSystem != null)
        {
            particleSystem.Play();
        }
        if (audioSource != null)
        {
            audioSource.Play();
        }
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            Color color = spriteRenderer.color;
            color.a = 0f; // 알파값을 0으로 설정하여 투명하게 만듦
            spriteRenderer.color = color;
        }
        Destroy(gameObject, 1f);
    }
}

