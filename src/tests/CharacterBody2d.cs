using System;
using System.Security;
using Godot;

public partial class CharacterBody2d : CharacterBody2D
{
    Tween movementTween = null!;
    bool canMove = true;

    public override void _Ready() { }

    private void collisionCheck(Node2D body)
    {
        if (body is StaticBody2D)
        {
            canMove = false;
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        MoveAndSlide();
    }

    public override void _Process(double delta)
    {
        GD.Print(Input.GetConnectedJoypads());
    }
}
