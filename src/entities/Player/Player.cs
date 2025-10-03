using System;
using System.Linq;
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

    public enum Height
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

    private State state;
    private Height setupHeight,
        AttackHeight;

    public Height BlockHeight = Height.NONE;
    Area2D HitboxArea = null!;
    Area2D HurtboxArea = null!;

    public override void _Ready()
    {
        Sprite = GetNode<Sprite2D>("Sprite2D");

        // should be a multidimensional list for the 2 follow ups off of every set up
        FollowUps = [LowFollowUp, MidFollowUp, HighFollowUp];
        SetUps = [LowSetUp, MidSetUp, HighSetUp];

        state = State.IDLE;
        setupHeight = Height.NONE;

        HurtboxArea = GetNode<Area2D>("HurtboxArea");
        HurtboxArea.CollisionLayer = (uint)CollisionLayer.PLAYER_HURT;
        HurtboxArea.CollisionMask = (uint)CollisionLayer.ENEMY_HIT;
        HurtboxArea.AreaEntered += HitByEnemy;

        HitboxArea = GetNode<Area2D>("HitboxArea");
        HitboxArea.CollisionLayer = (uint)CollisionLayer.PLAYER_HIT;
        HitboxArea.CollisionMask = (uint)CollisionLayer.ENEMY_HURT;
        HitboxArea.AreaEntered += HitEnemy;
    }

    void HitByEnemy(Area2D area)
    {
        GD.Print("got hit");

        var enemy = area.GetParent<Enemy>();
        if (BlockHeight == enemy.AttackHeight)
        {
            // TODO attack blocking
            return;
        }
    }

    void HitEnemy(Area2D area)
    {
        GD.Print("you hit the enemy");
        var enemy = area.GetParent<Enemy>();
        if (enemy.BlockHeight == AttackHeight)
        {
            // TODO attack blocking
            return;
        }
        enemy.GotHit();
        // play hit effect
        // apply hit stun
        // switch to hit animation on enemy
    }

    void UpdateDebugPanel()
    {
        ((Label)GetParent().GetNode("%CancelLabel")).Text = CanFollowUp.ToString();
        ((Label)GetParent().GetNode("%StateLabel")).Text = state.ToString();
        ((Label)GetParent().GetNode("%HeightLabel")).Text = setupHeight.ToString();
        ((Label)GetParent().GetNode("%AttackLabel")).Text = AttackHeight.ToString();
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
            if (!CanFollowUp || level == setupHeight)
            {
                return;
            }

            tween.Stop();
            state = State.ATTACKING;
            AttackHeight = level;
            FollowUps[(int)level]();
        }
        else if (state == State.IDLE)
        {
            setupHeight = level;
            AttackHeight = Height.NONE;
            state = State.STARTUP;
            SetUps[(int)setupHeight]();
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
