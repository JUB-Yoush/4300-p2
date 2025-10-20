using System;
using Godot;

public partial class BulletMessage : Label
{
    private const float KILL_TIME = 20;
    public float scrollSpeed = 200;
    public int direction = 1;
    private Timer killTimer = null!;

    public override void _Ready()
    {
        scrollSpeed += GD.Randi() % 51 - 25;
        killTimer = new Timer();
        AddChild(killTimer);
        killTimer.Timeout += QueueFree;
        killTimer.Start(KILL_TIME);
    }

    public override void _PhysicsProcess(double delta)
    {
        Position += new Vector2((float)delta * scrollSpeed * direction, 0);
    }
}
