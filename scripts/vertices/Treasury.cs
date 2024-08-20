using GMTK2024.scenes;
using GMTK2024.scripts.recipes;
using Godot;
using Hud = GMTK2024.scenes.hud.Hud;
using VertexIO = GMTK2024.scenes.vertex.VertexIO;

namespace GMTK2024.scripts.vertices;

public class Treasury(bool isInput) : VertexType(
    "Treasury",
    Palette.MoneyColor,
    isInput ? ["Coin"] : [],
    isInput ? [] : ["Coin"],
    [new TreasuryInputRecipe(), new TreasuryOutputRecipe()]
) {
    private static readonly PackedScene TreasuryBodyScene =
        ResourceLoader.Load<PackedScene>("res://scenes/vertices/treasury.tscn");

    private Hud.OnUpdateHudEventHandler _callback = null!;

    public override void Create() {
        var body = this.VertexNode.GetNode<Control>("VertexBody");
        var treasuryBody = TreasuryBodyScene.Instantiate<Control>();
        if (isInput) {
            treasuryBody.Position = new Vector2(
                147,
                0
            );
        }

        var label = treasuryBody.GetNode<Label>("CoinLabel");
        this._callback = () => { label.Text = (Game.State.Coins / 1000).ToString("0.0") + "k"; };
        Main.HudNode.OnUpdateHud += this._callback;
        body.AddChild(treasuryBody);
    }

    public override void Delete() {
        Main.HudNode.OnUpdateHud -= this._callback;
    }

    public override int AllowedMultiples() {
        return Mathf.Min(
            VertexIO.TransferRate,
            (int) Game.State.Coins
        );
    }

    public override void ProcessSideEffect(
        Recipe recipe,
        int multiple
    ) {
        if (recipe is TreasuryInputRecipe) {
            Game.State.Coins += multiple;
        } else if (recipe is TreasuryOutputRecipe) {
            Game.State.Coins -= multiple;
        }
    }
}