using Godot;

public partial class MenuScene : Control
{
	GameEvents GameEvents;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GameEvents = GetNode<GameEvents>("/root/GameEvents");
	}

	public void OnNewGamePressed() {
		GameEvents.EmitSignal(GameEvents.SignalName.NewGame);
	}

	public void OnExitPressed() {
		GameEvents.EmitSignal(GameEvents.SignalName.ExitGame);
	}
}
