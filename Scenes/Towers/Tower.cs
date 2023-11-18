using Godot;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public partial class Tower : Node2D
{
	[Export]
	public TowerResource TowerData;

	private Sprite2D _towerSprite;

	private Area2D _rangeArea;

	private List<BaseEnemy> _enemiesInRange = new List<BaseEnemy>();

	private bool _isFiring;
	private PackedScene _bulletScene = ResourceLoader.Load<PackedScene>("res://Scenes/Towers/Bullet/Bullet.tscn");

	public override void _Ready()
	{
		_towerSprite = GetNode<Sprite2D>("Tower");
		_rangeArea = GetNode<Area2D>("RangeArea");
		var shape = (CircleShape2D)(GetNode<CollisionShape2D>("RangeArea/CollisionShape2D").Shape);
		shape.Radius = TowerData.Range;
	}

	public override async void _PhysicsProcess(double delta)
	{
		var closestEnemy = _enemiesInRange.FirstOrDefault();
		if (closestEnemy != null) 
		{
			_towerSprite.LookAt(closestEnemy.GlobalPosition);
			if (!_isFiring)
			{
				await Fire(closestEnemy);
			}
		}
	}

	public void OnRangeEntered(Node2D body)
	{
		if (body is RigidBody2D rigidBody2D)
		{
			var enemy = rigidBody2D.GetParent<BaseEnemy>();
			_enemiesInRange.Add(enemy);
		}
	}

	public void OnRangeExited(Node2D body)
	{
		if (body is RigidBody2D rigidBody2D)
		{
			var enemy = rigidBody2D.GetParent<BaseEnemy>();
			_enemiesInRange.Remove(enemy);
		}
	}

	public async Task Fire(BaseEnemy target)
	{
		_isFiring = true;
		var bullet = _bulletScene.Instantiate<Bullet>();
		bullet.Target = target;
		bullet.Speed = TowerData.ProjectileSpeed;
		bullet.Damage = TowerData.BaseDamage;
		AddChild(bullet);
		await ToSignal(GetTree().CreateTimer(TowerData.RateOfFire), SceneTreeTimer.SignalName.Timeout);
		_isFiring = false;
	}
}
