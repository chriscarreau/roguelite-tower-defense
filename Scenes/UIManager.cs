using Godot;
using System;

public partial class UIManager : CanvasLayer
{
	GameEvents GameEvents;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GameEvents = GetNode<GameEvents>("/root/GameEvents");
	}

	public void OnStartBtnPressed()
	{
		GameEvents.EmitSignal(GameEvents.SignalName.StartWave);
	}
}
