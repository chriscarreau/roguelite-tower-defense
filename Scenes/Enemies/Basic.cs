using Godot;
using System;

public partial class Basic : BaseEnemy
{
	public override void _Ready()
	{
		base._Ready();
		this.Speed = 100;
		this.Health = 100;
	}
}
