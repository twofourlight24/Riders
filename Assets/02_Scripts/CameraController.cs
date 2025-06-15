using UnityEngine;
using Unity.Cinemachine;

public class CameraController : MonoBehaviour
{
    public CinemachineCamera virtualCamera; // �ν����Ϳ��� �Ҵ�
    public Transform target; // ���� Ÿ��

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