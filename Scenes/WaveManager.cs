using Godot;
using System.Collections.Generic;

public partial class WaveManager : Node
{
	[Export]
	Path2D Path;

	GameEvents GameEvents;

	public override void _Ready()
	{
		GameEvents = GetNode<GameEvents>("/root/GameEvents");
		GameEvents.StartWave += () => SpawnNextWave();
	}

	public async void SpawnNextWave()
	{
		List<WaveUnit> wave = GenerateWave();
		foreach(var waveUnit in wave)
		{
			SpawnUnit(waveUnit);
			await ToSignal(GetTree().CreateTimer(waveUnit.SpawnDelay), SceneTreeTimer.SignalName.Timeout);
		}
	}

	private List<WaveUnit> GenerateWave()
	{
		var wave = new List<WaveUnit>();
		wave.Add(new WaveUnit(EnemyType.Basic, 0.5f));
		wave.Add(new WaveUnit(EnemyType.Basic, 0.5f));
		wave.Add(new WaveUnit(EnemyType.Basic, 0.5f));
		return wave;
	}
	
	private void SpawnUnit(WaveUnit unit)
	{
		var scenePath = GetEnemyScenePath(unit.Enemy);
		var enemyScene = ResourceLoader.Load<PackedScene>(scenePath);
		Path.AddChild(enemyScene.Instantiate());
	}

	private string GetEnemyScenePath(EnemyType enemyType)
	{
		string scenePath = "";
		switch (enemyType)
		{
			case EnemyType.Basic:
				scenePath = "Basic";
				break;
			case EnemyType.Armored:
				scenePath = "Armored";
				break;
			case EnemyType.Fast:
				scenePath = "Fast";
				break;
		}
		return $"res://Scenes/Enemies/{scenePath}.tscn";
	}
}
