using Godot;
using System;

public partial class WarningArea : GpuParticles2D
{
    [Export]
    Control child;
    public override void _Ready()
    {
        Activate();
    }

    private async void Activate()
    {
        child.Visible = true;
        GetTree().CreateTimer(0.1f).Timeout += Deactivate;
    }

    private async void Deactivate()
    {
        child.Visible = false;
        GetTree().CreateTimer(0.1f).Timeout += Activate;
    }
}
