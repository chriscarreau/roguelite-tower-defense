using System;
using Godot;

public class BuildTowerState : State
{
    private bool _canBuild;
    private Tower _tower;

    public BuildTowerState(GameScene gameScene, Tower tower) : base(gameScene)
    {
        _tower = tower;
    }


    public override void HandleButton(ButtonEnum button)
    {
        switch (button)
        {
            case ButtonEnum.BuildRouteButton:
                CancelBuildTower();
				_gameScene.ChangeState(new BuildRouteState(_gameScene));
                return;
            case ButtonEnum.BuildTowerButton:
                CancelBuildTower();
				_gameScene.ChangeState(new DefaultState(_gameScene));
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
			_gameScene.ChangeState(new DefaultState(_gameScene));
		}
    }

    private void UpdateTowerPreview()
    {
        var mousePosition = _gameScene.GetGlobalMousePosition();
		var currentTile = _gameScene.Map.LocalToMap(_gameScene.ToLocal(mousePosition));
		var tilePosition = _gameScene.ToGlobal(_gameScene.Map.MapToLocal(currentTile));
		tilePosition.X -= 32;
		tilePosition.Y -= 32;
		_gameScene.CurrentBuildLocation = currentTile;
		if (_gameScene.Preview == null)
        {
            _gameScene.Preview = new Control();
            var overlay = new TextureRect
            {
                Texture = GD.Load<Texture2D>("res://Assets/UI/UI/Overlay.png"),
                Modulate = new Color(1, 0, 0),
                ZIndex = 5,
                Name = "Overlay"
            };
            Tower towerScene = InstantiateTower();
            towerScene.Position = new Vector2(32, 32);
            overlay.AddChild(towerScene);
            _gameScene.Preview.AddChild(overlay);
            _gameScene.AddChild(_gameScene.Preview);
        }
        _gameScene.Preview.Position = tilePosition;
		if (CheckCanBuild(_gameScene.CurrentBuildLocation) && CanAffordTower()){
			_canBuild = true;
			_gameScene.Preview.GetNode<TextureRect>("Overlay").Modulate = new Color(0,1,0);
		} else {
			_canBuild = false;
			_gameScene.Preview.GetNode<TextureRect>("Overlay").Modulate = new Color(1,0,0);
		}
    }

    private Tower InstantiateTower()
    {
        var towerName = _tower.GetType().Name;
        var scenePath = $"res://Scenes/Towers/{towerName}.tscn";
        var towerScene = ResourceLoader.Load<PackedScene>(scenePath).Instantiate<Tower>();
        return towerScene;
    }


    private bool CheckCanBuild(Vector2I currentBuildLocation)
    {
        return _gameScene.Map.GetCellTileData(1, currentBuildLocation) == null;
    }

    private void AddTower()
    {
        if (!_canBuild) {
			return;
		}
        _gameScene.GameStateManager.PurchaseTower(_tower.TowerData.Cost);
        var towerScene = InstantiateTower();
        towerScene.Position = _gameScene.Preview.Position + new Vector2(32, 32);
        _gameScene.AddChild(towerScene);
    }

    private void CancelBuildTower()
    {
		_gameScene.RemoveChild(_gameScene.Preview);
		_gameScene.Preview.QueueFree();
		_gameScene.Preview = null;
    }

    private bool CanAffordTower()
    {
        return _gameScene.GameState.Coins >= _tower.TowerData.Cost;
    }
}