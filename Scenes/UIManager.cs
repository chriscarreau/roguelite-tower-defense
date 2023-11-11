using Godot;
using System;

public partial class UIManager : CanvasLayer
{
	GameState GameState;
	GameEvents GameEvents;

	private Label _healthLabel;
	private Label _waveLabel;
	private Button _startBtn;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GameEvents = GetNode<GameEvents>("/root/GameEvents");
		GameState = GetNode<GameState>("/root/GameState");
		_healthLabel = GetNode<Label>("%HealthLbl");
		_waveLabel = GetNode<Label>("%WaveLbl");
		_startBtn = GetNode<Button>("%StartBtn");
	}

	public void OnStartBtnPressed()
	{
		GameEvents.EmitSignal(GameEvents.SignalName.StartWave);
	}

	public void UpdateHealthLabel(long health)
	{
		_healthLabel.Text = health.ToString();
	}

	public void UpdateWaveLabel(long wave)
	{
		_waveLabel.Text = wave.ToString();
	}

	public void SetStartButtonEnabled(bool enabled)
	{
		_startBtn.Disabled = !enabled;
	}
}
