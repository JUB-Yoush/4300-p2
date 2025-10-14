using System;
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
    public float AttackCooldown = 5f;
    public State CurrentState = State.BLOCKING;
    Timer attackTimer = null!;

    Area2D HitboxArea = null!;
    Area2D HurtboxArea = null!;

    Action[] Attacks = [];

    public override void _Ready()
    {
        attackTimer = new();
        AddChild(attackTimer);
        attackTimer.OneShot = true;
        attackTimer.Start(4);
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
        ChangeBlockHeight();
    }

    void Attack()
    {
        CurrentState = State.ATTACKING;
        AttackHeight = BlockHeight;
        GD.Print($"Attacking {AttackHeight}");
        Attacks[(int)AttackHeight]();
    }

    void LowAttack()
    {
        var tween = CreateTween();
        tween.Call(() =>
        {
            UpdateCollisionBox(BoxType.HITBOX, (Move)AttackHeight);
        });
        tween.TweenInterval(2);
        tween.Call(() =>
        {
            ResetCollisionBox(BoxType.HITBOX);
            ChangeToBlocking();
        });
    }

    void ChangeToBlocking()
    {
        CurrentState = State.BLOCKING;
        ChangeBlockHeight();
        attackTimer.Start(4);
    }

    void MidAttack()
    {
        LowAttack();
    }

    void HighAttack()
    {
        LowAttack();
    }

    public override void _PhysicsProcess(double delta)
    {
        MoveAndSlide();
    }

    public void ChangeBlockHeight()
    {
        //BlockHeight = (Height)new Random().Next(0, 3);
        BlockHeight = Height.MID;
        //GD.Print(BlockHeight);
    }

    public void GotHit()
    {
        //Position = Position with { X = Position.X + 20 };
        var tween = CreateTween();
        tween.Call(() => CurrentState = State.HIT);
        tween.VelocityMovement(this, new(Position.X + 5, Position.Y), FramesToSeconds(8));
        tween.TweenInterval(2);
        tween.Call(() =>
        {
            ResetCollisionBox(BoxType.HITBOX);
            ChangeToBlocking();
        });
    }

    public void Blocked()
    {
        var tween = CreateTween();
        tween.VelocityMovement(this, new(Position.X + 10, Position.Y), FramesToSeconds(8));
    }

    void UpdateCollisionBox(BoxType boxtype, Move move)
    {
        ResetCollisionBox(boxtype);

        var area = boxtype == BoxType.HITBOX ? HitboxArea : HurtboxArea;
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
}
