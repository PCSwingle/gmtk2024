using GMTK2024.scripts;
using Godot;

namespace GMTK2024.scenes;

public partial class Hud : Control {
    [Signal]
    public delegate void OnUpdateHudEventHandler();

    public override void _Ready() {
        Main.HudNode = this;
    }

    public void UpdateHud() {
        this.GetNode<Label>("CoinLabel").Text = Game.State.Coins.ToString();
        this.EmitSignal(SignalName.OnUpdateHud);
    }
}