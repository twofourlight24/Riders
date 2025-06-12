using UnityEngine;
using Unity.Cinemachine;

public class BoostController : MonoBehaviour
{
    public float boostSpeedValue = 20f;
    public float boostRateValue = 40f;
    public float boostDuration = 2f;
    public float boostCameraSize = 16f;
    public float defaultCameraSize = 11f;
    public CinemachineVirtualCamera virtualCamera; // 인스펙터에서 할당

    private void OnTriggerEnter2D(Collider2D other)
    {
        CarController car = other.GetComponent<CarController>();
        if (car != null)
        {
            car.ApplyBoost(boostSpeedValue, boostRateValue, boostDuration, this);
        }
    }

    // 카메라 사이즈 변경용 메서드
    public void SetCameraSize(bool isBoost)
    {
        if (virtualCamera != null)
            virtualCamera.m_Lens.OrthographicSize = isBoost ? boostCameraSize : defaultCameraSize;
    }
}