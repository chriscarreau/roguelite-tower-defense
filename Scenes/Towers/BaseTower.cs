using Godot;
using System;

public partial class BaseTower : Tower
{
    public BaseTower()
    {
        TowerData = ResourceLoader.Load<TowerResource>("res://Resources/Towers/BaseTower.tres");
    }

}
