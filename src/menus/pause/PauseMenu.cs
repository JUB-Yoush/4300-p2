using System;
using Godot;

public partial class PauseMenu : Control
{
    public static PackedScene packedScene = GD.Load<PackedScene>("res://src/scenes/controls.tscn");

    public override void _Ready()
    {
        GetNode<TextureButton>("Paused/Resume").Pressed += () =>
		{
			AudioManager.PlaySfx(SFX.UIClick);
            Input.MouseMode = Input.MouseModeEnum.Captured;
            GetTree().Paused = false;
            GetNode<Panel>("Paused").Visible = false;
        };
		GetNode<TextureButton>("Paused/Home").Pressed += () =>
		{
			AudioManager.PlaySfx(SFX.UIClick);
			GetTree().Paused = false;
			GetTree().ChangeSceneToFile("res://src/menus/title/title.tscn");
		};

		GetNode<TextureButton>("Paused/Home").FocusEntered += () =>
		{
			AudioManager.PlaySfx(SFX.UIHover);
		};
		GetNode<TextureButton>("Paused/Resume").FocusEntered += () =>
		{
			AudioManager.PlaySfx(SFX.UIHover);
		};
    }

    public override void _PhysicsProcess(double delta)
    {
        if (Input.IsActionJustPressed("pause") && !GetTree().Paused)
		{
			AudioManager.PlaySfx(SFX.UIClick);
            GetNode<TextureButton>("Paused/Home").GrabFocus();
            Input.MouseMode = Input.MouseModeEnum.Visible;
            GetNode<Panel>("Paused").Visible = true;
            GetTree().Paused = true;
        }
        else if (Input.IsActionJustPressed("pause") && GetTree().Paused)
		{
			AudioManager.PlaySfx(SFX.UIClick);
            Input.MouseMode = Input.MouseModeEnum.Captured;
            GetTree().Paused = false;
            GetNode<Panel>("Paused").Visible = false;
        }
        else if (Input.IsActionJustPressed("win_shortcut") && !GetTree().Paused)
        {
            Input.MouseMode = Input.MouseModeEnum.Visible;
            GetParent().GetNode<ResultScreens>("win_screen").Visible = true;
            GetParent()
                .GetNode<ResultScreens>("win_screen")
                .GetNode<TextureButton>("Result/Home")
                .GrabFocus();
            GetTree().Paused = true;
        }
        else if (Input.IsActionJustPressed("lose_shortcut") && !GetTree().Paused)
        {
            Input.MouseMode = Input.MouseModeEnum.Visible;
            GetParent().GetNode<ResultScreens>("lose_screen").Visible = true;
            GetParent()
                .GetNode<ResultScreens>("lose_screen")
                .GetNode<TextureButton>("Result/Home")
                .GrabFocus();
            GetTree().Paused = true;
        }
    }
}
