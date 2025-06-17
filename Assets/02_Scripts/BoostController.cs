using UnityEngine;


public class BoostController : MonoBehaviour
{
    public float boostSpeedValue = 20f;
    public float boostRateValue = 40f;
    public float boostDuration = 2f;
    public float boostCameraSize = 16f;
    public float defaultCameraSize = 11f;


    private void OnTriggerEnter2D(Collider2D other)
    {
        CarController car = other.GetComponent<CarController>();
        if (car != null)
        {
            car.ApplyBoost(boostSpeedValue, boostRateValue, boostDuration, this);
        }
    }

}