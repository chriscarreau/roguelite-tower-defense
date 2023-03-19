using Godot;

public partial class RouteSegment : Node
{
    
    public Direction Origin { get; set; }
    public Direction Destination { get; set; }
    public Vector2I Coords { get; set; }
    public RouteSegment PreviousSegment { get; set; }
    public RouteSegment NextSegment { get; set; }

    public RouteSegment(Direction origin, Direction destination, Vector2I coords)
    {
        Origin = origin;
        Destination = destination;
        Coords = coords;
    }
}
