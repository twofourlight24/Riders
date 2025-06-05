using UnityEngine;

public class Coin : MonoBehaviour
{
    private ParticleSystem particleSystem;
    private AudioSource audioSource;

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
        if (particleSystem != null)
        {
            particleSystem.Play();
        }
        if (audioSource != null)
        {
            audioSource.Play();
        }
    }
}
