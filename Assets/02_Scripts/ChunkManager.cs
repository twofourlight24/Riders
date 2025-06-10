using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    public List<GameObject> chunkPrefabs; // 미리 만들어놓은 청크들
    public Transform player;
    public float spawnDistance = 30f; // 플레이어 앞 몇 미터까지 청크를 미리 생성할지

    private Vector3 nextSpawnPosition;
    private List<GameObject> activeChunks = new List<GameObject>();

    void Start()
    {
        nextSpawnPosition = Vector3.zero;
        for (int i = 0; i < 5; i++) // 처음에 몇 개 미리 생성
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

        // 다음 청크가 어디 붙을지 계산
        Transform exitPoint = chunk.transform.Find("ExitPoint");
        nextSpawnPosition = exitPoint.position;

        // 오래된 청크 삭제
        if (activeChunks.Count > 10)
        {
            Destroy(activeChunks[0]);
            activeChunks.RemoveAt(0);
        }
    }
}
