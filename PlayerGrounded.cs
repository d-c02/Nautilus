using Godot;
using System;

public partial class PlayerGrounded : State
{
    [Export] private player _Player;

    [Export]
    public int IdleSpeed { get; set; } = 14;

    [Export]
    public int IdleAcceleration { get; set; } = 75;

    [Export]
    public int JumpSpeed { get; set; } = 20;

    //[Export]
    //private Camera3D _Camera;

    [Export]
    private CameraPivot _Camera;

    [Export]
    public double RollDelay { get; set; } = 0.8; //How long you need to wait between rolls

    [Export]
    public double AirDelay { get; set; } = 0.1; //How long you need to be in the air before going into falling state

    private double _CurAirTime;
    private double _CurRollTime;

    private Vector3 _targetVelocity;


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {

    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    // Called when the node enters the scene tree for the first time.
    public override void PhysicsUpdate(double delta)
    {
        var direction = Vector3.Zero;
        Vector3 cameraDifferenceVector = _Camera.GetMovementVector(_Player.GlobalPosition);
        cameraDifferenceVector.Y = 0;    
        cameraDifferenceVector = cameraDifferenceVector.Normalized();
        cameraDifferenceVector.Y = 0;
        Vector3 orthogonalCameraDifferenceVector = (new Vector3(-1 * cameraDifferenceVector.Z, 0, cameraDifferenceVector.X)).Normalized();

        if (Input.IsActionPressed("move_right"))
        {
            direction += orthogonalCameraDifferenceVector * Input.GetActionStrength("move_right"); //times GetActionStrength("move_right")
        }
        if (Input.IsActionPressed("move_left"))
        {
            direction -= orthogonalCameraDifferenceVector * Input.GetActionStrength("move_left");
        }
        if (Input.IsActionPressed("move_back"))
        {
            direction -= cameraDifferenceVector * Input.GetActionStrength("move_back");
        }
        if (Input.IsActionPressed("move_forward"))
        {
            direction += cameraDifferenceVector * Input.GetActionStrength("move_forward");
        }
        if (Input.IsActionJustPressed("jump"))
        {
            _targetVelocity.Y = JumpSpeed;
        }
        if (direction.Length() < 0.1)
        {
            direction = Vector3.Zero;
            _Player.SetAnimState("Idle");
        }
        else if (direction != Vector3.Zero)
        {
            _Player.SetAnimState("Run");
            if (direction.Length() > 1)
            {
                direction = direction.Normalized();
            }
            _Player.GetNode<Node3D>("Pivot").LookAt(_Player.Position + direction, Vector3.Up);
        }


        _targetVelocity.X = direction.X * IdleSpeed;
        _targetVelocity.Z = direction.Z * IdleSpeed;

        Vector2 groundVelocity = new Vector2(_targetVelocity.X, _targetVelocity.Z);
        Vector2 playerVelocity = new Vector2(_Player.Velocity.X, _Player.Velocity.Z);
        if (Math.Abs(playerVelocity.Length() - groundVelocity.Length()) > 10)
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

        if (!_Player.IsOnFloor() || Input.IsActionJustPressed("jump"))
        {
            _CurAirTime += delta;
            //Snap to floor if floor is nearby
        }
        else
        {
            _CurAirTime = 0;
            _targetVelocity.Y = 0;
        }

        _Player.SetRunTimeScale(new Vector3(_targetVelocity.X, 0, _targetVelocity.Z).Length());
        _Player.Velocity = _targetVelocity;
        _CurRollTime += delta;

        if (Input.IsActionJustPressed("roll") && _CurRollTime >= RollDelay)
        {
            _CurRollTime = 0;
            EmitSignal(SignalName.Transitioned, this.Name + "", "Rolling");
        }

        if (_CurAirTime > AirDelay || Input.IsActionJustPressed("jump"))
        {
            _CurRollTime = RollDelay;
            EmitSignal(SignalName.Transitioned, this.Name + "", "Falling");
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void Update(double delta)
    {
    }

    public override void Enter()
    {
        _targetVelocity = _Player.Velocity;
        _targetVelocity.Y = 0;
        _CurAirTime = 0;
    }

    public override void Exit()
    {
        _CurAirTime = 0;
    }
}
