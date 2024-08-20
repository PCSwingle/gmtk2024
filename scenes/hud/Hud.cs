using GMTK2024.scripts;
using Godot;

namespace GMTK2024.scenes.hud;

public partial class Hud : Control {
    [Signal]
    public delegate void OnUpdateHudEventHandler();

    public override void _Ready() {
        Main.HudNode = this;
    }

    public void UpdateHud() {
        this.GetNode<Label>("CoinLabel").Text = (Game.State.Coins / 1000).ToString("0.0") + "k";
        this.EmitSignal(SignalName.OnUpdateHud);
    }
}