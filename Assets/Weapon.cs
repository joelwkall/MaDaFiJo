using System;
using System.Collections.Generic;
using UnityEngine;
using Extensions;
using Random = UnityEngine.Random;

namespace Assets
{
    public class Weapon
    {
        public string Name;
        public float FireDelay;

        public ProjectileEmitter ProjectileEmitter;

        public Dictionary<string, Projectile> Projectiles;
    }

    public class Number<T> where T : struct
    {
        public T? Exact;
        public T[] Range;

        public T GetNumber()
        {
            if (Range != null)
            {
                if (Range.Length != 2)
                    throw new Exception("Range must have exactly 2 values.");

                if (typeof(T) == typeof(float))
                    return (T)(object)Random.Range((float)(object)Range[0], (float)(object)Range[1]);

                if (typeof(T) == typeof(int))
                    return (T)(object)Random.Range((int)(object)Range[0], (int)(object)Range[1]);

                throw new Exception("Number must be of type int or float.");
            }

            if (Exact != null)
                return Exact.Value;

            throw new Exception("Either Exact or Range must be specified.");
        }
    }

    public class ProjectileEmitter
    {
        public Number<int> Amount = new Number<int>{ Exact = 1 };

        public Number<float> Force;

        public float Spread; //0 means no spread, 1 means 180 degree spread in both directions

        public string ProjectileName;

        public Weapon Weapon;

        public Projectile EmittedProjectile
        {
            get { return Weapon.Projectiles[ProjectileName]; }
        }

        public void EmitProjectiles(BoxCollider2D parentCollider, Rigidbody2D parentBody2D, Vector2 direction)
        {
            for (var i = 0; i < Amount.GetNumber(); i++)
            {
                var go = new GameObject("Projectile");

                var projectileController = go.AddComponent<ProjectileController>();
                projectileController.Projectile = EmittedProjectile;
                go.transform.position = parentBody2D.position;

                if (EmittedProjectile.Physics.Solid)
                {
                    //TODO: boxcollider and other shapes
                    var bulletCollider = go.AddComponent<CircleCollider2D>();
                    //ignore collision with parent.
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

                projRb2d.AddForce(vector * (Force.GetNumber()));

                var rotation = Vector2.Angle(vector, Vector2.right);

                //TODO: this gives the wrong angle when firing downward
                projRb2d.MoveRotation(rotation);

                //recoil
                parentBody2D.AddForce(-vector * Force.GetNumber());

                //add renderer
                go.AddComponent<SpriteRenderer>();
            }
        }
    }

    public class Projectile
    {
        public ProjectileAppearance Appearance;
        public ProjectilePhysics Physics;

        public List<ProjectileEvent> Events;
    }

    public class ProjectileEvent
    {
        public ProjectileTrigger Trigger;
        public ProjectileAction[] Actions;
    }

    public class ProjectileTrigger
    {
        public TriggerTypes Type;

        //interval
        public Number<float> Interval;
        public Number<float> Start;
        public Number<float> End;

        //TODO: different collision types?

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
        public bool Solid = true; //TODO: different collision types?

        //TODO: drag

        //gravity?
    }
}
