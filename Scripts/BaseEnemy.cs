using Godot;

public partial class BaseEnemy : PathFollow2D
{
	[Export]
	public int Speed = 100;

	[Export]
	public int Health = 100;

	[Export]
	public long Reward = 10;

	private GameEvents _gameEvents;

	public override void _Ready()
	{
		_gameEvents = GetNode<GameEvents>("/root/GameEvents");
	}

	public override void _PhysicsProcess(double delta)
	{
		Move(delta);
	}

	public void OnHit(int dmg)
	{
		Health -= dmg;
		if (Health <= 0)
		{
			_gameEvents.EmitSignal(GameEvents.SignalName.EnemyDefeated, Reward);
			OnDestroy();
		}
	}

	public void OnDestroy()
	{
		QueueFree();
	}

	private void Move(double delta)
	{
		Progress += (float)(Speed * delta);
		if (ProgressRatio >= 0.99)
		{
			OnReachEnd();
		}
	}

	private void OnReachEnd()
	{
		_gameEvents.EmitSignal(GameEvents.SignalName.EnemyReachEnd, 1L);
		OnDestroy();
	}
}
