using System;
using System.Security;
using Godot;

public partial class CharacterBody2d : CharacterBody2D
{
    Tween movementTween = null!;

    public override void _Ready() { }

    public override void _Process(double delta)
    {
        var position = Position;
        if (position.X <= 500)
        {
            position.X = 500;
            movementTween!.Stop();
            Position = position;
        }
        if (
            Input.IsActionJustPressed("block")
            && (movementTween == null || !movementTween.IsRunning())
        )
        {
            var tween = CreateTween();
            tween.TweenProperty(this, "rotation", 180, 3);
            movementTween = tween.MakeMovementTween(this, new(Position.X - 500, Position.Y), 2);
        }
        //GD.Print(movementTween.IsRunning());
    }
}
