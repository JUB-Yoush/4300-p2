using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;
using Godot;

public partial class Player : Node2D
{
    enum State
    {
        IDLE,
        STARTUP,
        ATTACKING,
    }

    enum Height
    {
        LOW = 0,
        MEDIUM = 1,
        HIGH = 2,
        NONE,
    }

    Sprite2D Sprite = null!;
    Tween tween = null!;

    Action[] FollowUps = [];
    Action[] SetUps = [];

    bool CanFollowUp = false;
    bool tweening = false;

    //delete when animation are in
    [Export]
    public required Texture2D idle,
        startup,
        attack;
    const float STARTUP_TIME = 0.5f;
    const float HEAVY_TIME = 1f;
    const float LIGHT_TIME = 0.5f;
    const float BLOCK_TIME = 2f;
    private State state;
    private Height height,
        attackingHeight;
    Timer StartupTimer = null!;
    Timer AttackTimer = null!;
    Timer BlockTimer = null!;

    public override void _Ready()
    {
        Sprite = GetNode<Sprite2D>("Sprite2D");

        tween = CreateTween();

        // should be a multidimensional list for the 2 follow ups off of every set up
        FollowUps = new Action[] { LowFollowUp, MidFollowUp, HighFollowUp };
        SetUps = new Action[] { LowSetUp, MidSetUp, HighSetUp };

        StartupTimer = new Timer();
        AttackTimer = new Timer();
        BlockTimer = new Timer();
        AddChild(StartupTimer);
        AddChild(AttackTimer);
        AddChild(BlockTimer);

        state = State.IDLE;
        height = Height.NONE;
        GD.Print(SetUps.Length);
    }

    void UpdateDebugPanel()
    {
        ((Label)GetParent().FindChild("CancelLabel")).Text = CanFollowUp.ToString();
        ((Label)GetParent().FindChild("StateLabel")).Text = state.ToString();
        ((Label)GetParent().FindChild("HeightLabel")).Text = height.ToString();
    }

    public override void _PhysicsProcess(double delta)
    {
        //GD.Print(CanFollowUp, tweening);
        if (Input.IsActionJustPressed("high_attack"))
        {
            ProcessAttack(Height.HIGH);
        }
        else if (Input.IsActionJustPressed("medium_attack"))
        {
            ProcessAttack(Height.MEDIUM);
        }
        else if (Input.IsActionJustPressed("low_attack"))
        {
            ProcessAttack(Height.LOW);
        }
        UpdateDebugPanel();
    }

    void ProcessAttack(Height level)
    {
        if (state == State.STARTUP)
        {
            if (!CanFollowUp || level == height)
            {
                return;
            }

            tween.Stop();
            state = State.ATTACKING;
            FollowUps[(int)level]();
        }
        else if (state == State.IDLE)
        {
            height = level;
            state = State.STARTUP;
            GD.Print((int)height);
            SetUps[(int)height]();
        }
    }

    void MidSetUp()
    {
        CanFollowUp = false;
        var subtween = CreateTween();
        subtween.TweenCallback(Callable.From(() => Sprite.Frame = 0));
        subtween.SetTrans(Tween.TransitionType.Quad);
        subtween.TweenProperty(this, "position", new Vector2(Position.X - 50, Position.Y), .8f);
        //subtween.Parallel().TweenSubtween(DelayedCallable(() => Sprite.Frame = 1, .3f));
        subtween.DelayedCallable(CreateTween(), () => Sprite.Frame = 1, .3f);

        tween = CreateTween();
        tween.TweenSubtween(subtween);

        tween.TweenCallback(Callable.From(() => Sprite.Frame = 2));
        tween.SetTrans(Tween.TransitionType.Quad);
        tween.TweenProperty(this, "position", new Vector2(Position.X, Position.Y), .8f);
        tween.DelayedCallable(
            CreateTween(),
            () =>
            {
                CanFollowUp = true;
                Sprite.Frame = 3;
            },
            .3f
        );
        tween.TweenCallback(Callable.From(() => Sprite.Frame = 1));
        tween.TweenCallback(Callable.From(() => CanFollowUp = false));
        tween.TweenCallback(Callable.From(() => state = State.IDLE));
    }

    void HighSetUp()
    {
        // just calls mid for now
        MidSetUp();
    }

    void LowSetUp()
    {
        // just calls mid for now
        MidSetUp();
    }

    // the same thing 3 times
    void HighFollowUp()
    {
        tweening = true;
        CanFollowUp = false;
        tween = CreateTween();

        tween.TweenCallback(Callable.From(() => Sprite.Frame = 5));
        tween.SetTrans(Tween.TransitionType.Quad);
        tween.TweenProperty(this, "position", new Vector2(Position.X + 20, Position.Y), .4f);
        tween.TweenCallback(Callable.From(() => CanFollowUp = false));
        tween.TweenCallback(Callable.From(() => state = State.IDLE));
    }

    void LowFollowUp()
    {
        tweening = true;
        CanFollowUp = false;
        tween = CreateTween();

        tween.TweenCallback(Callable.From(() => Sprite.Frame = 4));
        tween.SetTrans(Tween.TransitionType.Quad);
        tween.TweenProperty(this, "position", new Vector2(Position.X + 20, Position.Y), .4f);
        tween.TweenCallback(Callable.From(() => CanFollowUp = false));
        tween.TweenCallback(Callable.From(() => state = State.IDLE));
    }

    void MidFollowUp()
    {
        tweening = true;
        CanFollowUp = false;
        tween = CreateTween();

        tween.TweenCallback(Callable.From(() => Sprite.Frame = 6));
        tween.SetTrans(Tween.TransitionType.Quad);
        tween.TweenProperty(this, "position", new Vector2(Position.X + 20, Position.Y), .4f);
        tween.TweenCallback(Callable.From(() => CanFollowUp = false));
        tween.TweenCallback(Callable.From(() => state = State.IDLE));
    }
}

// should go in dedicated extension method definition file
public static class Helpers
{
    public static void DelayedCallable(this Tween tween, Tween newTween, Action func, float time)
    {
        newTween.TweenInterval(time);
        newTween.TweenCallback(Callable.From(func));
        tween.Parallel().TweenSubtween(newTween);
    }
}
