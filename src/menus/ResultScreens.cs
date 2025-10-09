using Godot;
using System;

public partial class ResultScreens : Node
{
    public override void _Ready()
    {
        Input.MouseMode = Input.MouseModeEnum.Visible;
        GetNode<TextureButton>("Result/Reset").Pressed += () =>
        {
            Input.MouseMode = Input.MouseModeEnum.Captured;
            GetTree().Paused = false;
            GetTree().ReloadCurrentScene();
        };
        GetNode<TextureButton>("Result/Home").Pressed += () =>
        {
            GetTree().Paused = false;
            GetTree().ChangeSceneToFile("res://src/menus/title/title.tscn");
        };
        GetNode<TextureButton>("Result/Continue").Pressed += () =>
        {
            GetTree().Paused = false;
            GetTree().ChangeSceneToFile("res://src/menus/end/end_screen.tscn");
        };
    }
}
