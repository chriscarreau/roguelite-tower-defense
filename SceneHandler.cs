using Godot;
using Godot.Collections;
using System;

public partial class SceneHandler : Node
{
	PackedScene GameScene = (PackedScene)ResourceLoader.Load("res://Scenes/GameScene.tscn");
	PackedScene MenuScene = (PackedScene)ResourceLoader.Load("res://Scenes/MenuScene.tscn");
	Dictionary<ScenesEnum, PackedScene> Scenes = new Dictionary<ScenesEnum, PackedScene>();
	GameEvents GameEvents;

	public override void _Ready()
	{
		GameEvents = GetNode<GameEvents>("/root/GameEvents");
		Scenes[ScenesEnum.Game] = GameScene;
		Scenes[ScenesEnum.MainMenu] = MenuScene;
		GameEvents.NewGame += OnNewGamePressed;
		GameEvents.GameOver += () => TransitionToScene(ScenesEnum.MainMenu);
		GameEvents.ExitGame += Exit;
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
	public void TransitionToScene(ScenesEnum scene)
	{
		var currentScenes = GetChildren();
		foreach (Node sc in currentScenes)
		{
			RemoveChild(sc);
			sc.QueueFree();
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
