using System;
using GMTK2024.scenes;
using GMTK2024.scripts.recipes;
using Godot;

namespace GMTK2024.scripts.vertices;

public struct Constructable(
    string name,
    Func<VertexType> vertexConstructor
) {
    public readonly string Name = name;

    public readonly Texture2D ConstructionSprite =
        ResourceLoader.Load<CompressedTexture2D>($"res://sprites/vertices/{name.ToLower().Replace(" ", "_")}.png");

    public readonly Func<VertexType> VertexConstructor = vertexConstructor;
}

public class ConstructionPlanner()
    : VertexType(
        "Construction Planner",
        Palette.ConstructionPlannerColor,
        ["Coin"],
        [],
        [new ConstructionPlannerCoinRecipe()]
    ) {
    private const int MaxProgress = 200;

    private static readonly PackedScene ConstructionBodyScene =
        ResourceLoader.Load<PackedScene>("res://scenes/construction_body.tscn");

    public static Constructable Constructor = new(
        "Construction Planner",
        () => new Merger()
    );

    private static readonly Constructable[] ConstructableVertices =
        [Splitter.Constructor, Merger.Constructor, Constructor];

    private Button _acceptButton = null!;
    private Action _buttonCallback = null!;

    private OptionButton.ItemSelectedEventHandler _callback = null!;

    private OptionButton _dropdown = null!;

    private int _progress;
    private Sprite2D _progressBar = null!;

    private void _UpdateProgress() {
        var level = Mathf.Clamp(
            this._progress / (float) MaxProgress,
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

        var color = Palette.IoFillColors[pixelsMissing / 20];
        this._progressBar.Modulate = color;
    }

    private void _CreateBuilding() {
        var constructable = ConstructableVertices[this._dropdown.Selected];
        Command.CreateVertex(
            constructable.VertexConstructor(),
            this.VertexNode.Position + new Vector2(
                80,
                80
            )
        );


        this._progress = 0;
        this._dropdown.Selected = -1;
    }

    public override void Create() {
        var body = this.VertexNode.GetNode<Control>("VertexBody");
        var treasuryBody = ConstructionBodyScene.Instantiate<Control>();
        this._progressBar = treasuryBody.GetNode<Sprite2D>("ProgressSprite");

        this._acceptButton = treasuryBody.GetNode<Button>("AcceptButton");
        this._buttonCallback = this._CreateBuilding;
        this._acceptButton.Pressed += this._buttonCallback;

        this._dropdown = treasuryBody.GetNode<OptionButton>("ConstructionDropdown");
        for (var i = 0; i < ConstructableVertices.Length; i++) {
            var constructable = ConstructableVertices[i];
            this._dropdown.AddIconItem(
                constructable.ConstructionSprite,
                constructable.Name,
                i
            );
        }

        this._dropdown.Selected = -1;
        this._callback = index => {
            this._progress = 0;
            this._acceptButton.Disabled = this._progress != MaxProgress || index == -1;
        };
        this._dropdown.ItemSelected += this._callback;
        this._UpdateProgress();
        body.AddChild(treasuryBody);
    }

    public override void Delete() {
        this._dropdown.ItemSelected -= this._callback;
        this._acceptButton.Pressed -= this._buttonCallback;
    }

    public override int AllowedMultiples() {
        return this._dropdown.Selected != -1
            ? Mathf.Min(
                2,
                MaxProgress - this._progress
            )
            : 0;
    }

    public override void ProcessSideEffect(int multiple) {
        this._progress += multiple;
        if (this._progress == MaxProgress && this._dropdown.Selected != -1) {
            this._acceptButton.Disabled = false;
        }

        this._UpdateProgress();
    }
}