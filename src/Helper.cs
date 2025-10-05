using System;
using System.Runtime.InteropServices.Marshalling;
using Godot;

// should go in dedicated extension method definition file
public static class Helpers
{
    [Flags]
    public enum Collisions
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

    public static Vector2 GetMovementVelocity(Vector2 start, Vector2 end, float time)
    {
        var dist = new Vector2(end.X - start.X, end.Y - start.Y);
        GD.Print(dist);
        return dist / time;
    }

    public static void VelocityMovement(
        this Tween tween,
        CharacterBody2D node,
        Vector2 end,
        float time,
        bool parallel = true
    )
    {
        var newTween = node.CreateTween();
        var start = node.Position;
        var dist = new Vector2(end.X - start.X, end.Y - start.Y);
        var prevVel = node.Velocity;

        newTween.Call(() => node.Velocity = dist / time);
        newTween.TweenInterval(time);
        newTween.Call(() => node.Velocity = prevVel);

        GD.Print(dist);
        if (parallel)
        {
            tween.Parallel().TweenSubtween(newTween);
        }
        else
        {
            tween.TweenSubtween(newTween);
        }
    }

    public static void Call(this Tween tween, Action action)
    {
        tween.TweenCallback(Callable.From(action));
    }

    public static Tween MakeMovementTween(
        this Tween tween,
        Node2D node,
        Vector2 newPos,
        float time,
        bool parallel = true
    )
    {
        var newTween = node.CreateTween();
        newTween.TweenProperty(node, "position", newPos, time);
        if (parallel)
        {
            tween.Parallel().TweenSubtween(newTween);
        }
        else
        {
            tween.TweenSubtween(newTween);
        }
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
