using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private LayerMask groundLayer;

    [SerializeField] private Transform PlayerSquare;
    [SerializeField] private Transform feetPos;

    [SerializeField] private float groundDisctance = 0.25f;
    [SerializeField] private float jumpTime = 0.3f;

    [SerializeField] private float crouchHeight = 0.5f;

    private bool isGround = false;
    private bool isJumping = false;
    private float jumpTimer;

    private void Update()
    {
        isGround = Physics2D.OverlapCircle(feetPos.position, groundDisctance, groundLayer);

        // JUMPING
        if (isGround && Input.GetButtonDown("Jump"))
        {
            isJumping = true;
            rb.linearVelocity = Vector2.up * jumpForce;
        }

        if (isJumping && Input.GetButton("Jump"))
        {
            if (jumpTimer < jumpTime)
            {
                rb.linearVelocity = Vector2.up * jumpForce;
                jumpTimer += Time.deltaTime;
            }
            else isJumping = false;
        }

        if (Input.GetButtonUp("Jump"))
        {
            isJumping = false;
            jumpTimer = 0f;
        }

        // CROUCHING
        if (isGround && Input.GetButton("Crouch"))
        {
            PlayerSquare.localScale = new Vector3(PlayerSquare.localScale.x, crouchHeight, PlayerSquare.localScale.z);
            if (isJumping)
                PlayerSquare.localScale = new Vector3(PlayerSquare.localScale.x, 1f, PlayerSquare.localScale.z);
        }

        if (Input.GetButtonUp("Crouch"))
        {
            PlayerSquare.localScale = new Vector3(PlayerSquare.localScale.x, 1f, PlayerSquare.localScale.z);
        }
    }
}
