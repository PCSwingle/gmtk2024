using GMTK2024.scripts;
using Godot;

namespace GMTK2024.scenes.vertex;

public partial class VertexProgress : Node2D {
    private Sprite2D _progressBar = null!;

    public override void _Ready() {
        this._progressBar = this.GetNode<Sprite2D>("ProgressSprite");
        this.UpdateProgress(0f);
    }

    public void UpdateProgress(float progress) {
        var level = Mathf.Clamp(
            progress,
            0.0378f,
            1f
        );
        var pixelsMissing = 80 - (int) (level * 80);
        this._progressBar.Position = new Vector2(
            this._progressBar.Position.X,
            4 + pixelsMissing
        );
        this._progressBar.RegionRect = new Rect2(
            new Vector2(
                this._progressBar.RegionRect.Position.X,
                pixelsMissing
            ),
            new Vector2(
                this._progressBar.RegionRect.Size.X,
                80 - pixelsMissing
            )
        );

        var color = Palette.FillGradient(1 - (float) pixelsMissing / 80);
        this._progressBar.Modulate = color;
    }
}