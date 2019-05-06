using System;
using System.Collections.Generic;
using UnityEngine;
using Extensions;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Random = UnityEngine.Random;
using System.ComponentModel;

namespace Assets
{
    public class Weapon
    {
        public string Name;
        public float FireDelay;

        public ProjectileEmitter ProjectileEmitter;

        public Dictionary<string, Projectile> Projectiles;
    }

    public abstract class Number<T> where T : struct
    {
        public abstract T GetNumber();

        public static implicit operator Number<T>(T exact)
        {
            return new ExactNumber<T>(exact);
        }

        public static implicit operator Number<T>(T[] range)
        {
            return new RangeNumber<T>(range[0], range[1]);
        }
    }

    public class ExactNumber<T> : Number<T> where T: struct
    {
        private T _number;

        public ExactNumber(T number)
        {
            _number = number;
        }

        public override T GetNumber()
        {
            return _number;
        }
    }

    public class RangeNumber<T> : Number<T> where T : struct
    {
        private T _min;
        private T _max;

        public RangeNumber(T min, T max)
        {
            _min = min;
            _max = max;
        }

        public override T GetNumber()
        {
            if (typeof(T) == typeof(float))
                return (T)(object)Random.Range((float)(object)_min, (float)(object)_max);

            if (typeof(T) == typeof(int))
                return (T)(object)Random.Range((int)(object)_min, (int)(object)_max);

            throw new Exception("Number must be of type int or float.");
        }
    }

    public class NumberConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Number<float>) ||
                   objectType == typeof(Number<int>);
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if(reader.TokenType == JsonToken.StartArray)
            {
                var arr = JArray.Load(reader);

                if (objectType == typeof(Number<float>))
                {
                    return new RangeNumber<float>(arr[0].ToObject<float>(), arr[1].ToObject<float>());
                }

                if (objectType == typeof(Number<int>))
                {
                    return new RangeNumber<int>(arr[0].ToObject<int>(), arr[1].ToObject<int>());
                }

                throw new NotSupportedException("Value was an array but cannot be converted to desired type.");
            }
            else
            {
                var obj = reader.Value;
                var valueType = obj.GetType();

                if (objectType == typeof(Number<float>))
                {
                    return new ExactNumber<float>(Convert.ToSingle(obj));
                }

                if (objectType == typeof(Number<int>))
                {
                    return new ExactNumber<int>(Convert.ToInt32(obj));
                }

                throw new NotSupportedException("Value " + obj + " of type " + valueType + " cannot be converted to Number<T>");
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }

    public class ProjectileEmitter
    {
        [DefaultValue(1)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public Number<int> Amount;

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
        public Number<float> Radius; //used for circle
        public Number<float> Width; //used for rectangle
        public Number<float> Height; //used for rectangle

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
    }
}
