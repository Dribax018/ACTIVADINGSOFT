using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerControllerOverride : MonoBehaviour
{
    [Header("Movimiento")]
    public Rigidbody2D rb;
    public Animator anim;
    public float moveSpeed = 5f;
    public float jumpForce = 10f;

    private void Start()
    {
        rb.freezeRotation = true;
    }

    private void Update()
    {
        HandleMovement();
        HandleJump();
    }

    private void HandleMovement()
    {
        float horizontal = 0f;

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            horizontal = 1f;
            transform.localScale = new Vector2(1, 1);
        }
        else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            horizontal = -1f;
            transform.localScale = new Vector2(-1, 1);
        }

        rb.linearVelocity = new Vector2(horizontal * moveSpeed, rb.linearVelocity.y);
        anim.SetBool("running", horizontal != 0);
    }

    private void HandleJump()
    {
        bool enSuelo = Mathf.Abs(rb.linearVelocity.y) < 0.01f;

        if (enSuelo && (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow)))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Collectible"))
        {
            Destroy(other.gameObject);
        }
    }


}
