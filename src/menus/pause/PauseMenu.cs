using Godot;
using System;

public partial class PauseMenu : Control
{
    public static PackedScene packedScene = GD.Load<PackedScene>("res://src/scenes/controls.tscn");

    public override void _Ready()
    {
        GetTree().Paused = true;
        Input.MouseMode = Input.MouseModeEnum.Visible;
        GetNode<TextureButton>("Paused/Resume").Pressed += () =>
        {
            Input.MouseMode = Input.MouseModeEnum.Captured;
            GetTree().Paused = false;
            QueueFree();
        };
        GetNode<TextureButton>("Paused/Home").Pressed += () =>
        {
            GetTree().Paused = false;
            GetTree().ChangeSceneToFile("res://src/menus/title/title.tscn");
        };
    }
}
