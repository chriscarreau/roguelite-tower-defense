using Godot;

public class BuildRouteState : State
{
	private bool _canBuild;

    public override void Process(double delta)
    {
        UpdateRoutePreview();
    }

    public override void PhysicsProcess(double delta)
    {
        if (Input.IsActionPressed("build"))
		{
			AddRouteSegment();
			GameScene.ComputeRoute();
			if (GameScene.GameState.RouteLength - GameScene.CurrentRouteLength == 0)
			{
				CancelBuildRoute();
				GameScene.ChangeState(new DefaultState());
			}
		}
		if (Input.IsActionPressed("cancel"))
		{
			CancelBuildRoute();
			GameScene.ChangeState(new DefaultState());
		}
    }

    public override void HandleButton(ButtonEnum button)
    {
        switch (button)
        {
            case ButtonEnum.BuildRouteButton:
                CancelBuildRoute();
				GameScene.ChangeState(new DefaultState());
                return;
            case ButtonEnum.BuildTowerButton:
                CancelBuildRoute();
				GameScene.ChangeState(new BuildTowerState());
                return;
            default:
                GD.PrintErr("Unhandled button of type " + button.ToString());
                return;
        }
    }

	private void CancelBuildRoute() {
		GameScene.RemoveChild(GameScene.Preview);
		GameScene.Preview.QueueFree();
		GameScene.Preview = null;
	}

	private void AddRouteSegment()
	{
		RouteSegment lastSegment = GetLastSegment();
		if (!_canBuild) {
			return;
		}
		Direction origin = Direction.Top;
		Direction destination = Direction.Bottom;
		if (GameScene.CurrentBuildLocation.X > lastSegment.Coords.X){
			origin = Direction.Left;
			destination = Direction.Right;
		} else if (GameScene.CurrentBuildLocation.X < lastSegment.Coords.X) {
			origin = Direction.Right;
			destination = Direction.Left;
		} else if (GameScene.CurrentBuildLocation.Y > lastSegment.Coords.Y) {
			origin = Direction.Top;
			destination = Direction.Bottom;
		} else if (GameScene.CurrentBuildLocation.Y < lastSegment.Coords.Y) {
			origin = Direction.Bottom;
			destination = Direction.Top;
		}
		var newSegment = new RouteSegment(origin, destination, GameScene.CurrentBuildLocation);
		lastSegment.NextSegment = newSegment;
		lastSegment.Destination = destination;
	}

	private RouteSegment GetLastSegment()
	{
		var currSegment = GameScene.Route;
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
			return GameScene.Map.GetCellTileData(1, buildPosition) == null;
		}
		return false;
	}

	private void UpdateRoutePreview() {
		var mousePosition = GameScene.GetGlobalMousePosition();
		var currentTile = GameScene.Map.LocalToMap(GameScene.ToLocal(mousePosition));
		var tilePosition = GameScene.ToGlobal(GameScene.Map.MapToLocal(currentTile));
		tilePosition.X -= 32;
		tilePosition.Y -= 32;
		GameScene.CurrentBuildLocation = currentTile;
		if (GameScene.Preview == null){
			GameScene.Preview = new Control();
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
			GameScene.Preview.AddChild(overlay);
			GameScene.AddChild(GameScene.Preview);
		}
		RouteSegment lastSegment = GetLastSegment();
		GameScene.Preview.Position = tilePosition;
		if (CheckCanBuild(lastSegment.Coords, GameScene.CurrentBuildLocation)){
			_canBuild = true;
			GameScene.Preview.GetNode<TextureRect>("Overlay").Modulate = new Color(0,1,0);
		} else {
			_canBuild = false;
			GameScene.Preview.GetNode<TextureRect>("Overlay").Modulate = new Color(1,0,0);
		}
	}
}