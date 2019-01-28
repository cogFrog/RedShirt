using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformSpawner : MonoBehaviour {

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

    public GameObject player;
    Rigidbody2D rb;
    Vector2 playerSize;

    // Use this for initialization
    void Start () {
        objectPooler = ObjectPooler.Instance;

        rb = player.GetComponent<Rigidbody2D>();
        playerSize = player.GetComponent<BoxCollider2D>().size;

        smallWidth = groundSmall.GetComponent<BoxCollider2D>().size.x * groundSmall.transform.lossyScale.x;
        mediumWidth = groundMedium.GetComponent<BoxCollider2D>().size.x * groundMedium.transform.lossyScale.x;
        largeWidth = groundLarge.GetComponent<BoxCollider2D>().size.x * groundLarge.transform.lossyScale.x;
        baseHeight = -(groundLarge.GetComponent<BoxCollider2D>().size.y / 2) * groundLarge.transform.lossyScale.x - groundLarge.GetComponent<BoxCollider2D>().offset.y * groundLarge.transform.lossyScale.x - playerSize.y - 0.05f;

        objectPooler.SpawnFromPool("groundLarge", new Vector3((largeWidth / 2) - 5, baseHeight, 0.0f), Quaternion.identity);
        lastEdge = largeWidth - 5;
        lastHeight = baseHeight;

        for (int i = 0; i < 8; i++)
        {
            newPlatform();
        }
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        if (lastEdge <= 64 + rb.position.x)
        {
            newPlatform();
        }
    }

    private void newPlatform()
    {
        float angle = 0;
        if (lastHeight >= baseHeight + 18)
        {
            angle = Random.Range(-0.6f, 0f);
        }
        else if (lastHeight >= baseHeight - 18)
        {
            angle = Random.Range(0f, 0.6f);
        }
        else
        {
            angle = Random.Range(-0.6f, 0.6f);
        }
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
