using System;
using System.Security.Authentication.ExtendedProtection;
using Godot;
using static Helpers;
using static Player;

public partial class Enemy : CharacterBody2D
{
    public enum State
    {
        BLOCKING,
        HIT,
        ATTACKING,
    }

    public Height BlockHeight = Height.HIGH;
    public Height AttackHeight = Height.NONE;
    public float AttackCooldown = 5f;
    public State CurrentState = State.BLOCKING;

    Area2D HitboxArea = null!;
    Area2D HurtboxArea = null!;

    public override void _Ready()
    {
        HurtboxArea = GetNode<Area2D>("HurtboxArea");
        HurtboxArea.CollisionLayer = (uint)Collisions.ENEMY_HURT;
        HurtboxArea.CollisionMask = (uint)Collisions.PLAYER_HIT;
        //HurtboxArea.AreaEntered += HitByEnemy;

        HitboxArea = GetNode<Area2D>("HitboxArea");
        HitboxArea.CollisionLayer = (uint)Collisions.ENEMY_HIT;
        HitboxArea.CollisionMask = (uint)Collisions.ENEMY_HURT;
        //HitboxArea.AreaEntered += HitEnemy;
        ChangeBlockHeight();
    }

    public override void _PhysicsProcess(double delta)
    {
        GD.Print(Velocity);
        MoveAndSlide();
    }

    public void ChangeBlockHeight()
    {
        //BlockHeight = (Height)new Random().Next(0, 3);
        BlockHeight = Height.MID;
    }

    public void GotHit()
    {
        //Position = Position with { X = Position.X + 20 };
    }

    public void Blocked()
    {
        var tween = CreateTween();
        tween.VelocityMovement(this, new(Position.X + 10, Position.Y), FramesToSeconds(8));
    }
}
