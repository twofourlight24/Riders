using UnityEngine;

public class ObstacleSpin : MonoBehaviour
{
    public float rotationSpeed = 200f; // Speed of rotation in degrees per second
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.Rotate(Vector3.back, rotationSpeed * Time.deltaTime);
    }
}
