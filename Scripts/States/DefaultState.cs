using Godot;

public class DefaultState : State
{
    public DefaultState(GameScene gameScene) : base(gameScene)
    {
    }


    public override void Process(double delta)
    {
        
    }
    public override void PhysicsProcess(double delta)
    {
        // TODO
    }

    public override void HandleButton(ButtonEnum button)
    {
        switch (button)
        {
            case ButtonEnum.BuildRouteButton:
                InitiateBuildMode();
                return;
            case ButtonEnum.BuildTowerButton:
                BuildTowerButtonPressed();
                return;
            default:
                GD.PrintErr("Unhandled button of type " + button.ToString());
                return;
        }
    }

	private void InitiateBuildMode()
	{
		if (_gameScene.GameState.RouteLength - _gameScene.CurrentRouteLength <= 0) {
			return;
		}
		_gameScene.ChangeState(new BuildRouteState(_gameScene));
	}

	public void BuildTowerButtonPressed()
	{
		_gameScene.ChangeState(new BuildTowerState(_gameScene, new BaseTower()));
	}
}