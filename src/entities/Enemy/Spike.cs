using System;
using System.Security;
using Godot;
using static Helpers;

public partial class Spike : Area2D
{
    public static PackedScene PackedScene = GD.Load<PackedScene>(
        "res://src/entities/Enemy/Spike.tscn"
    );

    Sprite2D Sprite = null!;
    CollisionShape2D hbox = null!;
    Enemy enemy = null!;

    public override void _Ready()
    {
        Sprite = GetNode<Sprite2D>("Sprite2D");
        SlideUp();
    }

    public void SlideUp()
    {
        var tween = CreateTween();
        tween.SetEase(Tween.EaseType.Out);
        tween.TweenProperty(
            this,
            "position",
            new Vector2(Position.X, Position.Y - 400),
            FramesToSeconds(10)
        );
    }
}
