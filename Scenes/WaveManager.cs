using Godot;
using System.Collections.Generic;

public partial class WaveManager : Node
{
	[Export]
	Path2D Path;

	GameEvents GameEvents;
	GameStateManager GameStateManager;
	private UIManager _uiManager;
	private bool _waveActive;

	public override void _Ready()
	{
		GameEvents = GetNode<GameEvents>("/root/GameEvents");
		GameStateManager = GetNode<GameStateManager>("%GameStateManager");
		_uiManager = GetNode<UIManager>("%UI");
		GameEvents.StartWave += () => SpawnNextWave();
	}

    public override void _PhysicsProcess(double delta)
    {
        if (_waveActive && Path.GetChildCount() == 0) {
			_waveActive = false;
			_uiManager.SetStartButtonEnabled(true);
		}
    }

    public async void SpawnNextWave()
	{
		var currentWave = GameStateManager.IncreaseWave();
		List<WaveUnit> wave = GenerateWave(currentWave);
		_uiManager.UpdateWaveLabel(currentWave);
		_waveActive = true;
		_uiManager.SetStartButtonEnabled(false);
		foreach(var waveUnit in wave)
		{
			SpawnUnit(waveUnit);
			await ToSignal(GetTree().CreateTimer(waveUnit.SpawnDelay), SceneTreeTimer.SignalName.Timeout);
		}
	}

	private List<WaveUnit> GenerateWave(long currentWave)
	{
		var wave = new List<WaveUnit>();
		for (int i = 0; i < currentWave; i++)
		{
			wave.Add(new WaveUnit(EnemyType.Basic, 0.5f));
		}
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
