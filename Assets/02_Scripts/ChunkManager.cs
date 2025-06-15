using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    public List<GameObject> chunkPrefabs; // 미리 만들어놓은 청크들
    public Transform player; // 플레이어 Transform (직접 할당)
    public GameObject playerPrefab; // 플레이어 프리팹 (생성할 플레이어)
    public GameObject baseChunk; // 하이어라키에 배치된 기본 청크(첫 청크)
    public float spawnDistance = 30f; // 플레이어 앞 몇 미터까지 청크를 미리 생성할지

    public CameraController cameraController; // 인스펙터에서 할당

    public GameObject finishChunkPrefab; // FinishChunk 프리팹을 인스펙터에서 할당

    private Vector3 nextSpawnPosition;
    private List<GameObject> activeChunks = new List<GameObject>();
    private bool playerSpawned = false; // 플레이어가 생성되었는지 확인하는 플래그

    // 추가: 마지막으로 점수를 준 청크 인덱스
    private int lastScoredChunkIndex = -1;

    private int chunkCount = 0; // 생성된 청크 개수
    private bool finishChunkSpawned = false; // FinishChunk가 생성되었는지 여부

    void Start()
    {
        // 기본 청크를 activeChunks에 추가
        if (baseChunk != null)
        {
            activeChunks.Add(baseChunk);

            // ExitPoint를 찾아서 nextSpawnPosition 설정
            Transform exitPoint = baseChunk.transform.Find("ExitPoint");
            if (exitPoint != null)
            {
                nextSpawnPosition = exitPoint.position;
            }
        }

        // 추가 청크 미리 생성
        for (int i = 0; i < 4; i++)
        {
            SpawnNextChunk();
        }

        // 모든 초기 청크가 생성된 후 플레이어를 생성합니다.
        SpawnPlayerInFirstChunk();
    }

    void Update()
    {
        // 플레이어가 아직 생성되지 않았거나, 플레이어 Transform이 null이면 업데이트 로직을 실행하지 않습니다.
        if (player == null || !playerSpawned) 
        {
            return;
        }

        if (Vector3.Distance(player.position, nextSpawnPosition) < spawnDistance)
        {
            SpawnNextChunk();
        }

        // 오래된 청크 삭제
        if (activeChunks.Count > 0)
        {
            if (player.position.z - activeChunks[0].transform.position.z > spawnDistance * 2)
            {
                Destroy(activeChunks[0]);
                activeChunks.RemoveAt(0);
            }
        }

        // === 청크 통과 시 점수 부여 ===
        for (int i = lastScoredChunkIndex + 1; i < activeChunks.Count; i++)
        {
            GameObject chunk = activeChunks[i];
            Transform exitPoint = chunk.transform.Find("ExitPoint");
            if (exitPoint != null)
            {
                // 플레이어가 ExitPoint를 지났는지 확인 (x축 기준)
                if (player.position.x > exitPoint.position.x)   
                {
                    GameMgr.Instance.Score(); // 점수 1점 추가
                    lastScoredChunkIndex = i;
                }
                else
                {
                    break;
                }
            }
        }
    }

    void SpawnNextChunk()
    {
        // 100개가 생성되면 FinishChunk만 생성하고, 그 이후로는 아무것도 생성하지 않음
        if (chunkCount >= 100 && !finishChunkSpawned)
        {
            GameObject chunk = Instantiate(finishChunkPrefab, Vector3.zero, Quaternion.identity);

            Transform startPoint = chunk.transform.Find("StartPoint");
            Transform exitPoint = chunk.transform.Find("ExitPoint");

            if (startPoint != null && exitPoint != null)
            {
                Vector3 offset = nextSpawnPosition - startPoint.position;
                chunk.transform.position += offset;
                nextSpawnPosition = exitPoint.position;
            }
            else
            {
                chunk.transform.position = nextSpawnPosition;
                nextSpawnPosition = chunk.transform.position;
            }

            activeChunks.Add(chunk);
            finishChunkSpawned = true;
            return;
        }
        else if (chunkCount >= 100)
        {
            return;
        }

        // === 청크가 5개 이상이면 맨 앞 청크 삭제 ===
        if (activeChunks.Count >= 5)
        {
            Destroy(activeChunks[0]);
            activeChunks.RemoveAt(0);
            lastScoredChunkIndex = Mathf.Max(-1, lastScoredChunkIndex - 1); // 점수 인덱스 보정
        }

        // 일반 청크 생성
        GameObject prefab = chunkPrefabs[Random.Range(0, chunkPrefabs.Count)];
        GameObject normalChunk = Instantiate(prefab, Vector3.zero, Quaternion.identity);

        Transform normalStartPoint = normalChunk.transform.Find("StartPoint");
        Transform normalExitPoint = normalChunk.transform.Find("ExitPoint");

        if (normalStartPoint != null && normalExitPoint != null)
        {
            Vector3 offset = nextSpawnPosition - normalStartPoint.position;
            normalChunk.transform.position += offset;
            nextSpawnPosition = normalExitPoint.position;
        }
        else
        {
            normalChunk.transform.position = nextSpawnPosition;
            nextSpawnPosition = normalChunk.transform.position;
        }

        activeChunks.Add(normalChunk);
        chunkCount++;
    }

    void SpawnPlayerInFirstChunk()
    {
        if (activeChunks.Count > 0 && !playerSpawned)
        {
            GameObject firstChunk = activeChunks[0];
            Transform spawnPoint = firstChunk.transform.Find("SpawnPoint");

            if (spawnPoint != null)
            {
                // 플레이어 프리팹을 SpawnPoint 위치에 생성합니다.
                player = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation).transform;
                playerSpawned = true; 
                Debug.Log("Player spawned at: " + spawnPoint.position);

                if (cameraController != null)
                {
                    cameraController.SetTarget(player);
                }
            }
        }
    }
}