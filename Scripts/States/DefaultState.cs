using Godot;

public class DefaultState : State
{
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
		if (GameScene.GameState.RouteLength - GameScene.CurrentRouteLength <= 0) {
			return;
		}
		GameScene.ChangeState(new BuildRouteState());
	}

	public void BuildTowerButtonPressed()
	{
		GameScene.ChangeState(new BuildTowerState());
	}
}