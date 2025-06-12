using UnityEngine;
using Unity.Cinemachine;

public class BoostController : MonoBehaviour
{
    public float boostSpeedValue = 20f;
    public float boostRateValue = 40f;
    public float boostDuration = 2f;
    public float boostCameraSize = 16f;
    public float defaultCameraSize = 11f;
    public CinemachineVirtualCamera virtualCamera; // �ν����Ϳ��� �Ҵ�

    private void OnTriggerEnter2D(Collider2D other)
    {
        CarController car = other.GetComponent<CarController>();
        if (car != null)
        {
            car.ApplyBoost(boostSpeedValue, boostRateValue, boostDuration, this);
        }
    }

    // ī�޶� ������ ����� �޼���
    public void SetCameraSize(bool isBoost)
    {
        if (virtualCamera != null)
            virtualCamera.m_Lens.OrthographicSize = isBoost ? boostCameraSize : defaultCameraSize;
    }
}