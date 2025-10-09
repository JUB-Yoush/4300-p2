using Godot;
using System;

public partial class ResultScreens : Control
{
    //lose screen ref
    //https://commons.wikimedia.org/wiki/File:Television_static.gif
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
