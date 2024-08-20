using GMTK2024.scenes.hud;
using GMTK2024.scenes.vertex;
using Godot;

namespace GMTK2024.scenes;

public partial class Main : Node2D {
    [Signal]
    public delegate void OnTickEventHandler();

    private const float TickTime = 1f;
    public static Main MainNode = null!;
    public static hud.Hud HudNode = null!;
    public static Command CommandNode = null!;
    private float _remainingTickTime;

    public override void _Ready() {
        GD.Randomize();
        MainNode = this;
    }

    private void Tick() {
        foreach (var v in Vertex.FocusOrder) {
            v.SendResources();
        }

        foreach (var v in Vertex.FocusOrder) {
            v.ProcessResources();
        }

        HudNode.UpdateHud();
    }

    public override void _PhysicsProcess(double delta) {
        this._remainingTickTime += (float) Timebar.CurTime * (float) delta;
        if (this._remainingTickTime > TickTime) {
            if (this._remainingTickTime > 2 * TickTime) {
                GD.PrintErr("Running over 1 tick behind!");
            }

            this._remainingTickTime = Mathf.Min(
                TickTime,
                this._remainingTickTime - TickTime
            );
            this.Tick();
        }
    }
}