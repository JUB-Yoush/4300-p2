using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Godot;
using static Helpers;

public partial class Player : CharacterBody2D
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
        MID = 1,
        HIGH = 2,
        NONE,
    }

    Sprite2D Sprite = null!;
    Tween? tween = null;

    Tween? MovementTween = null;

    Action[,] FollowUps = { };
    Action[] SetUps = [];

    bool CanFollowUp = false;
    bool CanDoStartup = false;
    bool tweening = false;
    bool wasBlocked = false;

    private State state;
    private Height setupHeight,
        AttackHeight;

    public Height BlockHeight = Height.NONE;
    Area2D HitboxArea = null!;
    Area2D HurtboxArea = null!;

    public override void _Ready()
    {
        Sprite = GetNode<Sprite2D>("Sprite2D");

        Action[,] FollowUps1 =
        {
            { null!, LowMidFollowUp, LowHighFollowUp },
            { MidLowFollowUp, null!, MidHighFollowUp },
            { HighLowFollowUp, HighMidFollowUp, null! },
        };
        FollowUps = FollowUps1;
        SetUps = [LowSetUp, MidSetUp, HighSetUp];

        state = State.IDLE;
        setupHeight = Height.NONE;

        HurtboxArea = GetNode<Area2D>("HurtboxArea");
        // HurtboxArea.CollisionLayer = (uint)Collisions.PLAYER_HURT;
        // HurtboxArea.CollisionMask = (uint)Collisions.ENEMY_HIT;
        HurtboxArea.AreaEntered += HitByEnemy;

        HitboxArea = GetNode<Area2D>("HitboxArea");
        // HitboxArea.CollisionLayer = (uint)Collisions.PLAYER_HIT;
        // HitboxArea.CollisionMask = (uint)Collisions.ENEMY_HURT;
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
            if (wasBlocked)
            {
                return;
            }
            // TODO attack blocking
            wasBlocked = true;
            enemy.Blocked();
            GD.Print("they blocked it");
            PushBlock();

            return;
        }
        enemy.GotHit();
        CanDoStartup = true;

        // play hit effect
        // apply hit stun
        // switch to hit animation on enemy
    }

    void PushBlock()
    {
        //CanDoStartup = true;
        //Reset();
        // Velocity = GetMovementVelocity(
        //     Position,
        //     new(Position.X - 20, Position.Y),
        //     FramesToSeconds(2)
        // );
    }

    void UpdateDebugPanel()
    {
        ((Label)GetParent().GetNode("%CancelLabel")).Text = CanFollowUp.ToString();
        ((Label)GetParent().GetNode("%StateLabel")).Text = state.ToString();
        ((Label)GetParent().GetNode("%HeightLabel")).Text = setupHeight.ToString();
        ((Label)GetParent().GetNode("%AttackLabel")).Text = AttackHeight.ToString();
        ((Label)GetParent().GetNode("%VelLabel")).Text = Velocity.ToString();
        ((Label)GetParent().GetNode("%EnemyLabel")).Text = GetParent()
            .GetNode<Enemy>("Enemy")
            .BlockHeight.ToString();
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
            ProcessAttack(Height.MID);
        }
        else if (Input.IsActionJustPressed("low_attack"))
        {
            ProcessAttack(Height.LOW);
        }
        UpdateDebugPanel();
        MoveAndSlide();
        if (!IsOnFloor() && (tween == null || !tween.IsRunning()))
        {
            Velocity = Velocity with { Y = Math.Min(3000, Velocity.Y + 10) };
        }
    }

    void ProcessAttack(Height level)
    {
        if (state == State.STARTUP)
        {
            if (!CanFollowUp || level == setupHeight)
            {
                return;
            }

            tween!.Stop();
            state = State.ATTACKING;
            AttackHeight = level;
            FollowUps[(int)setupHeight, (int)AttackHeight]();
        }
        else if ((state == State.IDLE || CanDoStartup) && IsOnFloor())
        {
            Reset();
            tween?.Stop();
            setupHeight = level;
            AttackHeight = Height.NONE;
            state = State.STARTUP;
            SetUps[(int)setupHeight]();
        }
    }

    void MidSetUp()
    {
        CanFollowUp = false;

        // moving forward
        tween = CreateTween();

        // starts animation
        tween.Call(() =>
        {
            Sprite.Frame = 18;
            UpdateCollisionBox(CollisionBoxes.BoxType.HURTBOX, Move.M);
        });

        tween.VelocityMovement(
            this,
            new Vector2(Position.X - 300, Position.Y),
            FramesToSeconds(24)
        );
        tween.Call(() =>
        {
            GD.Print("actionable");
            CanFollowUp = true;
            Sprite.Frame = 18;
        });
        tween.Call(Reset, FramesToSeconds(12));
    }

    void HighSetUp()
    {
        CanFollowUp = false;

        tween = CreateTween();

        // starts animation
        tween.Call(() =>
        {
            Sprite.Frame = 14;
            UpdateCollisionBox(CollisionBoxes.BoxType.HURTBOX, Move.M);
        });

        tween.VelocityMovement(
            this,
            new Vector2(Position.X + 300, Position.Y),
            FramesToSeconds(24)
        );
        tween.Call(() => Sprite.Frame = 15, FramesToSeconds(12), true);
        tween.Call(() =>
        {
            CanFollowUp = true;
            Sprite.Frame = 13;
        });
        tween.Call(Reset, FramesToSeconds(12));
    }

    void LowSetUp()
    {
        CanFollowUp = false;

        tween = CreateTween();

        // starts animation
        tween.Call(() =>
        {
            Sprite.Frame = 17;
            UpdateCollisionBox(CollisionBoxes.BoxType.HURTBOX, Move.M);
        });

        tween.Call(() =>
        {
            Velocity = new(Velocity.X + 1000, Velocity.Y - 1000);
        });
        tween.TweenProperty(this, "velocity", new Vector2(Velocity.X, 0), FramesToSeconds(24));
        tween.Call(() => Sprite.Frame = 7, FramesToSeconds(20), true);
        tween.Call(() =>
        {
            CanFollowUp = true;
        });
        tween.TweenProperty(
            this,
            "velocity",
            new Vector2(Velocity.X, Velocity.Y + 300),
            FramesToSeconds(30)
        );

        tween.Call(() =>
        {
            Sprite.Frame = 13;
            CanFollowUp = false;
        });
        tween.TweenInterval(FramesToSeconds(24));
        tween.Call(Reset);
    }

    void MidHighFollowUp()
    {
        tweening = true;
        CanFollowUp = false;
        tween = CreateTween();
        tween.Call(() =>
        {
            UpdateCollisionBox(CollisionBoxes.BoxType.HITBOX, Move.MH);
            Sprite.Frame = 9;
        });
        tween.VelocityMovement(this, new Vector2(Position.X + 20, Position.Y), FramesToSeconds(24));
        tween.Call(Reset);
    }

    void MidLowFollowUp()
    {
        tweening = true;
        CanFollowUp = false;
        tween = CreateTween();
        tween.Call(() =>
        {
            UpdateCollisionBox(CollisionBoxes.BoxType.HITBOX, Move.ML);
            Sprite.Frame = 11;
        });
        tween.VelocityMovement(this, new Vector2(Position.X + 20, Position.Y), FramesToSeconds(24));

        tween.Call(Reset);
    }

    void LowHighFollowUp()
    {
        tweening = true;
        CanFollowUp = false;
        tween = CreateTween();
        tween.Call(() =>
        {
            UpdateCollisionBox(CollisionBoxes.BoxType.HITBOX, Move.LH);
            Sprite.Frame = 8;
        });
        tween.VelocityMovement(this, new Vector2(Position.X + 20, Position.Y), FramesToSeconds(24));
        tween.Call(Reset);
    }

    void LowMidFollowUp()
    {
        tweening = true;
        CanFollowUp = false;
        tween = CreateTween();
        tween.Call(() =>
        {
            UpdateCollisionBox(CollisionBoxes.BoxType.HITBOX, Move.LM);
            Sprite.Frame = 6;
        });
        tween.VelocityMovement(this, new Vector2(Position.X - 20, Position.Y), FramesToSeconds(24));
        //TODO maybe this instead?
        // tween.TweenProperty(
        //     this,
        //     "velocity",
        //     GetMovementVelocity(Position, new(Position.X + 20, Position.Y), FramesToSeconds(24)),
        //     FramesToSeconds(24)
        // );
        tween.Call(Reset);
    }

    void HighMidFollowUp()
    {
        tweening = true;
        CanFollowUp = false;
        tween = CreateTween();
        tween.Call(() =>
        {
            UpdateCollisionBox(CollisionBoxes.BoxType.HITBOX, Move.HM);
            Sprite.Frame = 1;
        });
        tween.VelocityMovement(this, new Vector2(Position.X + 20, Position.Y), FramesToSeconds(24));
        tween.Call(Reset);
    }

    void HighLowFollowUp()
    {
        tweening = true;
        CanFollowUp = false;
        tween = CreateTween();
        tween.Call(() =>
        {
            UpdateCollisionBox(CollisionBoxes.BoxType.HITBOX, Move.HL);
            Sprite.Frame = 2;
        });
        tween.VelocityMovement(this, new Vector2(Position.X + 20, Position.Y), FramesToSeconds(24));
        tween.Call(Reset);
    }

    void Reset()
    {
        CanFollowUp = false;
        state = State.IDLE;
        ResetCollisionBox(CollisionBoxes.BoxType.HITBOX);
        UpdateCollisionBox(CollisionBoxes.BoxType.HURTBOX, Move.IDLE);
        Sprite.Frame = 4;
        if (IsOnFloor())
        {
            Velocity = Vector2.Zero;
        }
        wasBlocked = false;
        CanDoStartup = false;
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
