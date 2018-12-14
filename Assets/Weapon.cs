using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets
{
    public class Weapon
    {
        public float RateOfFire;
        public float Speed;
        public float Spread;

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
    }

    public enum ProjectileShapes
    {
        Circle
    }

    public class ProjectileAppearance
    {
        public ProjectileShapes Shape;
        public string Color;
        public float Radius;
    }

    public class ProjectilePhysics
    {
        public float Mass;
    }
}
