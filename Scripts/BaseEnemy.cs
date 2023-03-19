using Godot;

public partial class BaseEnemy : PathFollow2D
{
    [Export]
    public int Speed = 100;

    [Export]
    public int Health = 100;

    public override void _Ready()
    {
    }

    public override void _PhysicsProcess(double delta)
    {
        Move(delta);
    }

    public void OnHit()
    {

    }

    public void OnDestroy()
    {

    }

    private void Move(double delta)
    {
        this.Progress += (float)(Speed * delta);
    }
}