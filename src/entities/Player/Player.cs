using System;
using System.Diagnostics;
using Godot;

public partial class Player : Sprite2D
{
    enum State
    {
        IDLE,
        STARTUP,
        ATTACKING,
    }

    enum Height
    {
        LOW,
        MEDIUM,
        HIGH,
        NONE,
    }

    //delete when animation are in
    [Export]
    public Texture2D idle,
        startup,
        attack;
    const float STARTUP_TIME = 0.5f;
    const float HEAVY_TIME = 1f;
    const float LIGHT_TIME = 0.5f;
    const float BLOCK_TIME = 2f;
    private State state;
    private Height height,
        attackingHeight;
    private Timer StartupTimer;
    private Timer AttackTimer;
    private Timer BlockTimer;

    public override void _Ready()
    {
        StartupTimer = new Timer();
        AttackTimer = new Timer();
        BlockTimer = new Timer();
        AddChild(StartupTimer);
        AddChild(AttackTimer);
        AddChild(BlockTimer);
        StartupTimer.Timeout += ProcessAttack;
        AttackTimer.Timeout += ResetToIdle;

        state = State.IDLE;
        height = Height.NONE;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (Input.IsActionJustPressed("High Attack"))
            ProcessAttack(Height.HIGH);
        if (Input.IsActionJustPressed("Medium Attack"))
            ProcessAttack(Height.MEDIUM);
        if (Input.IsActionJustPressed("Low Attack"))
            ProcessAttack(Height.LOW);
    }

    private void ProcessAttack(Height level)
    {
        if (state == State.IDLE)
            TriggerStartup(level);
        else if (state == State.STARTUP)
            attackingHeight = level;
    }

    private void TriggerStartup(Height level)
    {
        Texture = startup;
        state = State.STARTUP;
        height = level;
        StartupTimer.Start(STARTUP_TIME);
        attackingHeight = Height.NONE;
        GD.Print("Startup on lane: " + height.ToString());
    }

    private void ProcessAttack()
    {
        StartupTimer.Stop();
        float attackDelay = HEAVY_TIME;
        //kid named if
        if (attackingHeight == height || attackingHeight == Height.NONE)
        {
            ResetToIdle();
            return;
        }
        else if (height == Height.HIGH)
        {
            if (attackingHeight == Height.MEDIUM)
            {
                //crouching light
                attackDelay = LIGHT_TIME;
            }
            else if (attackingHeight == Height.LOW) { } //crouching heavy

        }
        else if (height == Height.MEDIUM)
        {
            if (attackingHeight == Height.HIGH) { } //dandy heavy

            else if (attackingHeight == Height.LOW)
            {
                //dandy light
                attackDelay = LIGHT_TIME;
            }
        }
        else //height == Height.LOW
        {
            if (attackingHeight == Height.HIGH) { } //jumping heavy

            else if (attackingHeight == Height.MEDIUM)
            {
                //jumping light
                attackDelay = LIGHT_TIME;
            }
        }

        Texture = attack; //remove once specific attacks are in
        AttackTimer.Start(attackDelay);
        state = State.ATTACKING;
        GD.Print("Attacking on lane: " + height.ToString());
    }

    private void ResetToIdle()
    {
        StartupTimer.Stop();
        AttackTimer.Stop();
        Texture = idle;
        state = State.IDLE;
        height = Height.NONE;
    }
}
