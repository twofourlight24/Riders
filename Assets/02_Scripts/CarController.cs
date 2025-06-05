using UnityEngine;

public class CarController : MonoBehaviour
{
    private SurfaceEffector2D surfaceEffector2D;
    private Rigidbody2D rb;
    public float jumpForce = 7f; // 점프 힘 조절용

    private bool isGrounded = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (surfaceEffector2D != null)
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                surfaceEffector2D.speed = 10f;
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                surfaceEffector2D.speed = 5f;
            }
            UIMgr.Instance.UpdateSurfaceSpeedText($"Surface Speed: {surfaceEffector2D.speed:F1}");
        }

        // 스페이스바로 점프
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && rb != null)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            isGrounded = false;
        }
        UIMgr.Instance.UpdateCarSpeedText($"Car Speed: {rb.linearVelocity.magnitude:F1}");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent<SurfaceEffector2D>(out var effecet))
        {
            surfaceEffector2D = effecet;
            Debug.Log($"Collision with {collision.gameObject.name} detected. SurfaceEffector2D: {surfaceEffector2D.speed}");    
        }

        // 바닥에 닿았을 때 점프 가능
        if (collision.contacts.Length > 0 && collision.contacts[0].normal.y > 0.5f)
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        // 바닥에서 떨어지면 점프 불가
        isGrounded = false;
    }
}
