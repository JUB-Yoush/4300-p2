using System;
using Godot;
using static Helpers;
using static Player;

public partial class Enemy : Node2D
{
    public Height BlockHeight = Height.NONE;
    public Height AttackHeight = Height.NONE;

    Area2D HitboxArea = null!;
    Area2D HurtboxArea = null!;

    public override void _Ready()
    {
        HurtboxArea = GetNode<Area2D>("HurtboxArea");
        HurtboxArea.CollisionLayer = (uint)CollisionLayer.ENEMY_HURT;
        HurtboxArea.CollisionMask = (uint)CollisionLayer.PLAYER_HIT;
        //HurtboxArea.AreaEntered += HitByEnemy;

        HitboxArea = GetNode<Area2D>("HitboxArea");
        HitboxArea.CollisionLayer = (uint)CollisionLayer.ENEMY_HIT;
        HitboxArea.CollisionMask = (uint)CollisionLayer.ENEMY_HURT;
        //HitboxArea.AreaEntered += HitEnemy;
    }

    public void GotHit()
    {
        Position = Position with { X = Position.X + 20 };
    }
}
