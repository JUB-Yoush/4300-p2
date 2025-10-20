using System;
using System.Collections.Generic;
using Godot;

public partial class InputManager : Node
{
    enum Controllers
    {
        PLAYER_ONE = 0,
        PLAYER_TWO = 1,
        PLAYER_THREE = 2,
    }

    public enum Inputs
    {
        HIGH,
        MID,
        LOW,
        BLOCK,
    }

    bool multiplayer = true;

    public static Dictionary<Inputs, bool> Map = new()
    {
        { Inputs.LOW, false },
        { Inputs.MID, false },
        { Inputs.HIGH, false },
        { Inputs.BLOCK, false },
    };

    public override void _Process(double delta)
    {
        if (multiplayer)
        {
            Map[Inputs.HIGH] =
                Input.IsActionJustPressed("high_attack")
                && Input.IsJoyButtonPressed((int)Controllers.PLAYER_ONE, JoyButton.X);
            Map[Inputs.MID] =
                Input.IsActionJustPressed("mid_attack")
                && Input.IsJoyButtonPressed((int)Controllers.PLAYER_TWO, JoyButton.X);
            Map[Inputs.LOW] =
                Input.IsActionJustPressed("low_attack")
                && Input.IsJoyButtonPressed((int)Controllers.PLAYER_THREE, JoyButton.X);

            Map[Inputs.BLOCK] = Input.IsActionPressed("block");
        }
        else
        {
            Map[Inputs.HIGH] = Input.IsActionJustPressed("high_attack");
            Map[Inputs.MID] = Input.IsActionJustPressed("mid_attack");
            Map[Inputs.LOW] = Input.IsActionJustPressed("low_attack");
            Map[Inputs.BLOCK] = Input.IsActionPressed("block");
        }
        if (Input.IsActionJustPressed("toggle_multiplayer"))
        {
            multiplayer = !multiplayer;
        }
    }
}
