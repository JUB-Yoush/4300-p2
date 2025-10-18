using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using static CollisionBoxes;
using static Helpers;
using static Player;

public partial class Enemy : CharacterBody2D
{
    /*
     * if not attacking or being hit, blocking
     * have re-occurning timer to pick a random attack height and attack from that height
     */
    public enum State
    {
        BLOCKING,
        HIT,
        ATTACKING,
    }

    public enum Move
    {
        IDLE = -1,
        L = 0,
        M = 1,
        H = 2,
    }

    public Height BlockHeight = Height.HIGH;
    public Height AttackHeight = Height.NONE;
    public int[] AttackSpeedRange = [1, 4];
    public State CurrentState = State.BLOCKING;
    Timer attackTimer = null!;
    Tween tween = null!;

    AudioStream CurrentAttackSFX = null!;

    Sprite2D Sprite = null!;

    Area2D HitboxArea = null!;
    Area2D HurtboxArea = null!;

    public int Hp = 100;

    Action[] Attacks = [];

    AnimationPlayer AnimPlayer = null!;

    public Dictionary<Move, AttackData> AttackDataMap = new()
    {
        { Move.L, new(5, 100, 15) },
        { Move.M, new(5, 100, 15) },
        { Move.H, new(5, 100, 15) },
    };
    public Move currentMove;
    GameCamera Cam = null!;

    public override void _Ready()
    {
        Cam = GetParent().GetNode<GameCamera>("GameCamera");
        AnimPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        attackTimer = new();
        AddChild(attackTimer);
        attackTimer.OneShot = true;
        attackTimer.Start(GetAttackSpeed());
        attackTimer.Timeout += Attack;

        Attacks = [LowAttack, MidAttack, HighAttack];

        HurtboxArea = GetNode<Area2D>("HurtboxArea");
        // HurtboxArea.CollisionLayer = (uint)Collisions.ENEMY_HURT;
        // HurtboxArea.CollisionMask = (uint)Collisions.PLAYER_HIT;
        //HurtboxArea.AreaEntered += HitByEnemy;

        HitboxArea = GetNode<Area2D>("HitboxArea");
        // HitboxArea.CollisionLayer = (uint)Collisions.ENEMY_HIT;
        // HitboxArea.CollisionMask = (uint)Collisions.ENEMY_HURT;
        //HitboxArea.AreaEntered += HitEnemy;
        Sprite = GetNode<Sprite2D>("Sprite2D");
        ChangeBlockHeight();
    }

    float GetAttackSpeed() => new Random().Next(AttackSpeedRange[0], AttackSpeedRange[1] + 1);

    void Attack()
    {
        if (CurrentState == State.ATTACKING)
        {
            return;
        }
        CurrentState = State.ATTACKING;
        AttackHeight = BlockHeight;
        GD.Print($"Attacking {AttackHeight}");
        Attacks[(int)AttackHeight]();
    }

    void ChangeToBlocking()
    {
        CurrentState = State.BLOCKING;
        ClearSpikes();
        ChangeBlockHeight();
        attackTimer.Start(GetAttackSpeed());
    }

    void LowAttack()
    {
        tween = CreateTween();
        tween.SetTrans(Tween.TransitionType.Quad);
        tween.Call(() => Sprite.Frame = 0);
        // tween.TweenProperty(
        //     this,
        //     "position",
        //     new Vector2(Position.X + 10, Position.Y),
        //     FramesToSeconds(10)
        // );
        // tween.TweenProperty(
        //     this,
        //     "position",
        //     new Vector2(Position.X - 10, Position.Y),
        //     FramesToSeconds(10)
        // );
        // tween.TweenProperty(
        //     this,
        //     "position",
        //     new Vector2(Position.X, Position.Y + 10),
        //     FramesToSeconds(10)
        // );
        SetAttackSFX(SFX.KaijuLow);
        tween.TweenInterval(FramesToSeconds(10));

        tween.TweenInterval(FramesToSeconds(20));
        tween.Call(() => Sprite.Frame = 1);
        tween.Call(() =>
        {
            UpdateCollisionBox(BoxType.HITBOX, Move.L);
            SpawnSpike(new(GlobalPosition.X - 600, GlobalPosition.Y + 300));
        });
        tween.TweenInterval(FramesToSeconds(5));
        tween.Call(() => SpawnSpike(new(GlobalPosition.X - 900, GlobalPosition.Y + 300)));
        tween.TweenInterval(FramesToSeconds(5));
        tween.Call(() => SpawnSpike(new(GlobalPosition.X - 1200, GlobalPosition.Y + 300)));
        tween.TweenInterval(FramesToSeconds(5));
        tween.Call(() => SpawnSpike(new(GlobalPosition.X - 1500, GlobalPosition.Y + 300)));
        tween.TweenInterval(FramesToSeconds(30));
        tween.Call(() =>
        {
            ResetCollisionBox(BoxType.HITBOX);
            ChangeToBlocking();
        });
        tween.Call(() =>
        {
            var Spikes = GetNode<Node2D>("Spikes");
            for (int i = 0; i < Spikes.GetChildCount(); i++)
            {
                Spikes.GetChild(i).QueueFree();
            }
        });
    }

