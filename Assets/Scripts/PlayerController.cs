using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    float speed;
    public float initialSpeed = 5;
    public float finalSpeed = 10;
    public float overallAccel = 100;
    public float accel = 5;
    public float jumpPower = 20;

    public float maxJump = 10f;
    public float maxJumpMulti = 10f;
    float jumpStartHeight = 0f;

    float groundedSkin = 0.05f;
    public LayerMask mask;

    public float fallMulti = 10f;
    public float lowJumpMulti = 10f;

    bool jumpRequest = false;
    bool jumpingHeld = false;
    bool grounded;
    bool wasGrounded;

    Vector2 playerSize;
    Vector2 boxSize;

    Rigidbody2D rb;

    
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        playerSize = GetComponent<BoxCollider2D>().size;
        boxSize = new Vector2(playerSize.x - 0.2f, groundedSkin);

        maxJump = MainMenu.maxJump;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Cancel"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
        }


        if (Input.GetButtonUp("Jump"))
        {
            jumpingHeld = false;
        }

        if (transform.position.y < -40f)
        {
            transform.position = new Vector3(0f, 0f, 0);
            rb.velocity = new Vector2(0, 0);
        }
        else if ((Input.GetButtonDown("Jump") && grounded) || (Input.GetButton("Jump") && !wasGrounded && grounded))
        {
            jumpRequest = true;
            jumpingHeld = true;
        }
    }

    void FixedUpdate()
    {
        speed = finalSpeed - (finalSpeed - initialSpeed) * Mathf.Exp(-rb.position.x / overallAccel);
        if (jumpRequest)
        {
            jumpStartHeight = rb.position.y;
            
            // forwards motion plus the jump force
            rb.AddForce(new Vector2(accel * (speed - rb.velocity.x), jumpPower), ForceMode2D.Impulse);

            jumpRequest = false;
            grounded = false;
        }
        else
        {
            // The force, but only forwards
            rb.AddForce(new Vector2(accel * (speed - rb.velocity.x), 0f), ForceMode2D.Impulse);

            Vector2 boxCenter = (Vector2)transform.position + Vector2.down * (playerSize.y + boxSize.y) * 0.5f;
            wasGrounded = grounded;
            grounded = (Physics2D.OverlapBox(boxCenter, boxSize, 0f, mask) != null);
        }

        if (rb.velocity.y < 0)
        {
            rb.gravityScale = fallMulti;
        }
        else if (rb.position.y >= jumpStartHeight + maxJump)
        {
            rb.gravityScale = maxJumpMulti;
        }
        else if (rb.velocity.y > 0 && !jumpingHeld)
        {
            rb.gravityScale = lowJumpMulti;
        }
        else
        {
            rb.gravityScale = 1f;
        }      
    }
}