using Godot;

namespace GMTK2024.scenes.hud;

public enum Time {
    Paused = 0,
    OneX = 1,
    TwoX = 2,
    ThreeX = 3,
    FourX = 4,
    FiveX = 5
}

public partial class Timebar : Control {
    [Signal]
    public delegate void OnTimeChangeEventHandler(Time newTime);

    public static Time CurTime = Time.Paused;

    private Time _prevTime = Time.Paused;

    public override void _Ready() {
        var startingTime = OS.IsDebugBuild() ? Time.FiveX : Time.OneX;
        this._ChangeTime(startingTime);
    }

    private void _ChangeTime(Time newTime) {
        this._prevTime = CurTime;
        CurTime = newTime;
        this.GetNode<Sprite2D>("TimebarSprite").Frame = (int) newTime;
        this.EmitSignal(
            SignalName.OnTimeChange,
            (int) newTime
        );
    }

    public override void _UnhandledInput(InputEvent e) {
        if (e.IsActionPressed("Pause")) {
            if (CurTime == Time.Paused) {
                this._ChangeTime(this._prevTime);
            } else {
                this._ChangeTime(Time.Paused);
            }
        }

        if (e.IsActionPressed("Time1x")) {
            this._ChangeTime(Time.OneX);
        }

        if (e.IsActionPressed("Time2x")) {
            this._ChangeTime(Time.TwoX);
        }

        if (e.IsActionPressed("Time3x")) {
            this._ChangeTime(Time.ThreeX);
        }

        if (e.IsActionPressed("Time4x")) {
            this._ChangeTime(Time.FourX);
        }

        if (e.IsActionPressed("Time5x")) {
            this._ChangeTime(Time.FiveX);
        }
    }

    public override void _GuiInput(InputEvent e) {
        if (e is InputEventMouseButton { Pressed: true, ButtonIndex: MouseButton.Left } me) {
            var newTime = ((int) me.Position.X - 2) / 44;
            if (newTime > 5) {
                newTime = 5;
            }

            this._ChangeTime((Time) newTime);
        }
    }
}