using Godot;
using System;

public partial class TowerResource : Resource
{

    [Export]
	public int Range { get; set; }

	[Export]
	public int BaseDamage { get; set; }

	[Export]
	public float RateOfFire { get; set; }

	[Export]
	public float ProjectileSpeed { get; set; }

    public TowerResource() : this(0, 0, 0, 0) {}

    public TowerResource(int range, int baseDamage, float rateOfFire, float projectileSpeed)
    {
        Range = range;
        BaseDamage = baseDamage;
        RateOfFire = rateOfFire;
        ProjectileSpeed = projectileSpeed;
    }
}
