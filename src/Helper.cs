using System;
using System.Runtime.InteropServices.Marshalling;
using System.Threading.Tasks;
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
        bool parallel = true,
        bool reset = true
    )
    {
        var newTween = node.CreateTween();
        var start = node.Position;
        var dist = new Vector2(end.X - start.X, end.Y - start.Y);
        var prevVel = node.Velocity;

        newTween.Call(() => node.Velocity = dist / time);
        newTween.TweenInterval(time);
        if (reset)
        {
            newTween.Call(() => node.Velocity = prevVel);
        }
        else
        {
            newTween.Call(() => node.Velocity = Vector2.Zero);
        }

        if (parallel)
        {
            tween.Parallel().TweenSubtween(newTween);
        }
        else
        {
            tween.TweenSubtween(newTween);
        }
    }

    public static void Call(this Tween tween, Action action, float delay = 0, bool parallel = false)
    {
        if (parallel)
        {
            tween.Parallel().TweenCallback(Callable.From(action)).SetDelay(delay);
        }
        else
        {
            tween.TweenCallback(Callable.From(action)).SetDelay(delay);
        }
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
}
