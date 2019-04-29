using System.Collections.Generic;
using UnityEngine;
using Extensions;

namespace Assets
{
    public class Weapon
    {
        public string Name;
        public float FireDelay;

        public ProjectileEmitter ProjectileEmitter;

        public Dictionary<string, Projectile> Projectiles;
    }

    public class ProjectileEmitter
    {
        public float Force;

        //TODO: maybe all floats should be a pair of number+variance?

        public float Spread; //0 means no spread, 1 means 180 degree spread in both directions
        public float ForceVariance; //0 means no variance, 1 means between 10% - 1000% force

        public string ProjectileName;

        public Weapon Weapon;

        public Projectile EmittedProjectile
        {
            get { return Weapon.Projectiles[ProjectileName]; }
        }

        public void EmitProjectile(BoxCollider2D parentCollider, Rigidbody2D parentBody2D, Vector2 direction)
        {
            var go = new GameObject("Projectile");

            var projectileController = go.AddComponent<ProjectileController>();
            projectileController.Projectile = EmittedProjectile;
            go.transform.position = parentBody2D.position;

            if (EmittedProjectile.Physics.Solid)
            {
                //TODO: boxcollider and other shapes
                var bulletCollider = go.AddComponent<CircleCollider2D>();
                //ignore collision with parent. We could fix this by having bullets spawn
                //just outside the player instead
                Physics2D.IgnoreCollision(parentCollider, bulletCollider);
            }

            var projRb2d = go.AddComponent<Rigidbody2D>();
            projRb2d.sharedMaterial = new PhysicsMaterial2D()
            {
                bounciness = EmittedProjectile.Physics.Bounciness,
                friction = EmittedProjectile.Physics.Friction
            };
            projRb2d.gravityScale = EmittedProjectile.Physics.Gravity;

            var rotationSpread = Spread * Random.Range(-180f, 180f);
            var vector = direction.normalized.Rotate(rotationSpread);
            var forceSpread = (ForceVariance * Random.Range(-0.9f, 10f)) * Force;

            projRb2d.AddForce(vector * (Force + forceSpread));

            var rotation = Vector2.Angle(vector, Vector2.right);

            //TODO: this gives the wrong angle when firing downward
            projRb2d.MoveRotation(rotation);

            //recoil
            parentBody2D.AddForce(-vector * Force);

            //add renderer
            go.AddComponent<SpriteRenderer>();
        }
    }

    public class Projectile
    {
        public ProjectileAppearance Appearance;
        public ProjectilePhysics Physics;

        public List<ProjectileEvent> Events;

        //TODO: triggers, such as time, interval, collision (different types?)
        //actions can be destroy, emit, modify physics or appearance. adding force is done by emitting backwards?
    }

    public class ProjectileEvent
    {
        public ProjectileTrigger Trigger;
        //TODO: multiple actions
        public ProjectileAction Action;
    }

    public class ProjectileTrigger
    {
        public TriggerTypes Type;

        //interval
        public float Interval;
        public float Start;
        public float End;

        //TODO: collision

        public enum TriggerTypes
        {
            Interval,
            Collision
        }
    }

    public class ProjectileAction
    {
        public ActionTypes Type;

        //emit
        public ProjectileEmitter ProjectileEmitter;

        //TODO: other types

        public enum ActionTypes
        {
            Destroy,
            Emit,
            ModifyPhysics,
            ModifyAppearance
        }
    }

    

    public class ProjectileAppearance
    {
        public Shapes Shape;
        public string Color;
        //TODO: use width and height for cirles too, since they can be ellipses
        public float Radius; //used for circle
        public float Width; //used for rectangle
        public float Height; //used for rectangle

        //TODO: add opacity, gradients, glow & other effects

        public enum Shapes
        {
            Circle,
            Rectangle,
            //TODO: add more shapes
        }
    }

    public class ProjectilePhysics
    {
        public float Mass = 1;

        public float Bounciness = 1;
        public float Friction = 1;

        public float Gravity = 1;
        public bool Solid = true;

        //TODO: drag

        //gravity, collidable?
    }
}
