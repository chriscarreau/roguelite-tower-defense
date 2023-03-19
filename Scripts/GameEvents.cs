using Godot;

public partial class GameEvents : Node
{
    [Signal]
    public delegate void NewGameEventHandler();

    [Signal]
    public delegate void ExitGameEventHandler();

    [Signal]
    public delegate void StartWaveEventHandler();

    [Signal]
    public delegate void PauseEventHandler();
}