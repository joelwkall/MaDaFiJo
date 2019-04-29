using System.Collections.Generic;

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
        //TODO: bounce, friction etc
    }
}
