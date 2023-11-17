using Godot;

public class BuildRouteState
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


	public BuildRouteState(GameState gameState, TileMap map, Path2D path, Label badge)
	{
		_route = new RouteSegment(Direction.Left, Direction.Right, new Vector2I(1,5));
		_route.NextSegment = new RouteSegment(Direction.Left, Direction.Right, new Vector2I(2,5));
		_route.NextSegment.PreviousSegment = _route;
		_gameState = gameState;
		_map = map;
		_path = path;
		_badge = badge;
		CreateRoute();
	}


    public void Update()
    {
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

	private RouteSegment GetLastSegment()
	{
		var currSegment = _route;
		while (currSegment.NextSegment != null)
		{
			currSegment = currSegment.NextSegment;
		}

		return currSegment;
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
}