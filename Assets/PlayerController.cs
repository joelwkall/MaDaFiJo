using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	public float speedMultiplier;             //Floating point variable to store the player's movement speed.
    public float maxSpeed;
    public float jumpStrength;
    public float rateOfFire;

    private Rigidbody2D rb2d;   
    private BoxCollider2D boxCollider;

    private float timeSinceLastFire;

	// Use this for initialization
	void Start () {
		//Get and store a reference to the Rigidbody2D component so that we can access it.
        rb2d = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
	}
	
	//FixedUpdate is called at a fixed interval and is independent of frame rate. Put physics code here.
    void FixedUpdate()
    {
        //Store the current horizontal input in the float moveHorizontal.
        var moveHorizontal = Input.GetAxis("Horizontal");

        //Use the two store floats to create a new Vector2 variable movement.
        var movement = new Vector2(moveHorizontal, 0);

        //Call the AddForce function of our Rigidbody2D rb2d supplying movement multiplied by speed to move our player.
        if (Mathf.Abs(rb2d.velocity.x + movement.x) < maxSpeed)
            rb2d.AddForce (movement * speedMultiplier);

        //jump
        if (Input.GetButton("Jump"))
        {
            var bottomOfPlayer = transform.position + new Vector3(0, -((boxCollider.size.y/2)+0.05f));

            RaycastHit2D hit = Physics2D.Raycast(bottomOfPlayer, Vector3.down, 0.05f);

            if (hit)
            {
                var jumpMovement = new Vector2(0, jumpStrength);
                rb2d.AddForce(jumpMovement);
            }
        }

        timeSinceLastFire += Time.deltaTime;

        //fire bullets
        if(Input.GetMouseButton(0) && timeSinceLastFire > rateOfFire)
        {
            var go = new GameObject("Bullet");
            go.AddComponent<SpriteRenderer>();
            go.AddComponent<Rigidbody2D>();
            go.AddComponent<Bullet>();
            
            go.transform.position = rb2d.position;

            var bulletCollider = go.AddComponent<CircleCollider2D>();
            //ignore collision with player. We could fix this by having bullets spawn
            //just outside the player instead
            Physics2D.IgnoreCollision(boxCollider, bulletCollider);

            timeSinceLastFire = 0;
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
