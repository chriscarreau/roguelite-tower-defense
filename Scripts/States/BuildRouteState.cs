using Godot;

public class BuildRouteState : State
{
	private bool _canBuild;

    public BuildRouteState(GameScene gameScene) : base(gameScene)
    {
    }


    public override void Process(double delta)
    {
        UpdateRoutePreview();
    }

    public override void PhysicsProcess(double delta)
    {
        if (Input.IsActionPressed("build"))
		{
			AddRouteSegment();
			_gameScene.ComputeRoute();
			if (_gameScene.GameState.RouteLength - _gameScene.CurrentRouteLength == 0)
			{
				CancelBuildRoute();
				_gameScene.ChangeState(new DefaultState(_gameScene));
			}
		}
		if (Input.IsActionPressed("cancel"))
		{
			CancelBuildRoute();
			_gameScene.ChangeState(new DefaultState(_gameScene));
		}
    }

    public override void HandleButton(ButtonEnum button)
    {
        switch (button)
        {
            case ButtonEnum.BuildRouteButton:
                CancelBuildRoute();
				_gameScene.ChangeState(new DefaultState(_gameScene));
                return;
            case ButtonEnum.BuildTowerButton:
                CancelBuildRoute();
				_gameScene.ChangeState(new BuildTowerState(_gameScene, new BaseTower()));
                return;
            default:
                GD.PrintErr("Unhandled button of type " + button.ToString());
                return;
        }
    }

	private void CancelBuildRoute() {
		_gameScene.RemoveChild(_gameScene.Preview);
		_gameScene.Preview.QueueFree();
		_gameScene.Preview = null;
	}

	private void AddRouteSegment()
	{
		RouteSegment lastSegment = GetLastSegment();
		if (!_canBuild) {
			return;
		}
		Direction origin = Direction.Top;
		Direction destination = Direction.Bottom;
		if (_gameScene.CurrentBuildLocation.X > lastSegment.Coords.X){
			origin = Direction.Left;
			destination = Direction.Right;
		} else if (_gameScene.CurrentBuildLocation.X < lastSegment.Coords.X) {
			origin = Direction.Right;
			destination = Direction.Left;
		} else if (_gameScene.CurrentBuildLocation.Y > lastSegment.Coords.Y) {
			origin = Direction.Top;
			destination = Direction.Bottom;
		} else if (_gameScene.CurrentBuildLocation.Y < lastSegment.Coords.Y) {
			origin = Direction.Bottom;
			destination = Direction.Top;
		}
		var newSegment = new RouteSegment(origin, destination, _gameScene.CurrentBuildLocation);
		lastSegment.NextSegment = newSegment;
		lastSegment.Destination = destination;
	}

	private RouteSegment GetLastSegment()
	{
		var currSegment = _gameScene.Route;
		while (currSegment.NextSegment != null)
		{
			currSegment = currSegment.NextSegment;
		}

		return currSegment;
	}

	private bool CheckCanBuild(Vector2I lastSegmentPosition, Vector2I buildPosition)
	{
		var deltaX = Mathf.Abs(lastSegmentPosition.X - buildPosition.X);
		var deltaY = Mathf.Abs(lastSegmentPosition.Y - buildPosition.Y);
		if (deltaX + deltaY == 1)
		{
			return _gameScene.Map.GetCellTileData(1, buildPosition) == null;
		}
		return false;
	}

	private void UpdateRoutePreview() {
		var mousePosition = _gameScene.GetGlobalMousePosition();
		var currentTile = _gameScene.Map.LocalToMap(_gameScene.ToLocal(mousePosition));
		var tilePosition = _gameScene.ToGlobal(_gameScene.Map.MapToLocal(currentTile));
		tilePosition.X -= 32;
		tilePosition.Y -= 32;
		_gameScene.CurrentBuildLocation = currentTile;
		if (_gameScene.Preview == null){
			_gameScene.Preview = new Control();
            var overlay = new TextureRect
            {
                Texture = GD.Load<Texture2D>("res://Assets/UI/UI/Overlay.png"),
                Modulate = new Color(1, 0, 0),
                ZIndex = 5,
                Name = "Overlay"
            };
			var routePreview = new TextureRect
            {
                Texture = GD.Load<Texture2D>("res://Assets/UI/RoutePreview.png"),
                Modulate = new Color(1, 1, 1, 0.3f),
                ZIndex = 5,
                Name = "Prev"
            };
			overlay.AddChild(routePreview);
			_gameScene.Preview.AddChild(overlay);
			_gameScene.AddChild(_gameScene.Preview);
		}
		RouteSegment lastSegment = GetLastSegment();
		_gameScene.Preview.Position = tilePosition;
		if (CheckCanBuild(lastSegment.Coords, _gameScene.CurrentBuildLocation)){
			_canBuild = true;
			_gameScene.Preview.GetNode<TextureRect>("Overlay").Modulate = new Color(0,1,0);
		} else {
			_canBuild = false;
			_gameScene.Preview.GetNode<TextureRect>("Overlay").Modulate = new Color(1,0,0);
		}
	}
}