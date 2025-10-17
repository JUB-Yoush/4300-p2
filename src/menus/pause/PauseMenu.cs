using Godot;
using System;

public partial class PauseMenu : Control
{
	public static PackedScene packedScene = GD.Load<PackedScene>("res://src/scenes/controls.tscn");

	public override void _Ready()
	{
		GetNode<TextureButton>("Paused/Resume").Pressed += () =>
		{
			Input.MouseMode = Input.MouseModeEnum.Captured;
			GetTree().Paused = false;
			GetNode<Panel>("Paused").Visible = false;
		};
		GetNode<TextureButton>("Paused/Home").Pressed += () =>
		{
			GetTree().Paused = false;
			GetTree().ChangeSceneToFile("res://src/menus/title/title.tscn");
		};
	}

	public override void _PhysicsProcess(double delta)
	{
		if (Input.IsActionJustPressed("pause") && !GetTree().Paused)
		{
			GetNode<TextureButton>("Paused/Home").GrabFocus();
			Input.MouseMode = Input.MouseModeEnum.Visible;
			GetNode<Panel>("Paused").Visible = true;
			GetTree().Paused = true;
		}
		else if (Input.IsActionJustPressed("pause") && GetTree().Paused)
		{
			Input.MouseMode = Input.MouseModeEnum.Captured;
			GetTree().Paused = false;
			GetNode<Panel>("Paused").Visible = false;
		}
		else if (Input.IsActionJustPressed("win_shortcut") && !GetTree().Paused)
		{
			Input.MouseMode = Input.MouseModeEnum.Visible;
			GetParent().GetNode<ResultScreens>("win_screen").Visible = true;
			GetParent().GetNode<ResultScreens>("win_screen").GetNode<TextureButton>("Result/Home").GrabFocus();
			GetTree().Paused = true;
		}
		else if (Input.IsActionJustPressed("lose_shortcut") && !GetTree().Paused)
		{
			Input.MouseMode = Input.MouseModeEnum.Visible;
			GetParent().GetNode<ResultScreens>("lose_screen").Visible = true;
			GetParent().GetNode<ResultScreens>("lose_screen").GetNode<TextureButton>("Result/Home").GrabFocus();
			GetTree().Paused = true;
		}
	}
}
