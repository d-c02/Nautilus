using Godot;
using Godot.NativeInterop;
using System;

public partial class PlayerFalling : State
{

    [Export] private CharacterBody3D _Player;

    private bool floatFall = true;

    [Export]
    public int IdleSpeed { get; set; } = 14;


    [Export]
    public int IdleAcceleration { get; set; } = 50;

    [Export]
    public int FallAcceleration { get; set; } = 70;

    private Vector3 _targetVelocity;

    public override void Enter()
    {
        _targetVelocity = _Player.Velocity;
        if (!Input.IsActionPressed("jump"))
        {
            floatFall = false;
        }
        else
        {
            floatFall = true;
        }
    }

    public override void Exit()
    {

    }

    public override void Update(double delta)
    {
        if (!Input.IsActionPressed("jump"))
        {
            floatFall = false;
        }
    }

    public override void PhysicsUpdate(double delta)
    {
        var direction = Vector3.Zero;

        if (Input.IsActionPressed("move_right"))
        {
            direction.X += 1.0f; //times GetActionStrength("move_right")
        }
        if (Input.IsActionPressed("move_left"))
        {
            direction.X -= 1.0f;
        }
        if (Input.IsActionPressed("move_back"))
        {
            direction.Z += 1.0f;
        }
        if (Input.IsActionPressed("move_forward"))
        {
            direction.Z -= 1.0f;
        }
        if (direction != Vector3.Zero)
        {
            direction = direction.Normalized();
        }

        //_targetVelocity.X = direction.X * IdleSpeed;
        //_targetVelocity.Z = direction.Z * IdleSpeed;

        Vector2 groundVelocity = new Vector2(direction.X * IdleSpeed, direction.Z * IdleSpeed);
        Vector2 playerVelocity = new Vector2(_Player.Velocity.X, _Player.Velocity.Z);
        if (Math.Abs(playerVelocity.Length() - groundVelocity.Length()) > 1)
        {
            if (playerVelocity.Length() > groundVelocity.Length())
            {
                Vector2 decelDirection = playerVelocity.Normalized();
                _targetVelocity.X = _Player.Velocity.X - (float)(IdleAcceleration * delta * decelDirection.X);

                //decelDirection.Y isn't an error, because it's equal to _Player.Velocity.Z.
                _targetVelocity.Z = _Player.Velocity.Z - (float)(IdleAcceleration * delta * decelDirection.Y);
            }
            else if (playerVelocity.Length() < groundVelocity.Length())
            {
                _targetVelocity.X = _Player.Velocity.X + (float)(IdleAcceleration * delta * direction.X);
                _targetVelocity.Z = _Player.Velocity.Z + (float)(IdleAcceleration * delta * direction.Z);
            }
        }

        float tmpAccel = FallAcceleration;
        if (!floatFall)
        {
            tmpAccel = FallAcceleration * 2;
        }
        _targetVelocity.Y = _targetVelocity.Y - (float)(tmpAccel * delta);

        if (_Player.IsOnFloor())
        {
            _targetVelocity.Y = 0;
        }

        _Player.Velocity = _targetVelocity;
        if (_Player.IsOnFloor())
        {
            EmitSignal(SignalName.Transitioned, this.Name + "", "Grounded");
        }
    }
}
