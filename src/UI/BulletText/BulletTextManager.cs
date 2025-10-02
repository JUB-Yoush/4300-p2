using System;
using Godot;

public partial class BulletTextManager : Node
{
    [Export]
    private string[] positiveChats = null!,
        negativeChats = null!;
    string textPath = "res://src/UI/BulletText/bullet_message.tscn";
    PackedScene? textScene;

    private double reputation = -1000;
    private const double REPUTATION_LIMIT = 1000;
    private const double REPUTATION_DECAY = 150;
    private const float SPAWN_BUFFER_SPACE = 200;

    public override void _Ready()
    {
        textScene = GD.Load<PackedScene>(textPath);
    }

    public void InfluenceReputation(double impact)
    {
        reputation = double.Clamp(reputation + impact, -REPUTATION_LIMIT, REPUTATION_LIMIT);
    }

    public void ReputationOverride(double newValue)
    {
        reputation = double.Clamp(newValue, -REPUTATION_LIMIT, REPUTATION_LIMIT);
    }

    public override void _PhysicsProcess(double delta)
    {
        reputation -= delta * REPUTATION_DECAY * (reputation / REPUTATION_LIMIT);

        if (GD.Randi() % (REPUTATION_LIMIT + 1) < Math.Abs(reputation))
        {
            var msg = textScene!.Instantiate<RichTextLabel>();
            msg.Text = GetMessage();
            msg.SetSize(msg.GetThemeDefaultFont().GetStringSize(msg.Text));
            AddChild(msg);
            int direction = (int)(GD.Randi() % 2) * 2 - 1;
            ((BulletMessage)msg).direction = -direction;
            var ScreenSize = GetViewport().GetVisibleRect().Size;
            msg.Position = new Vector2(
                (ScreenSize.X / 2)
                    + (ScreenSize.X / 2 * direction)
                    + (msg.Size.X * direction)
                    + (SPAWN_BUFFER_SPACE * direction),
                GD.Randi() % ScreenSize.Y
            );
        }
    }

    private string GetMessage()
    {
        string[] chosen = positiveChats;
        if ((GD.Randi() % ((REPUTATION_LIMIT * 2) + 1)) - REPUTATION_LIMIT + (reputation * 4) < 0)
            chosen = negativeChats;
        return chosen[GD.Randi() % chosen.Length];
    }
}
