using UnityEngine;

public class Coin : MonoBehaviour
{
    private ParticleSystem particleSystem;
    private AudioSource audioSource;
    public bool isCollected = false; // ������ �����Ǿ����� ���θ� ��Ÿ���� ����

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // ���� ���� ������Ʈ�� �ִ� ParticleSystem, AudioSource ������Ʈ ��������
        particleSystem = GetComponent<ParticleSystem>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Ʈ���ſ� ����� �� ȣ���
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isCollected) isCollected = true; // ������ �����Ǿ����� ǥ��
        else return; // �̹� ������ ������ ����
        if (particleSystem != null)
        {
            particleSystem.Play();
        }
        if (audioSource != null)
        {
            audioSource.Play();
        }
        GameMgr.Instance.Coin();
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            Color color = spriteRenderer.color;
            color.a = 0f; // ���İ��� 0���� �����Ͽ� �����ϰ� ����
            spriteRenderer.color = color;
        }
        Destroy(gameObject, 1f);
    }
}

