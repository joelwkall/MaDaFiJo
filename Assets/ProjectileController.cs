using Assets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour {

    public Projectile Projectile;

    private Rigidbody2D rb2d;
    private float lifeTime;

    // Use this for initialization
    void Start () {

        var renderer = GetComponent<SpriteRenderer>();

        if (Projectile.Appearance.Shape == ProjectileShapes.Circle)
        {
            renderer.sprite = Resources.Load("Sprites/Circle", typeof(Sprite)) as Sprite;
            gameObject.transform.localScale = new Vector3(Projectile.Appearance.Radius, Projectile.Appearance.Radius);
        }
        else if (Projectile.Appearance.Shape == ProjectileShapes.Rectangle)
        {
            renderer.sprite = Resources.Load("Sprites/Square", typeof(Sprite)) as Sprite;
            gameObject.transform.localScale = new Vector3(Projectile.Appearance.Width, Projectile.Appearance.Height);
        }

        Color color;
        if (!ColorUtility.TryParseHtmlString(Projectile.Appearance.Color, out color))
            color = Color.red;

        renderer.color = color;
        
        rb2d = GetComponent<Rigidbody2D>();
        rb2d.mass = Projectile.Physics.Mass;
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