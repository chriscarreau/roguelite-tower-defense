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

		_gameEvents.EnemyDefeated += OnEnemyDefeated;
		_gameEvents.EnemyReachEnd += OnEnemyReachEnd;
        _gameEvents.ResetGameState += ResetGameState;
    }

    public long IncreaseWave()
    {
        return ++_gameState.CurrentWave;
    }

    public void ResetGameState()
    {
        _gameState.Health = _gameState.MaxHealth;
        _uiManager.UpdateHealthLabel(_gameState.Health);
        _gameState.CurrentWave = 0;
        _uiManager.UpdateWaveLabel(_gameState.CurrentWave);
        _gameState.Coins = 500;
        _uiManager.UpdateCoinsLabel(_gameState.Coins);
    }

    public void PurchaseTower(long cost)
    {
        _gameState.Coins -= cost;
        _uiManager.UpdateCoinsLabel(_gameState.Coins);
    }

    private void OnEnemyDefeated(long reward)
    {
        _gameState.Coins += reward;
        _uiManager.UpdateCoinsLabel(_gameState.Coins);
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
}