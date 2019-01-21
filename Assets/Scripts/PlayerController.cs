using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed;
    public float initialSpeed = 5;
    public float finalSpeed = 10;
    public float overallAccel = 100;
    public float accel = 5;
    public float jumpPower = 20;
    public float groundedSkin = 0.05f;
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

    private ObjectPooler objectPooler;
    // public float nextEdge;    // The next right edge the player will encounter
    float lastEdge;    // The furthest right egde yet 
    float lastHeight;
    float baseHeight;

    public GameObject groundSmall;
    float smallWidth;
    public GameObject groundMedium;
    float mediumWidth;
    public GameObject groundLarge;
    float largeWidth;

    private void Start()
    {
        smallWidth = groundSmall.GetComponent<BoxCollider2D>().size.x * groundSmall.transform.lossyScale.x;
        mediumWidth = groundMedium.GetComponent<BoxCollider2D>().size.x * groundMedium.transform.lossyScale.x;
        largeWidth = groundLarge.GetComponent<BoxCollider2D>().size.x * groundLarge.transform.lossyScale.x;
        baseHeight = -(groundLarge.GetComponent<BoxCollider2D>().size.y / 2) * groundLarge.transform.lossyScale.x - groundLarge.GetComponent<BoxCollider2D>().offset.y * groundLarge.transform.lossyScale.x - playerSize.y - 0.05f;

        objectPooler = ObjectPooler.Instance;
        objectPooler.SpawnFromPool("groundLarge", new Vector3((largeWidth/2) - 5, baseHeight, 0.0f), Quaternion.identity);
        lastEdge = largeWidth - 5;
        lastHeight = baseHeight;

        for (int i = 0; i < 8; i++)
        {
            newPlatform();
        }
    }

    // Use this for initialization
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        playerSize = GetComponent<BoxCollider2D>().size;
        boxSize = new Vector2(playerSize.x - 0.2f, groundedSkin);
    }

    // Update is called once per frame
    void Update()
    {
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
        Debug.Log("" + rb.velocity);
        if (jumpRequest)
        {
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
        else if (rb.velocity.y > 0 && !jumpingHeld)
        {
            rb.gravityScale = lowJumpMulti;
        }
        else
        {
            rb.gravityScale = 1f;
        }

        // adding new platforms
        if (lastEdge <= 64 + rb.position.x)
        {
            newPlatform();
        }
    }

    private void newPlatform()
    {
        float angle = Random.Range(-0.6f, 0.6f);
        float distance = Random.Range(10.0f, 18.0f);

        float groundNumber = Random.value;
        string groundType;
        float currentWidth;
        if (groundNumber < 0.35)
        {
            groundType = "groundSmall";
            currentWidth = smallWidth;
        }
        else if (groundNumber < 0.7)
        {
            groundType = "groundMedium";
            currentWidth = mediumWidth;
        }
        else
        {
            groundType = "groundLarge";
            currentWidth = largeWidth;
        }

        Quaternion flip;
        if (Random.value > 0.5)
        {
            flip = new Quaternion(0, 1, 0, 0);
        }
        else
        {
            flip = new Quaternion(0, 0, 0, 1);
        }

        objectPooler.SpawnFromPool(groundType, new Vector3(lastEdge + (distance * Mathf.Cos(angle)) + (currentWidth / 2), Mathf.Clamp(lastHeight + (distance * Mathf.Sin(angle)), baseHeight - 20.0f, baseHeight + 20.0f)), flip);
        lastEdge += (distance * Mathf.Cos(angle)) + currentWidth;
        lastHeight += distance * Mathf.Sin(angle);
    }
}