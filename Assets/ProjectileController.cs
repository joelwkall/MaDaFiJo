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

        if (Projectile.Appearance.Shape == ProjectileAppearance.Shapes.Circle)
        {
            renderer.sprite = Resources.Load("Sprites/Circle", typeof(Sprite)) as Sprite;
            gameObject.transform.localScale = new Vector3(Projectile.Appearance.Radius, Projectile.Appearance.Radius);
        }
        else if (Projectile.Appearance.Shape == ProjectileAppearance.Shapes.Rectangle)
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

    private Dictionary<int,float> _intervalTriggered = new Dictionary<int, float>();
	
	// Update is called once per frame
	void Update ()
    {
        if (Projectile.Events == null)
            return;

        for(var i = 0; i < Projectile.Events.Count; i++)
        {
            var e = Projectile.Events[i];

            bool triggered = false;
            if (e.Trigger.Type == ProjectileTrigger.TriggerTypes.Interval)
            {
                if (e.Trigger.Start < lifeTime && e.Trigger.End > lifeTime)
                {
                    if (!_intervalTriggered.ContainsKey(i) || _intervalTriggered[i] + e.Trigger.Interval < lifeTime)
                    {
                        triggered = true;
                        _intervalTriggered[i] = lifeTime;
                    }
                }
            }
            //TODO: support other trigger types

            if (!triggered)
                continue;

            if (e.Action.Type == ProjectileAction.ActionTypes.Emit)
            {
                var rotationRadians = rb2d.rotation * (Mathf.PI / 180f);
                var direction = new Vector2(Mathf.Cos(rotationRadians), Mathf.Sin(rotationRadians));
                e.Action.ProjectileEmitter.EmitProjectile(GetComponent<BoxCollider2D>(), rb2d, direction);
            }
            else if (e.Action.Type == ProjectileAction.ActionTypes.Destroy)
            {
                Destroy(gameObject);
            }
            //TODO: support other action types
        }
	}

    void FixedUpdate()
    {
        lifeTime += Time.deltaTime;

        //TODO: adjustable lifetime? or controlled by events
        if (lifeTime > 10)
            Destroy(gameObject);
    }
}