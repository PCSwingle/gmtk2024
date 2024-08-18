using Godot;

namespace GMTK2024.scenes;

public partial class Main : Node2D {
    [Signal]
    public delegate void OnTickEventHandler();

    private const float TickTime = 1f;
    public static Main MainNode = null!;
    private float _remainingTickTime;


    public override void _Ready() {
        MainNode = this;
    }

    private void Tick() {
        this.EmitSignal(SignalName.OnTick);
        foreach (var v in Vertex.FocusOrder) {
            v.SendResources();
        }

        foreach (var v in Vertex.FocusOrder) {
            v.ProcessResources();
        }
    }

    public override void _PhysicsProcess(double delta) {
        this._remainingTickTime += (float) Timebar.CurTime * (float) delta;
        if (this._remainingTickTime > TickTime) {
            this._remainingTickTime = Mathf.Min(TickTime, this._remainingTickTime - TickTime);
            this.Tick();
        }
    }
}