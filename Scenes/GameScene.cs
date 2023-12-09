using Godot;
using System;
using System.Collections.Generic;

public partial class GameScene : Node2D
{
	/*==== Nodes ====*/
	public TileMap Map;
	public Path2D Path;
	public Vector2I CurrentBuildLocation;
	public Control Preview;
	public Label Badge;

	/*==== State ====*/
	public State CurrentState;
	public RouteSegment Route;
	public List<BaseTower> Towers = new List<BaseTower>();
	public int CurrentRouteLength;
	public GameState GameState;
	public GameStateManager GameStateManager;
	public GameEvents GameEvents;


	public override void _Ready()
	{
		CurrentState = new DefaultState(this);
		Route = new RouteSegment(Direction.Left, Direction.Right, new Vector2I(1,5));
		GameState = GetNode<GameState>("/root/GameState");
		GameEvents = GetNode<GameEvents>("/root/GameEvents");
		GameStateManager = GetNode<GameStateManager>("%GameStateManager");
		Map = GetNode<TileMap>("TileMap");
		Path = GetNode<Path2D>("Path2D");
		Badge = GetNode<Label>("UI/HUD/RouteButton/GridContainer/HBoxContainer/MarginContainer/RouteBadge");
		ComputeRoute();
		GameEvents.EmitSignal(GameEvents.SignalName.ResetGameState);
	}

	public override void _Process(double delta){
		CurrentState.Process(delta);
	}

	public override void _PhysicsProcess(double delta){
		CurrentState.PhysicsProcess(delta);
    }

	public void ChangeState(State nextState)
	{
		CurrentState = nextState;
	}
	
	public void BuildRouteButtonPressed()
	{
		CurrentState.HandleButton(ButtonEnum.BuildRouteButton);
	}
	
	public void BuildTowerButtonPressed()
	{
		CurrentState.HandleButton(ButtonEnum.BuildTowerButton);
	}

	public void ComputeRoute() {
		Path.Curve = new Curve2D();
		var currSegment = Route;
		var length = 0;
		while (currSegment != null)
		{
			Map.SetCell(1, currSegment.Coords, 0, GetSegmentTilesetCoord(currSegment));
			var globalCoord = ToGlobal(Map.MapToLocal(currSegment.Coords));
			Path.Curve.AddPoint(globalCoord);
			currSegment = currSegment.NextSegment;
			length++;
		}
		CurrentRouteLength = length;
		DrawDebugLine();
		UpdateLabel();
	}

	private void DrawDebugLine() {
		Line2D line = GetNodeOrNull<Line2D>("DebugLine");
		if (line == null) {
			line = new Line2D();
			AddChild(line);
		}
		line.ClearPoints();
		line.DefaultColor = new Color(1,1,1,1);
		line.Width = 10;
		foreach (var point in Path.Curve.GetBakedPoints())
		{
			line.AddPoint(point);
		}
		line.ZIndex = 3;
		line.Name = "DebugLine";
	}

	private void UpdateLabel() {
		Badge.Text = (GameState.RouteLength - CurrentRouteLength).ToString();
	}

	private Vector2I GetSegmentTilesetCoord(RouteSegment segment) {
		if ((segment.Origin == Direction.Left && segment.Destination == Direction.Right)
		|| (segment.Origin == Direction.Right && segment.Destination == Direction.Left)) {
				return new Vector2I(2,0);
		}
		else if ((segment.Origin == Direction.Top && segment.Destination == Direction.Bottom)
		|| (segment.Origin == Direction.Bottom && segment.Destination == Direction.Top)) {
			return new Vector2I(1,0);
		}
		else if ((segment.Origin == Direction.Left && segment.Destination == Direction.Top)
		|| (segment.Origin == Direction.Top && segment.Destination == Direction.Left)) {
			return new Vector2I(6,1);
		}
		else if ((segment.Origin == Direction.Left && segment.Destination == Direction.Bottom)
		|| (segment.Origin == Direction.Bottom && segment.Destination == Direction.Left)) {
			return new Vector2I(4,1);
		}
		else if ((segment.Origin == Direction.Right && segment.Destination == Direction.Top)
		|| (segment.Origin == Direction.Top && segment.Destination == Direction.Right)) {
			return new Vector2I(5,1);
		}
		else if ((segment.Origin == Direction.Right && segment.Destination == Direction.Bottom)
		|| (segment.Origin == Direction.Bottom && segment.Destination == Direction.Right)) {
			return new Vector2I(3,1);
		}
		return new Vector2I(0,0);
	}
}
