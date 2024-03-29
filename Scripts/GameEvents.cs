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

    [Signal]
    public delegate void GameOverEventHandler();

    [Signal]
    public delegate void EnemyDefeatedEventHandler(long reward);

    [Signal]
    public delegate void EnemyReachEndEventHandler(long healthRemoved);

    [Signal]
    public delegate void ResetGameStateEventHandler();
}