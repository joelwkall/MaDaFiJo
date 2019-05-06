using Assets;
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
            gameObject.transform.localScale = new Vector3(Projectile.Appearance.Radius.GetNumber(), Projectile.Appearance.Radius.GetNumber());
        }
        else if (Projectile.Appearance.Shape == ProjectileAppearance.Shapes.Rectangle)
        {
            renderer.sprite = Resources.Load("Sprites/Square", typeof(Sprite)) as Sprite;
            gameObject.transform.localScale = new Vector3(Projectile.Appearance.Width.GetNumber(), Projectile.Appearance.Height.GetNumber());
        }

        Color color;
        if (!ColorUtility.TryParseHtmlString(Projectile.Appearance.Color, out color))
            color = Color.red;

        renderer.color = color;

        renderer.sortingOrder = 10;
        
        rb2d = GetComponent<Rigidbody2D>();
        rb2d.mass = Projectile.Physics.Mass;
    }

    private Dictionary<int,float> _intervalTriggered = new Dictionary<int, float>();
    private Dictionary<int, float> _collisionTriggered = new Dictionary<int, float>();

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
                var hasStarted = e.Trigger.Start == null || e.Trigger.Start.GetNumber() < lifeTime;
                var hasEnded = e.Trigger.End != null && e.Trigger.End.GetNumber() < lifeTime;

                if (hasStarted && !hasEnded)
                {
                    //no interval, just trigger once and never again
                    if (e.Trigger.Interval == null)
                    {
                        triggered = true;
                        _intervalTriggered[i] = float.MaxValue;
                    }
                    else if (!_intervalTriggered.ContainsKey(i) || _intervalTriggered[i] + e.Trigger.Interval.GetNumber() < lifeTime)
                    {
                        triggered = true;
                        _intervalTriggered[i] = lifeTime;
                    }
                }
            }

            if (!triggered)
                continue;

            TriggerAction(e);
        }
    }

    private void TriggerAction(ProjectileEvent e)
    {
        foreach (var action in e.Actions)
        {
            if (action.Type == ProjectileAction.ActionTypes.Emit)
            {
                var rotationRadians = rb2d.rotation * (Mathf.PI / 180f);
                var direction = new Vector2(Mathf.Cos(rotationRadians), Mathf.Sin(rotationRadians));
                action.ProjectileEmitter.EmitProjectiles(GetComponent<BoxCollider2D>(), rb2d, direction);
            }
            else if (action.Type == ProjectileAction.ActionTypes.Destroy)
            {
                Destroy(gameObject);
            }
        }

        //TODO: support other action types
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (Projectile.Events == null)
            return;

        for (var i = 0; i < Projectile.Events.Count; i++)
        {
            var e = Projectile.Events[i];
            if (e.Trigger.Type == ProjectileTrigger.TriggerTypes.Collision)
            {
                //make sure we dont trigger collisions too often
                if (!_collisionTriggered.ContainsKey(i) || _collisionTriggered[i] + 0.1 < lifeTime)
                {
                    TriggerAction(e);
                    _collisionTriggered[i] = lifeTime;
                }
            }
        }
    }

    void FixedUpdate()
    {
        lifeTime += Time.deltaTime;
        
        if (lifeTime > 20)
            Destroy(gameObject);
    }
}