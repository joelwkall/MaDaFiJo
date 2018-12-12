using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	public float speedMultiplier;             //Floating point variable to store the player's movement speed.
    public float maxSpeed;
    public float jumpStrength;
    public float rateOfFire;

    private Rigidbody2D rb2d;   
    private BoxCollider2D collider;

    private float timeSinceLastFire;

	// Use this for initialization
	void Start () {
		//Get and store a reference to the Rigidbody2D component so that we can access it.
        rb2d = GetComponent<Rigidbody2D>();
        collider = GetComponent<BoxCollider2D>();
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

        if (Input.GetButton("Jump"))
        {
            var jumpMovement = new Vector2(0, jumpStrength);
            rb2d.AddForce(jumpMovement);
        }

        timeSinceLastFire += Time.deltaTime;

        if(Input.GetMouseButton(0) && timeSinceLastFire > rateOfFire)
        {
            var go = new GameObject("Bullet");
            go.AddComponent<SpriteRenderer>();
            go.AddComponent<Rigidbody2D>();
            go.AddComponent<Bullet>();
            
            go.transform.position = rb2d.position;

            var bulletCollider = go.AddComponent<CircleCollider2D>();
            Physics2D.IgnoreCollision(collider, bulletCollider);

            timeSinceLastFire = 0;
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
