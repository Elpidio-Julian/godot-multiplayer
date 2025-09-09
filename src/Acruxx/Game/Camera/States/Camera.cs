using Godot;

namespace Acruxx.Camera.States;

/// <summary>
/// The camera.
/// </summary>
public partial class Camera : CameraState
{
    [Export]
    public float DeadZoneBackwards = 0.3f;

    [Export]
    public float FovDefault = 70.0f;

    public bool IsAiming = false;

    [Export]
    public bool IsYInverted = true;

    [Export]
    public Vector2 SensivityGamePad = new(2.5f, 2.5f);

    [Export]
    public Vector2 SensivityMouse = new(0.1f, 0.1f);

    /// <summary>
    /// The zoom step.
    /// </summary>
    private const float ZOOM_STEP = 0.1f;

    private Vector2 _input_relative = Vector2.Zero;

    /// <inheritdoc/>
    public override void _Ready()
    {
        base._Ready();
    }

    /// <inheritdoc/>
    public override void Process(double delta)
    {
        //TODO: is there a way to make this better?

        var transform = this._cameraRig!.Transform;
        transform.Origin = this._cameraRig.Player!.GlobalTransform.Origin + this._cameraRig.PositionStart;
        this._cameraRig.GlobalTransform = transform;

        Vector2 lookDirection = GetLookDirection();
        Vector3 moveDirection = GetMoveDirection();

        if (this._input_relative.Length() > 0.0f)
        {
            this.UpdateRotation(this._input_relative * this.SensivityMouse * (float)delta);
            this._input_relative = Vector2.Zero;
        }

        if (lookDirection.Length() > 0.0f)
        {
            this.UpdateRotation(lookDirection * this.SensivityGamePad * (float)delta);
        }

        bool isMovingTowardsCamera =
            (moveDirection.X >= -this.DeadZoneBackwards) &&
            (moveDirection.X <= this.DeadZoneBackwards);

        if (!isMovingTowardsCamera && !this.IsAiming)
        {
            this.AutoRotate();
        }

        // prevent winding
        var rot = this._cameraRig.Rotation;
        rot.Y = Mathf.Wrap(this._cameraRig.Rotation.Y, -Mathf.Pi, Mathf.Pi);
        this._cameraRig.Rotation = rot;
    }

    /// <inheritdoc/>
    public override void UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("zoom_in"))
        {
            this._cameraRig!.Zoom += ZOOM_STEP;
        }
        else if (@event.IsActionPressed("zoom_out"))
        {
            this._cameraRig!.Zoom -= ZOOM_STEP;
        }
        else if ((@event is InputEventMouseMotion) && (Input.MouseMode == Input.MouseModeEnum.Captured))
        {
            this._input_relative += (@event as InputEventMouseMotion)!.Relative;
        }
    }

    /// <summary>
    /// Gets the look direction.
    /// </summary>
    /// <returns>A Vector2.</returns>
    private static Vector2 GetLookDirection()
    {
        return new Vector2(
            Input.GetActionStrength("look_right") - Input.GetActionStrength("look_left"),
            Input.GetActionStrength("look_up") - Input.GetActionStrength("look_down")
        );
    }

    /// <summary>
    /// Gets the move direction.
    /// </summary>
    /// <returns>A Vector3.</returns>
    private static Vector3 GetMoveDirection()
    {
        return new Vector3(
            Input.GetActionStrength("move_right") - Input.GetActionStrength("move_left"),
            0.0f,
            Input.GetActionStrength("move_back") - Input.GetActionStrength("move_front")
        );
    }

    /// <summary>
    /// Autos the rotate.
    /// </summary>
    private void AutoRotate()
    {
        float offset = this._cameraRig!.Player!.Rotation.Y - this._cameraRig.Rotation.Y;
        float targetAngle = this.CalculateTargetAngle(offset);
        var rot = this._cameraRig.Rotation;
        rot.Y = Mathf.Lerp(rot.Y, targetAngle, 0.015f);
        this._cameraRig.Rotation = rot;
    }

    /// <summary>
    /// Calculates the target angle.
    /// </summary>
    /// <param name="offset">The offset.</param>
    /// <returns>A float.</returns>
    private float CalculateTargetAngle(float offset)
    {
        return offset > Mathf.Pi
            ? this._cameraRig!.Player!.Rotation.Y - (2 * Mathf.Pi)
            : offset < -Mathf.Pi ? this._cameraRig!.Player!.Rotation.Y + (2 * Mathf.Pi) : this._cameraRig!.Player!.Rotation.Y;
    }

    /// <summary>
    /// Updates the rotation.
    /// </summary>
    /// <param name="offset">The offset.</param>
    private void UpdateRotation(Vector2 offset)
    {
        // left right rotation
        var rot = this._cameraRig!.Rotation;
        rot.Y -= offset.X;
        this._cameraRig!.Rotation = rot;

        // up down rotation
        rot.X += (this.IsYInverted ? (offset.Y * -1.0f) : offset.Y);
        this._cameraRig!.Rotation = rot;

        // limit camera rotation
        rot.X = Mathf.Clamp(rot.X, -0.75f, 1.25f);
        this._cameraRig!.Rotation = rot;

        // not z rotation
        rot.Z = 0.0f;
        this._cameraRig!.Rotation = rot;
    }
}