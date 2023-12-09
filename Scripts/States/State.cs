public abstract class State 
{
	protected GameScene _gameScene;

    protected State(GameScene gameScene) {
        _gameScene = gameScene;
    }
    
    public abstract void Process(double delta);
    public abstract void PhysicsProcess(double delta);

    public abstract void HandleButton(ButtonEnum button);
}