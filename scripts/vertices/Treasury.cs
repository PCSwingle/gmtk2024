using GMTK2024.scenes;
using GMTK2024.scripts.recipes;
using Godot;
using VertexIO = GMTK2024.scenes.vertex.VertexIO;

namespace GMTK2024.scripts.vertices;

public class Treasury() : VertexType(
    "Treasury",
    Palette.TreasuryColor,
    [],
    ["Coin"],
    [new TreasuryRecipe()]
) {
    private static readonly PackedScene TreasuryBodyScene =
        ResourceLoader.Load<PackedScene>("res://scenes/vertices/treasury.tscn");

    private Hud.OnUpdateHudEventHandler _callback = null!;

    public override void Create() {
        var body = this.VertexNode.GetNode<Control>("VertexBody");
        var treasuryBody = TreasuryBodyScene.Instantiate<Control>();
        var label = treasuryBody.GetNode<Label>("CoinLabel");
        this._callback = () => { label.Text = Game.State.Coins.ToString(); };
        Main.HudNode.OnUpdateHud += this._callback;
        body.AddChild(treasuryBody);
    }

    public override void Delete() {
        Main.HudNode.OnUpdateHud -= this._callback;
    }

    public override int AllowedMultiples() {
        return Mathf.Min(
            VertexIO.TransferRate,
            Game.State.Coins
        );
    }

    public override void ProcessSideEffect(int multiple) {
        Game.State.Coins -= multiple;
    }
}