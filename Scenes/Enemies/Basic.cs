using Godot;
using System;

public partial class Basic : BaseEnemy
{
	public override void _Ready()
	{
		base._Ready();
		Speed = 100;
		Health = 100;
	}
}
