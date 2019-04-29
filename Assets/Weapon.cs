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

        public float Spread; //0 means no spread, 1 means 180 degree spread in both directions
        public float ForceVariance; //0 means no variance, 1 means between 10% - 1000% force

        public string ProjectileName;

        public Weapon Weapon;

        public Projectile EmittedProjectile
        {
            get { return Weapon.Projectiles[ProjectileName]; }
        }

        public void EmitProjectile(BoxCollider2D parentCollider, Rigidbody2D parentBody2D)
        {
            var go = new GameObject("Projectile");

            var projectileController = go.AddComponent<ProjectileController>();
            projectileController.Projectile = EmittedProjectile;
            go.transform.position = parentBody2D.position;

            var bulletCollider = go.AddComponent<CircleCollider2D>();
            //ignore collision with parent. We could fix this by having bullets spawn
            //just outside the player instead
            Physics2D.IgnoreCollision(parentCollider, bulletCollider);

            var projRb2d = go.AddComponent<Rigidbody2D>();
            projRb2d.sharedMaterial = new PhysicsMaterial2D()
            {
                bounciness = EmittedProjectile.Physics.Bounciness,
                friction = EmittedProjectile.Physics.Friction
            };

            var mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var diff = new Vector2(mouse.x, mouse.y) - parentBody2D.position;
            var rotationSpread = Spread * Random.Range(-180f, 180f);
            var vector = diff.normalized.Rotate(rotationSpread);
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

        //TODO: triggers, such as time, interval, collision
        //actions can be destroy, spawn, or modify physics (such as changing speed or direction)
    }

    public enum ProjectileShapes
    {
        Circle,
        Rectangle
    }

    public class ProjectileAppearance
    {
        public ProjectileShapes Shape;
        public string Color;
        //TODO: use width and height for cirles too, since they can be ellipses
        public float Radius; //used for circle
        public float Width; //used for rectangle
        public float Height; //used for rectangle

        //TODO: add opacity
    }

    public class ProjectilePhysics
    {
        public float Mass;

        public float Bounciness;
        public float Friction;
        //TODO: drag
    }
}
