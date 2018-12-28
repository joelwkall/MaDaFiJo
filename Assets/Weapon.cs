using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets
{
    //TODO: split this up into Weapon and ProjectileGenerator 
    //(which can be reused for triggers)
    public class Weapon
    {
        public string Name;
        public float FireDelay;
        public float Force;

        //TODO: implement these
        public float Spread;
        public float ForceVariance;

        public string MainProjectileName;

        public Dictionary<string, Projectile> Projectiles;

        public Projectile MainProjectile
        {
            get
            {
                return Projectiles[MainProjectileName];
            }
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
    }

    public class ProjectilePhysics
    {
        public float Mass;
        //TODO: bounce, friction etc
    }
}
