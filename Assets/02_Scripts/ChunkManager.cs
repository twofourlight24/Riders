using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    public List<GameObject> chunkPrefabs; // �̸� �������� ûũ��
    public Transform player;
    public float spawnDistance = 30f; // �÷��̾� �� �� ���ͱ��� ûũ�� �̸� ��������

    private Vector3 nextSpawnPosition;
    private List<GameObject> activeChunks = new List<GameObject>();

    void Start()
    {
        nextSpawnPosition = Vector3.zero;
        for (int i = 0; i < 5; i++) // ó���� �� �� �̸� ����
            SpawnNextChunk();
    }

    void Update()
    {
        if (Vector3.Distance(player.position, nextSpawnPosition) < spawnDistance)
            SpawnNextChunk();
    }

    void SpawnNextChunk()
    {
        GameObject prefab = chunkPrefabs[Random.Range(0, chunkPrefabs.Count)];
        GameObject chunk = Instantiate(prefab, nextSpawnPosition, Quaternion.identity);
        activeChunks.Add(chunk);

        // ���� ûũ�� ��� ������ ���
        Transform exitPoint = chunk.transform.Find("ExitPoint");
        nextSpawnPosition = exitPoint.position;

        // ������ ûũ ����
        if (activeChunks.Count > 10)
        {
            Destroy(activeChunks[0]);
            activeChunks.RemoveAt(0);
        }
    }
}
