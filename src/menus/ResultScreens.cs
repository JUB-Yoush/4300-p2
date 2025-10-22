using System;
using Godot;

public partial class ResultScreens : Control
{
    //lose screen ref
    //https://commons.wikimedia.org/wiki/File:Television_static.gif
    public override void _Ready()
	{
		if (Name == "lose_screen")
			AudioManager.PlayMusic(BGM.Static);
        GetTree().Paused = false;
        Input.MouseMode = Input.MouseModeEnum.Visible;
        GetNode<TextureButton>("Result/Home").GrabFocus();
        GetNode<TextureButton>("Result/Reset").Pressed += () =>
		{
			if (Name == "lose_screen")
			{
				AudioManager.StopAll();
				AudioManager.PlayMusic(BGM.FightMusic);
			}
            AudioManager.PlaySfx(SFX.UIClick);
            Input.MouseMode = Input.MouseModeEnum.Captured;
            GetTree().Paused = false;
            GetTree().ChangeSceneToFile("res://src/toplevel_scenes/main.tscn");
        };
        GetNode<TextureButton>("Result/Home").Pressed += () =>
		{
			if (Name == "lose_screen")
				AudioManager.StopAll();
            AudioManager.PlaySfx(SFX.UIClick);
            GetTree().Paused = false;
            GetTree().ChangeSceneToFile("res://src/menus/title/title.tscn");
        };
		GetNode<TextureButton>("Result/Continue").Pressed += () =>
		{
			AudioManager.PlaySfx(SFX.UIClick);
			GetTree().Paused = false;
			GetTree().ChangeSceneToFile("res://src/menus/end/end_screen.tscn");
		};

		GetNode<TextureButton>("Result/Continue").FocusEntered += () =>
		{
			AudioManager.PlaySfx(SFX.UIHover);
		};
		GetNode<TextureButton>("Result/Home").FocusEntered += () =>
		{
			AudioManager.PlaySfx(SFX.UIHover);
		};
		GetNode<TextureButton>("Result/Reset").FocusEntered += () =>
		{
			AudioManager.PlaySfx(SFX.UIHover);
		};
    }
}
