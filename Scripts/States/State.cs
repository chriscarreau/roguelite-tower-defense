public abstract class State 
{
	public GameScene GameScene;
    public abstract void Process(double delta);
    public abstract void PhysicsProcess(double delta);

    public abstract void HandleButton(ButtonEnum button);
}