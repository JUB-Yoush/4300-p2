using System;
using Godot;

// should go in dedicated extension method definition file
public static class Helpers
{
    [Flags]
    public enum CollisionLayer
    {
        PLAYER_HIT = 0b1,
        PLAYER_HURT = 0b10,
        ENEMY_HIT = 0b100,
        ENEMY_HURT = 0b1000,
    }

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

    public static Tween MakeMovementTween(this Tween tween, Node2D node, Vector2 newPos, float time)
    {
        var newTween = node.CreateTween();
        newTween.TweenProperty(node, "position", newPos, time);
        tween.Parallel().TweenSubtween(newTween);
        return newTween;
    }

    public static float FramesToSeconds(int frames)
    {
        return (float)frames / 60;
    }

    public static Node blah()
    {
        return new Node();
    }
}
