using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Godot;
using static Helpers;

public partial class Player : Node2D
{
    public enum Move
    {
        IDLE,
        L,
        M,
        H,
        LM,
        LH,
        ML,
        MH,
        HL,
        HM,
    }

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
    Area2D HitboxArea = null!;
    Area2D HurtboxArea = null!;

    public override void _Ready()
    {
        Sprite = GetNode<Sprite2D>("Sprite2D");

        // should be a multidimensional list for the 2 follow ups off of every set up
        FollowUps = [LowFollowUp, MidFollowUp, HighFollowUp];
        SetUps = [LowSetUp, MidSetUp, HighSetUp];

        state = State.IDLE;
        height = Height.NONE;
        HurtboxArea = GetNode<Area2D>("HurtboxArea");
        HitboxArea = GetNode<Area2D>("HitboxArea");
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
        //subtween.TweenCallback(Callable.From(() => Sprite.Frame = 0));
        subtween.Call(() => Sprite.Frame = 0);
        subtween.Call(() => UpdateCollisionBox(CollisionBoxes.BoxType.HURTBOX, Move.M));
        subtween.SetTrans(Tween.TransitionType.Quad);
        subtween.TweenProperty(
            this,
            "position",
            new Vector2(Position.X - 50, Position.Y),
            FramesToSeconds(24)
        );
        subtween.DelayedCallable(CreateTween(), () => Sprite.Frame = 1, FramesToSeconds(12));

        tween = CreateTween();
        tween.TweenSubtween(subtween);

        tween.Call(() => Sprite.Frame = 2);
        tween.SetTrans(Tween.TransitionType.Quad);
        tween.TweenProperty(
            this,
            "position",
            new Vector2(Position.X, Position.Y),
            FramesToSeconds(24)
        );
        tween.DelayedCallable(
            CreateTween(),
            () =>
            {
                CanFollowUp = true;
                Sprite.Frame = 3;
            },
            FramesToSeconds(12)
        );

        tween.Call(() =>
        {
            CanFollowUp = false;
            state = State.IDLE;
            UpdateCollisionBox(CollisionBoxes.BoxType.HURTBOX, Move.IDLE);
            Sprite.Frame = 7;
        });
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

    void HighFollowUp()
    {
        tweening = true;
        CanFollowUp = false;
        tween = CreateTween();

        tween.TweenCallback(Callable.From(() => Sprite.Frame = 5));
        tween.Call(() => UpdateCollisionBox(CollisionBoxes.BoxType.HITBOX, Move.MH));
        tween.SetTrans(Tween.TransitionType.Quad);
        tween.TweenProperty(
            this,
            "position",
            new Vector2(Position.X + 20, Position.Y),
            FramesToSeconds(24)
        );
        tween.Call(() =>
        {
            CanFollowUp = false;
            state = State.IDLE;
            ResetCollisionBox(CollisionBoxes.BoxType.HITBOX);
            UpdateCollisionBox(CollisionBoxes.BoxType.HURTBOX, Move.IDLE);
            Sprite.Frame = 7;
        });
    }

    void LowFollowUp()
    {
        tweening = true;
        CanFollowUp = false;
        tween = CreateTween();
        tween.Call(() => UpdateCollisionBox(CollisionBoxes.BoxType.HITBOX, Move.ML));

        tween.TweenCallback(Callable.From(() => Sprite.Frame = 4));
        tween.SetTrans(Tween.TransitionType.Quad);
        tween.TweenProperty(
            this,
            "position",
            new Vector2(Position.X + 20, Position.Y),
            FramesToSeconds(24)
        );
        tween.Call(() =>
        {
            CanFollowUp = false;
            state = State.IDLE;
            ResetCollisionBox(CollisionBoxes.BoxType.HITBOX);
            UpdateCollisionBox(CollisionBoxes.BoxType.HURTBOX, Move.IDLE);
            Sprite.Frame = 7;
        });
    }

    void MidFollowUp()
    {
        tweening = true;
        CanFollowUp = false;
        tween = CreateTween();

        tween.Call(() => UpdateCollisionBox(CollisionBoxes.BoxType.HITBOX, Move.LM));
        tween.TweenCallback(Callable.From(() => Sprite.Frame = 6));
        tween.SetTrans(Tween.TransitionType.Quad);
        tween.TweenProperty(
            this,
            "position",
            new Vector2(Position.X + 20, Position.Y),
            FramesToSeconds(24)
        );
        tween.Call(() =>
        {
            CanFollowUp = false;
            state = State.IDLE;
            ResetCollisionBox(CollisionBoxes.BoxType.HITBOX);
            UpdateCollisionBox(CollisionBoxes.BoxType.HURTBOX, Move.IDLE);
            Sprite.Frame = 7;
        });
    }

    void UpdateCollisionBox(CollisionBoxes.BoxType boxtype, Move move)
    {
        ResetCollisionBox(boxtype);

        var area = boxtype == CollisionBoxes.BoxType.HITBOX ? HitboxArea : HurtboxArea;
        area.GetNode<CollisionShape2D>(move.ToString()).Disabled = false;
    }

    void ResetCollisionBox(CollisionBoxes.BoxType boxtype)
    {
        var area = boxtype == CollisionBoxes.BoxType.HITBOX ? HitboxArea : HurtboxArea;
        area.GetChildren()
            .OfType<CollisionShape2D>()
            .ToList()
            .ForEach(child => child.Disabled = true);
    }
}

public static class CollisionBoxes
{
    public enum BoxType
    {
        HITBOX,
        HURTBOX,
    }
}

// should go in dedicated extension method definition file
public static class Helpers
{
    public static void DelayedCallable(this Tween tween, Tween newTween, Action action, float time)
    {
        newTween.TweenInterval(time);
        newTween.TweenCallback(Callable.From(action));
        tween.Parallel().TweenSubtween(newTween);
    }

    public static void Call(this Tween tween, Action action)
    {
        tween.TweenCallback(Callable.From(action));
    }

    public static float FramesToSeconds(int frames)
    {
        return (float)frames / 60;
    }
}