    void MidAttack()
    {
        tween = CreateTween();
        SetAttackSFX(SFX.KaijuMid);
        tween.SetTrans(Tween.TransitionType.Quad);
        tween.Call(() => Sprite.Frame = 3);
        tween.TweenInterval(FramesToSeconds(30));
        tween.Call(() => Sprite.Frame = 4);
        tween.Call(() =>
        {
            UpdateCollisionBox(BoxType.HITBOX, Move.M);
        });
        tween.TweenProperty(Sprite, "scale", new Vector2(1.8f, .75f), FramesToSeconds(5));
        tween
            .Parallel()
            .TweenProperty(
                this,
                "position",
                new Vector2(Position.X - 150, Position.Y),
                FramesToSeconds(5)
            );
        tween.TweenProperty(Sprite, "scale", new Vector2(.75f, .75f), FramesToSeconds(30));
        tween.Call(() =>
        {
            ResetCollisionBox(BoxType.HITBOX);
            ChangeToBlocking();
        });
    }

    void ClearSpikes()
    {
        var Spikes = GetNode<Node2D>("Spikes");
        for (int i = 0; i < Spikes.GetChildCount(); i++)
        {
            Spikes.GetChild(i).QueueFree();
        }
    }

    void HighAttack()
    {
        tween = CreateTween();
        SetAttackSFX(SFX.KaijuHigh);
        tween.SetTrans(Tween.TransitionType.Sine);
        tween.Call(() => Sprite.Frame = 16);
        tween.TweenProperty(Sprite, "scale", new Vector2(.75f, 1f), FramesToSeconds(30));
        tween.TweenProperty(Sprite, "scale", new Vector2(.75f, .75f), FramesToSeconds(5));
        tween.Call(() => Sprite.Frame = 17);
        tween.Call(() =>
        {
            UpdateCollisionBox(BoxType.HITBOX, Move.H);
            GetNode<GpuParticles2D>("EnemyLaser").Emitting = true;
            GetNode<GpuParticles2D>("EnemyLaser/GPUParticles2D").Emitting = true;
            GetNode<GpuParticles2D>("EnemyLaser/GPUParticles2D2").Emitting = true;
        });
        tween.TweenProperty(Sprite, "scale", new Vector2(1f, .75f), FramesToSeconds(30));
        tween.Call(() =>
        {
            ResetCollisionBox(BoxType.HITBOX);
            ChangeToBlocking();
        });
        tween.TweenProperty(Sprite, "scale", new Vector2(.75f, .75f), FramesToSeconds(5));
    }

    void SpawnSpike(Vector2 spawnPosition)
    {
        Spike spike = Spike.PackedScene.Instantiate<Spike>();
        spike.TopLevel = true;
        spike.GlobalPosition = spawnPosition;
        GetNode<Node2D>("Spikes").AddChild(spike);
    }

    public override void _PhysicsProcess(double delta)
    {
        MoveAndSlide();
    }

    public void ChangeBlockHeight()
    {
        var blockFrameMap = new Dictionary<Height, int>()
        {
            { Height.HIGH, 5 },
            { Height.MID, 9 },
            { Height.LOW, 7 },
        };

        var newBlockHeight = (Height)new Random().Next(0, 3);
        // while (newBlockHeight == BlockHeight)
        // {
        //     newBlockHeight = (Height)new Random().Next(0, 3);
        // }
        BlockHeight = newBlockHeight;
        //BlockHeight = Height.LOW;
        Sprite.Frame = blockFrameMap[BlockHeight];
    }

    public void GotHit(int hitstun, int damage, int knockback)
    {
        GD.Print(hitstun, damage, knockback);
        Sprite.Scale = new(.75f, .75f);
        tween?.Stop();
        tween = CreateTween();
        tween.Call(() =>
        {
            CurrentState = State.HIT;
            Sprite.Frame = 11;
            Hp = Math.Max(0, Hp - damage);
            AudioManager.StopSfx(CurrentAttackSFX);
            AudioManager.PlaySfx(SFX.KaijuDamage);
            Cam.SetScreenShake(5, 3f);
            Hitstop(0.05f, 100);
            AnimPlayer.Play("hitflash");
        });
        tween.VelocityMovement(
            this,
            new(Position.X + knockback, Position.Y),
            FramesToSeconds(hitstun)
        );
        tween.Call(() =>
        {
            ResetCollisionBox(BoxType.HITBOX);
            Sprite.Frame = 12;
            ChangeToBlocking();
        });
    }

    public void Blocked()
    {
        //do an attack
        tween?.Stop();
        AnimPlayer.Play("block");
        Hitstop(0.05f, 100);
        if (CurrentState != State.ATTACKING)
        {
            Attack();
        }
    }

    void UpdateCollisionBox(BoxType boxtype, Move move)
    {
        ResetCollisionBox(boxtype);

        var area = boxtype == BoxType.HITBOX ? HitboxArea : HurtboxArea;
        if (boxtype == BoxType.HITBOX)
        {
            currentMove = move;
        }
        area.GetNode<CollisionShape2D>(move.ToString()).Disabled = false;
    }

    void ResetCollisionBox(BoxType boxtype)
    {
        var area = boxtype == BoxType.HITBOX ? HitboxArea : HurtboxArea;
        area.GetChildren()
            .OfType<CollisionShape2D>()
            .ToList()
            .ForEach(child => child.Disabled = true);
    }

    void SetAttackSFX(AudioStream sfx)
    {
        CurrentAttackSFX = sfx;
        AudioManager.PlaySfx(sfx);
    }
}
