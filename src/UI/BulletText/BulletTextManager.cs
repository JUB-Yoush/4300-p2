using System;
using System.Security;
using Godot;
using Microsoft.VisualBasic;

public partial class BulletTextManager : Node
{
    [Export]
    private string[] positiveChats = null!,
        negativeChats = null!,
        chosen = null!;
    PackedScene textScene = GD.Load<PackedScene>("res://src/UI/BulletText/bullet_message.tscn");
    Camera2D Camera = null!;
    private double reputation = 250;
    private const double REPUTATION_LIMIT = 1000;
    private const double REPUTATION_DECAY = 150;
    private const float SPAWN_BUFFER_SPACE = 200;
    float MessageFreq = .3f;
    Timer MessageTimer = new();

    public void InfluenceReputation(double impact)
    {
        reputation = double.Clamp(reputation + impact, -REPUTATION_LIMIT, REPUTATION_LIMIT);
    }

    public void ReputationOverride(double newValue)
    {
        reputation = double.Clamp(newValue, -REPUTATION_LIMIT, REPUTATION_LIMIT);
    }

    public override void _Ready()
    {
        Camera = GetParent().GetNode<Camera2D>("GameCamera");
        AddChild(MessageTimer);
        MessageTimer.Timeout += AddChatMsg;
        MessageTimer.Start(GetNextChatTime());
    }

    void AddChatMsg()
    {
        var msg = textScene.Instantiate<BulletMessage>();
        msg.Text = GetMessage();
        msg.SetSize(msg.GetThemeDefaultFont().GetStringSize(msg.Text));
        AddChild(msg);

        int direction = chosen == negativeChats ? 1 : -1;

        msg.direction = -direction;

        var ScreenSize = GetViewport().GetVisibleRect().Size;

        var Pos_X =
            (ScreenSize.X / 2)
            + (ScreenSize.X / 2 * direction)
            + (msg.Size.X * direction)
            + (SPAWN_BUFFER_SPACE * direction)
            + Camera.Position.X / 2;

        msg.Position = new Vector2(Pos_X, GD.Randi() % ScreenSize.Y);
        MessageTimer.Start(GetNextChatTime());
    }

    float GetNextChatTime()
    {
        float repScale = (float)Math.Abs(reputation);
        return Math.Max(750 - repScale, 50) / 1000;
    }

    public override void _PhysicsProcess(double delta)
    {
        reputation -= delta * REPUTATION_DECAY * (reputation / REPUTATION_LIMIT);
    }

    private string GetMessage()
    {
        chosen = positiveChats;
        if ((GD.Randi() % ((REPUTATION_LIMIT * 2) + 1)) - REPUTATION_LIMIT + (reputation * 4) < 0)
            chosen = negativeChats;
        return chosen[GD.Randi() % chosen.Length];
    }
}
