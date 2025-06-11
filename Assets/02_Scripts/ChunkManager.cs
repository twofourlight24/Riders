using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine; // Cinemachine을 사용하기 위해 추가

public class ChunkManager : MonoBehaviour
{
    public List<GameObject> chunkPrefabs; // 미리 만들어놓은 청크들
    public Transform player; // 플레이어 Transform (이제 직접 할당하지 않고 코드로 설정)
    public GameObject playerPrefab; // 플레이어 프리팹 (생성할 플레이어)
    public float spawnDistance = 30f; // 플레이어 앞 몇 미터까지 청크를 미리 생성할지

    // 추가: Cinemachine Virtual Camera 레퍼런스
    public CinemachineCamera virtualCamera; // 인스펙터에서 할당할 가상 카메라

    private Vector3 nextSpawnPosition;
    private List<GameObject> activeChunks = new List<GameObject>();
    private bool playerSpawned = false; // 플레이어가 생성되었는지 확인하는 플래그

    void Start()
    {
        nextSpawnPosition = Vector3.zero;

        // 처음에 5개의 청크를 미리 생성합니다.
        for (int i = 0; i < 5; i++)
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

        // 플레이어가 nextSpawnPosition에서 spawnDistance 미만으로 가까워지면 청크 생성
        if (Vector3.Distance(player.position, nextSpawnPosition) < spawnDistance)
        {
            SpawnNextChunk();
        }

        // 플레이어로부터 일정 거리 이상 멀어진 오래된 청크 삭제
        if (activeChunks.Count > 0)
        {
            if (player.position.z - activeChunks[0].transform.position.z > spawnDistance * 2)
            {
                Destroy(activeChunks[0]);
                activeChunks.RemoveAt(0);
            }
        }
    }

    void SpawnNextChunk()
    {
        GameObject prefab = chunkPrefabs[Random.Range(0, chunkPrefabs.Count)];
        GameObject chunk = Instantiate(prefab, Vector3.zero, Quaternion.identity); 

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
            Debug.LogWarning("Chunk prefab " + prefab.name + " is missing StartPoint or ExitPoint! Chunk will not be placed correctly.");
            chunk.transform.position = nextSpawnPosition;
        }

        activeChunks.Add(chunk);
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

                // ⭐ 중요: Cinemachine Virtual Camera의 Target을 새로 생성된 플레이어로 설정 ⭐
                if (virtualCamera != null)
                {
                    virtualCamera.Follow = player;
                    virtualCamera.LookAt = player;
                    Debug.Log("Cinemachine Virtual Camera's Follow and LookAt targets set to Player.");
                }
                else
                {
                    Debug.LogError("Cinemachine Virtual Camera is not assigned! Please assign it in the Inspector.");
                }
            }
            else
            {
                Debug.LogError("First chunk (" + firstChunk.name + ") is missing a 'SpawnPoint' child. Player cannot be spawned.");
            }
        }
    }
}