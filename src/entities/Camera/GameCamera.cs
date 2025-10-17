using System;
using System.Data;
using System.Linq;
using Godot;

public partial class GameCamera : Camera2D
{
    private static readonly Vector2 Resolution = new(1920, 1080);
    private static readonly int[] CameraLimits = [-670, 2797];
    const int SCREEN_WIDTH = 1400;
    const int CAMERA_HEIGHT = 580;

    Player player = null!;
    Enemy enemy = null!;
    Vector2 playerToEnemyDist = Vector2.Zero;

    CollisionShape2D LeftWall = null!;
    CollisionShape2D RightWall = null!;

    /*
     * attach static bodies to the bounds of the camera
     * calculate the distance between the player and enemy
     * if it's larger than the width of the screen (or some slightly smaller value) then
     */

    public override void _Ready()
    {
        player = GetParent().GetNode<Player>("Player");
        enemy = GetParent().GetNode<Enemy>("Enemy");
        LeftWall = GetNode<CollisionShape2D>("Border/LeftWall");
        RightWall = GetNode<CollisionShape2D>("Border/RightWall");
    }

    public override void _Process(double delta)
    {
        var midpoint = (player.GlobalPosition.X + enemy.GlobalPosition.X) / 2;
        playerToEnemyDist = enemy.GlobalPosition - player.GlobalPosition;
        if (playerToEnemyDist.X >= SCREEN_WIDTH)
        {
            SetWalls(true);
        }
        else
        {
            SetWalls(false);
        }
        GlobalPosition = new(midpoint, CAMERA_HEIGHT);

        LeftWall.GlobalPosition = LeftWall.GlobalPosition with
        {
            X = Math.Max(GlobalPosition.X - Resolution.X / 2, CameraLimits[0]),
        };

        RightWall.GlobalPosition = RightWall.GlobalPosition with
        {
            X = Math.Min(GlobalPosition.X + Resolution.X / 2, CameraLimits[1]),
        };
    }

    void SetWalls(bool state)
    {
        LeftWall.Disabled = !state;
        RightWall.Disabled = !state;
    }
}
