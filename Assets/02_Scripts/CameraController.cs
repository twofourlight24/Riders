using UnityEngine;
using Unity.Cinemachine;

public class CameraController : MonoBehaviour
{
    public CinemachineCamera virtualCamera; // 인스펙터에서 할당
    public Transform target; // 따라갈 타겟

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        if (virtualCamera != null)
        {
            virtualCamera.Follow = target;
            virtualCamera.LookAt = target;
        }
    }
}