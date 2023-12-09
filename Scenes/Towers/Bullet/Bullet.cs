using Godot;
using System;

public partial class Bullet : Node2D
{
	public float Speed;
	public int Damage;
	public BaseEnemy Target;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	public override void _PhysicsProcess(double delta)
	{
		if (!IsInstanceValid(Target) || Target.IsQueuedForDeletion())
		{
			QueueFree();
			return;
		}
		GlobalPosition = GlobalPosition.MoveToward(Target.GlobalPosition, Speed * (float)delta);
	}

	public void OnBodyEntered(Node2D body) {
		if (body.GetParent() is BaseEnemy enemy && enemy == Target) {
			enemy.OnHit(Damage);
			QueueFree();
		}
	}
}
