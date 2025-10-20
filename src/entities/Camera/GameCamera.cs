using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Godot;

public partial class GameCamera : Camera2D
{
    private static readonly Vector2 Resolution = new(1920, 1080);
    private static readonly int[] CameraLimits = [0, 3423];
    const int SCREEN_WIDTH = 1400;
    const int CAMERA_HEIGHT = 580;

    Player player = null!;
    Enemy enemy = null!;
    Vector2 playerToEnemyDist = Vector2.Zero;

    CollisionShape2D LeftWall = null!;
    CollisionShape2D RightWall = null!;

    float ShakeStrength = 0;
    float ShakeDecay = 0;

    Vector2 CameraZoom = new Vector2(2f, 2f);
    const float CAMERA_ZOOM_DURATION = 2f;
    Tween? cameraTween = null;

    /*
     * attach static bodies to the bounds of the camera
     * calculate the distance between the player and enemy
     * if it's larger than the width of the screen (or some slightly smaller value) then
     */

    public override void _Ready()
    {
        ProcessMode = ProcessModeEnum.Always; //so when the game is over, the camera will still move
        LimitLeft = CameraLimits[0];
        LimitRight = CameraLimits[1];
        player = GetParent().GetNode<Player>("Player");
        enemy = GetParent().GetNode<Enemy>("Enemy");
        LeftWall = GetNode<CollisionShape2D>("Border/LeftWall");
        RightWall = GetNode<CollisionShape2D>("Border/RightWall");
    }

    public override void _Process(double delta)
    {
        var midpoint = (player.GlobalPosition.X + enemy.GlobalPosition.X) / 2;
        playerToEnemyDist = enemy.GlobalPosition - player.GlobalPosition;
        if (playerToEnemyDist.X >= SCREEN_WIDTH)
        {
            SetWalls(true);
        }
        else
        {
            SetWalls(false);
        }
        GlobalPosition = new(midpoint, CAMERA_HEIGHT);

        LeftWall.GlobalPosition = LeftWall.GlobalPosition with
        {
            X = Math.Max(GlobalPosition.X - Resolution.X / 2, CameraLimits[0]),
        };

        RightWall.GlobalPosition = RightWall.GlobalPosition with
        {
            X = Math.Min(GlobalPosition.X + Resolution.X / 2, CameraLimits[1]),
        };

        ShakeStrength = (float)Mathf.Lerp(ShakeStrength, 0, ShakeDecay * delta);
        Offset = GetShakeOffset();
    }

    void SetWalls(bool state)
    {
        LeftWall.Disabled = !state;
        RightWall.Disabled = !state;
    }

    public void SetScreenShake(float intensity, float decay_rate)
    {
        ShakeStrength = intensity;
        ShakeDecay = decay_rate;
    }

    Vector2 GetShakeOffset()
    {
        var rng = new Random();
        Offset = new Vector2(
            rng.Next((int)-ShakeStrength, (int)ShakeStrength),
            rng.Next((int)-ShakeStrength, (int)ShakeStrength)
        );
        return Offset;
    }

    public void ZoomIn(Vector2 location)
    {
        cameraTween = CreateTween();
        cameraTween.SetParallel();
        cameraTween.TweenProperty(this, "position", location, CAMERA_ZOOM_DURATION);
        cameraTween.TweenProperty(this, "zoom", CameraZoom, CAMERA_ZOOM_DURATION);
    }
}
