using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {


    private Rigidbody2D rb2d;       //Store a reference to the Rigidbody2D component required to use 2D Physics.
    private float lifeTime;

    // Use this for initialization
    void Start () {

        var renderer = GetComponent<SpriteRenderer>();

        renderer.sprite = Resources.Load("Sprites/Circle", typeof(Sprite)) as Sprite;
        renderer.color = Color.cyan;
        
        gameObject.transform.localScale = new Vector3(0.4f, 0.4f);

        rb2d = GetComponent<Rigidbody2D>();
        rb2d.mass = 0.01f;

        var mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var diff = new Vector2(mouse.x, mouse.y) - rb2d.position;

        rb2d.AddForce(diff.normalized * 5f);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void FixedUpdate()
    {
        lifeTime += Time.deltaTime;

        if (lifeTime > 1)
            Destroy(gameObject);
    }
}