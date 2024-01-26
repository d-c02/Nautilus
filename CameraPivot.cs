using Godot;
using System;
using static Godot.TextServer;

public partial class CameraPivot : Node3D
{

    private Vector3 _targetRotation;

    [Export]
    public float RotationSpeed { get; set; } = 0.01f;

    [Export]
    public Camera3D _Camera;

    [Export]
    private Camera3D _TransitionCamera;

    private bool _Transitioning = false;

    private double _TransitionTime = 0.0;

    private Camera3D _NextCamera;

    [Export]
    private double _AnimationLerpSpeed = 1.5;

    //private Vector3 _TransitionPos;

    //private Vector3 _TransitionRot;

    private Transform3D _TransitionTransform;

    private Vector2 _PanDir;

    private enum CameraModes {FreeLook, Fixed, OnTrack};

    private int _CurMode = (int) CameraModes.FreeLook;

    private Vector3 _CurZoneMovementVector;

    private Vector3 _PrevMovementVector;

    private float _JoystickDeadzone = 0.1f;

    private int _CurZones = 0;

    private CameraZone _NextZone;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        _targetRotation = Rotation;
        _Camera.MakeCurrent();
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        _PanDir = Vector2.Zero;
        if (_CurMode == (int) CameraModes.FreeLook && !_Transitioning)
        {
            if (Input.IsActionPressed("tilt_camera_right"))
            {
                _PanDir.Y += 1.0f * Input.GetActionStrength("tilt_camera_right");//times GetActionStrength("tilt_camera_right")
            }
            if (Input.IsActionPressed("tilt_camera_left"))
            {
                _PanDir.Y -= 1.0f * Input.GetActionStrength("tilt_camera_left");
            }
            if (Input.IsActionPressed("tilt_camera_up"))
            {
                _PanDir.X -= 1.0f * Input.GetActionStrength("tilt_camera_up");
            }
            if (Input.IsActionPressed("tilt_camera_down"))
            {
                _PanDir.X += 1.0f * Input.GetActionStrength("tilt_camera_down");
            }
            if (_PanDir != Vector2.Zero)
            {
                //direction = direction.Normalized();
            }
            _targetRotation.X = _targetRotation.X + _PanDir.X * RotationSpeed;
            _targetRotation.Y = _targetRotation.Y + _PanDir.Y * RotationSpeed;
        }
    }


    //TODO: All camera stuff needs to be moved to _Process. However this causes visual bugs.
    public override void _PhysicsProcess(double delta)
    {
        Rotation = _targetRotation;

        if (_Transitioning)
        {
            _TransitionTime += delta * _AnimationLerpSpeed;
            //_TransitionCamera.GlobalPosition = _TransitionPos.Lerp(_NextCamera.GlobalPosition, (float)_TransitionTime);
            //_TransitionCamera.GlobalRotation = _TransitionRot.Lerp(_NextCamera.GlobalRotation, (float)_TransitionTime);
            _TransitionCamera.GlobalTransform = _TransitionTransform.InterpolateWith(_NextCamera.GlobalTransform, (float)_TransitionTime);
            if (_TransitionTime >= 1.0)
            {
                _Transitioning = false;
                _NextCamera.MakeCurrent();
                _TransitionTime = 0.0;
            }
        }
    }
    public Vector3 GetMovementVector(Vector3 playerPosition)
    {
        if (!_Transitioning)
        {
            if (_CurMode == (int)CameraModes.FreeLook)
            {
                _PrevMovementVector = playerPosition - _Camera.GlobalPosition;
                return playerPosition - _Camera.GlobalPosition;
            }
            else if (_CurMode == (int)CameraModes.Fixed)
            {
                if (!IsPlayerMoving())
                {
                    _PrevMovementVector = _CurZoneMovementVector;
                    return _CurZoneMovementVector;
                }
                else
                {
                    return _PrevMovementVector;
                }
            }
            else
            {
                return Vector3.Zero; //Placeholder, remember to change as this will probably cause bugs.
            }
        }
        else
        {
            return _PrevMovementVector;
        }
    }

    private bool IsPlayerMoving()
    {
        float right = Input.GetActionStrength("move_right");
        float left = Input.GetActionStrength("move_left");
        float back = Input.GetActionStrength("move_back");
        float forward = Input.GetActionStrength("move_forward");
        return (right > _JoystickDeadzone || left > _JoystickDeadzone || back > _JoystickDeadzone || forward > _JoystickDeadzone);
    }

    private void CameraZoneEntered(CameraZone zone)
    {
        //_TransitionPos = GetViewport().GetCamera3D().GlobalPosition;
        //_TransitionRot = GetViewport().GetCamera3D().GlobalRotation;
        _NextZone = zone;
        if (_CurZones < 1)
        {
            _TransitionTransform = GetViewport().GetCamera3D().GlobalTransform;
            _TransitionCamera.GlobalTransform = _TransitionTransform;
            _TransitionCamera.MakeCurrent();
            _TransitionTime = 0;
            _Transitioning = true;
            _CurZoneMovementVector = zone._MovementVector;
            _CurMode = (int)CameraModes.Fixed;
            _NextCamera = zone.Camera;
        }
        
        _CurZones++;
    }

    private void CameraZoneExit(CameraZone zone)
    {
        //_TransitionPos = GetViewport().GetCamera3D().GlobalPosition;
        //_TransitionRot = GetViewport().GetCamera3D().GlobalRotation;
        if (_CurZones <= 1)
        {
            _TransitionTransform = GetViewport().GetCamera3D().GlobalTransform;
            //_TransitionCamera.GlobalPosition = _TransitionPos;
            //_TransitionCamera.GlobalRotation = _TransitionRot;
            _TransitionCamera.GlobalTransform = _TransitionTransform;
            _TransitionCamera.MakeCurrent();
            _NextCamera = _Camera;
            _TransitionTime = 0;
            _Transitioning = true;
            _CurMode = (int)CameraModes.FreeLook;
        }
        else
        {
            if (_NextZone != zone)
            {
                _TransitionTransform = GetViewport().GetCamera3D().GlobalTransform;
                _TransitionCamera.GlobalTransform = _TransitionTransform;
                _TransitionCamera.MakeCurrent();
                _CurZoneMovementVector = _NextZone._MovementVector;
                _NextCamera = _NextZone.Camera;
                _TransitionTime = 0;
                _Transitioning = true;
                _CurMode = (int)CameraModes.Fixed;
            }
        }
        _CurZones--;
    }
}
