using Godot;
using System;
using System.Collections.Generic;

public partial class GameScene : Node2D
{
	/*==== Nodes ====*/
	private TileMap _map;
	private Path2D _path;
	private Vector2I _currentBuildLocation;
	private Control _routePreview;
	private Label _badge;

	/*==== Internal State ====*/
	private RouteSegment _route;
	private int _currentRouteLength;
	private GameState _gameState;
	private bool _isBuildMode;
	private bool _canBuild;
	private string _currentBuildType;


	public override void _Ready()
	{
		_route = new RouteSegment(Direction.Left, Direction.Right, new Vector2I(1,5));
		_route.NextSegment = new RouteSegment(Direction.Left, Direction.Right, new Vector2I(2,5));
		_route.NextSegment.PreviousSegment = _route;
		_gameState = GetNode<GameState>("/root/GameState");
		_map = GetNode<TileMap>("TileMap");
		_path = GetNode<Path2D>("Path2D");
		_badge = GetNode<Label>("UI/HUD/TextureButton/GridContainer/HBoxContainer/MarginContainer/RouteBadge");
		foreach (var buildBtn in GetTree().GetNodesInGroup("build_buttons"))
		{
			buildBtn.Connect(TextureButton.SignalName.Pressed, Callable.From(() => InitiateBuildMode(buildBtn.Name.ToString())));
		}
		CreateRoute();
	}

	public override void _Process(double delta){
		if (_isBuildMode) {
			UpdateTowerPreview();
		}
	}

	public override void _PhysicsProcess(double delta){
		if (_isBuildMode) {
			if (Input.IsActionPressed("tower_build"))
			{
				BuildRoute();
				CreateRoute();
				if (_gameState.RouteLength - _currentRouteLength == 0)
				{
					CancelBuildMode();
				}
			}
			if (Input.IsActionPressed("tower_cancel"))
			{
				CancelBuildMode();
			}
		}
	}

	private void BuildRoute()
	{
		RouteSegment lastSegment = GetLastSegment();
		if (!_canBuild) {
			return;
		}
		Direction origin = Direction.Top;
		Direction destination = Direction.Bottom;
		if (_currentBuildLocation.X > lastSegment.Coords.X){
			origin = Direction.Left;
			destination = Direction.Right;
		} else if (_currentBuildLocation.X < lastSegment.Coords.X) {
			origin = Direction.Right;
			destination = Direction.Left;
		} else if (_currentBuildLocation.Y > lastSegment.Coords.Y) {
			origin = Direction.Top;
			destination = Direction.Bottom;
		} else if (_currentBuildLocation.Y < lastSegment.Coords.Y) {
			origin = Direction.Bottom;
			destination = Direction.Top;
		}
		var newSegment = new RouteSegment(origin, destination, _currentBuildLocation);
		lastSegment.NextSegment = newSegment;
		lastSegment.Destination = destination;
	}

	private bool CanBuild(Vector2I lastSegmentPosition, Vector2I buildPosition)
	{
		var deltaX = Mathf.Abs(lastSegmentPosition.X - buildPosition.X);
		var deltaY = Mathf.Abs(lastSegmentPosition.Y - buildPosition.Y);
		if (deltaX + deltaY == 1)
		{
			return _map.GetCellTileData(1, buildPosition) == null;
		}
		return false;
	}

	private RouteSegment GetLastSegment()
	{
		var currSegment = _route;
		while (currSegment.NextSegment != null)
		{
			currSegment = currSegment.NextSegment;
		}

		return currSegment;
	}

	private void CreateRoute() {
		_path.Curve = new Curve2D();
		var currSegment = _route;
		var length = 0;
		while (currSegment != null)
		{
			_map.SetCell(1, currSegment.Coords, 0, GetSegmentTilesetCoord(currSegment));
			var globalCoord = ToGlobal(_map.MapToLocal(currSegment.Coords));
			_path.Curve.AddPoint(globalCoord);
			currSegment = currSegment.NextSegment;
			length++;
		}
		_currentRouteLength = length;
		DrawDebugLine();
		UpdateLabel();
	}

	private void InitiateBuildMode(string name)
	{
		if (_gameState.RouteLength - _currentRouteLength <= 0) {
			return;
		}
		_currentBuildType = name;
		_isBuildMode = true;
	}

	private void CancelBuildMode() {
		RemoveChild(_routePreview);
		_routePreview.QueueFree();
		_routePreview = null;
		_isBuildMode = false;
	}

	private void UpdateTowerPreview() {
		var mousePosition = GetGlobalMousePosition();
		var currentTile = _map.LocalToMap(ToLocal(mousePosition));
		var tilePosition = ToGlobal(_map.MapToLocal(currentTile));
		tilePosition.X -= 32;
		tilePosition.Y -= 32;
		_currentBuildLocation = currentTile;
		if (_routePreview == null){
			_routePreview = new Control();
			var overlay = new TextureRect();
			overlay.Texture = GD.Load<Texture2D>("res://Assets/UI/UI/Overlay.png");
			overlay.Modulate = new Color(1,0,0);
			overlay.ZIndex = 5;
			overlay.Name = "Overlay";
			_routePreview.AddChild(overlay);
			AddChild(_routePreview);
		}
		RouteSegment lastSegment = GetLastSegment();
		_routePreview.Position = tilePosition;
		if (CanBuild(lastSegment.Coords, _currentBuildLocation)){
			_canBuild = true;
			_routePreview.GetNode<TextureRect>("Overlay").Modulate = new Color(0,1,0);
		} else {
			_canBuild = false;
			_routePreview.GetNode<TextureRect>("Overlay").Modulate = new Color(1,0,0);
		}
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

	private void DrawDebugLine() {
		Line2D line = GetNodeOrNull<Line2D>("DebugLine");
		if (line == null) {
			line = new Line2D();
			AddChild(line);
		}
		line.ClearPoints();
		line.DefaultColor = new Color(1,1,1,1);
		line.Width = 10;
		foreach (var point in _path.Curve.GetBakedPoints())
		{
			line.AddPoint(point);
		}
		line.ZIndex = 3;
		line.Name = "DebugLine";
	}

	private void UpdateLabel() {
		this._badge.Text = (_gameState.RouteLength - _currentRouteLength).ToString();
	}
}
