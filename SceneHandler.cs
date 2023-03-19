using Godot;
using Godot.Collections;
using System;

public partial class SceneHandler : Node
{
	PackedScene GameScene = (PackedScene)ResourceLoader.Load("res://Scenes/GameScene.tscn");
	Dictionary<ScenesEnum, PackedScene> Scenes = new Dictionary<ScenesEnum, PackedScene>();
	GameEvents GameEvents;

	public override void _Ready()
	{
		GameEvents = GetNode<GameEvents>("/root/GameEvents");
		Scenes[ScenesEnum.Game] = GameScene;
		GameEvents.NewGame += () => this.OnNewGamePressed();
		GameEvents.ExitGame += () => this.Exit();
	}
	public void OnNewGamePressed()
	{
		TransitionToScene(ScenesEnum.Game);
	}

	public void Exit()
	{
		GetTree().Quit();
	}

	/// <summary>
	/// Replace current scene by provided scene
	/// </summary>
	/// <param name="scene">The scene to transition to</param>
	private void TransitionToScene(ScenesEnum scene)
	{
		var currentScenes = GetChildren();
		foreach (Node sc in currentScenes)
		{
			RemoveChild(sc);
		}
		var instance = Scenes[scene].Instantiate();
		AddChild(instance);
	}
}

public enum ScenesEnum
{
	MainMenu,
	Game
}
