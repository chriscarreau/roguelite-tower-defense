using Godot;

public partial class GameStateManager : Node 
{
    private GameState _gameState;
	private GameEvents _gameEvents;
	private UIManager _uiManager;

    public override void _Ready()
    {
		_gameState = GetNode<GameState>("/root/GameState");
		_gameEvents = GetNode<GameEvents>("/root/GameEvents");
		_uiManager = GetNode<UIManager>("%UI");

		_gameEvents.EnemyReachEnd += OnEnemyReachEnd;
        ResetGameState();
    }

    public long IncreaseWave()
    {
        return ++_gameState.CurrentWave;
    }

    private void OnEnemyReachEnd(long healthRemoved)
    {
        _gameState.Health -= healthRemoved;
        if (_gameState.Health <= 0)
        {
            _gameEvents.EmitSignal(GameEvents.SignalName.GameOver);
        }
        _uiManager.UpdateHealthLabel(_gameState.Health);
    }

    private void ResetGameState()
    {
        _gameState.Health = _gameState.MaxHealth;
        _uiManager.UpdateHealthLabel(_gameState.Health);
        _gameState.CurrentWave = 0;
        _uiManager.UpdateWaveLabel(_gameState.CurrentWave);
    }
}