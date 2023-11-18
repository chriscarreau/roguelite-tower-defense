using System;
using Godot;

public class BuildTowerState : State
{
    private bool _canBuild;

    public override void HandleButton(ButtonEnum button)
    {
        switch (button)
        {
            case ButtonEnum.BuildRouteButton:
                CancelBuildTower();
				GameScene.ChangeState(new BuildRouteState());
                return;
            case ButtonEnum.BuildTowerButton:
                CancelBuildTower();
				GameScene.ChangeState(new DefaultState());
                return;
            default:
                GD.PrintErr("Unhandled button of type " + button.ToString());
                return;
        }
    }

    public override void Process(double delta)
    {
        UpdateTowerPreview();
    }

    public override void PhysicsProcess(double delta)
    {
        if (Input.IsActionJustPressed("build"))
		{
			AddTower();
		}
		if (Input.IsActionPressed("cancel"))
		{
			CancelBuildTower();
			GameScene.ChangeState(new DefaultState());
		}
    }

    private void UpdateTowerPreview()
    {
        var mousePosition = GameScene.GetGlobalMousePosition();
		var currentTile = GameScene.Map.LocalToMap(GameScene.ToLocal(mousePosition));
		var tilePosition = GameScene.ToGlobal(GameScene.Map.MapToLocal(currentTile));
		tilePosition.X -= 32;
		tilePosition.Y -= 32;
		GameScene.CurrentBuildLocation = currentTile;
		if (GameScene.Preview == null){
			GameScene.Preview = new Control();
            var overlay = new TextureRect
            {
                Texture = GD.Load<Texture2D>("res://Assets/UI/UI/Overlay.png"),
                Modulate = new Color(1, 0, 0),
                ZIndex = 5,
                Name = "Overlay"
            };
            var scenePath = $"res://Scenes/Towers/BaseTower.tscn";
		    var towerScene = ResourceLoader.Load<PackedScene>(scenePath).Instantiate<Tower>();
            towerScene.Position = new Vector2(32, 32);
			overlay.AddChild(towerScene);
			GameScene.Preview.AddChild(overlay);
			GameScene.AddChild(GameScene.Preview);
		}
		GameScene.Preview.Position = tilePosition;
		if (CheckCanBuild(GameScene.CurrentBuildLocation)){
			_canBuild = true;
			GameScene.Preview.GetNode<TextureRect>("Overlay").Modulate = new Color(0,1,0);
		} else {
			_canBuild = false;
			GameScene.Preview.GetNode<TextureRect>("Overlay").Modulate = new Color(1,0,0);
		}
    }
    private bool CheckCanBuild(Vector2I currentBuildLocation)
    {
        return GameScene.Map.GetCellTileData(1, currentBuildLocation) == null;
    }

    private void AddTower()
    {
        if (!_canBuild) {
			return;
		}
        var scenePath = $"res://Scenes/Towers/BaseTower.tscn";
        var towerScene = ResourceLoader.Load<PackedScene>(scenePath).Instantiate<BaseTower>();
        towerScene.Position = GameScene.Preview.Position + new Vector2(32, 32);
        GameScene.AddChild(towerScene);
    }

    private void CancelBuildTower()
    {
		GameScene.RemoveChild(GameScene.Preview);
		GameScene.Preview.QueueFree();
		GameScene.Preview = null;
    }
}