using System;
using System.Collections.Generic;
using GMTK2024.scenes.vertex;
using GMTK2024.scripts.recipes;
using Godot;
using Command = GMTK2024.scenes.hud.Command;

namespace GMTK2024.scripts.vertices;

public struct Constructable(
    string name,
    Func<VertexType> vertexConstructor
) {
    public readonly string Name = name;

    public readonly Texture2D ConstructionSprite = Utils.LoadSprite(
        name,
        "vertices"
    );

    public readonly Func<VertexType> VertexConstructor = vertexConstructor;

    public static readonly List<Constructable> ConstructableVertices = [
        new Constructable(
            "Splitter",
            () => new Splitter()
        ),
        new Constructable(
            "Merger",
            () => new Merger()
        ),
        new Constructable(
            "Construction Planner",
            () => new ConstructionPlanner()
        )
    ];
}

public class ConstructionPlanner()
    : VertexType(
        "Construction Planner",
        Palette.ConstructionColor,
        ["Coin"],
        [],
        [new ConstructionPlannerRecipe()]
    ) {
    private const int MaxProgress = 200;

    private static readonly PackedScene ConstructionBodyScene =
        ResourceLoader.Load<PackedScene>("res://scenes/vertices/construction_planner.tscn");


    private Button _acceptButton = null!;
    private OptionButton _dropdown = null!;
    private VertexProgress _progressBar = null!;

    private int _progress;

    private void _CreateBuilding() {
        var constructable = Constructable.ConstructableVertices[this._dropdown.Selected];
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

    public override async void Create() {
        var body = this.VertexNode.GetNode<Control>("VertexBody");
        var treasuryBody = ConstructionBodyScene.Instantiate<Control>();
        this._progressBar = treasuryBody.GetNode<VertexProgress>("VertexProgress");
        this._acceptButton = treasuryBody.GetNode<Button>("AcceptButton");
        this._acceptButton.Pressed += this._CreateBuilding;

        this._dropdown = treasuryBody.GetNode<OptionButton>("ConstructionDropdown");
        for (var i = 0; i < Constructable.ConstructableVertices.Count; i++) {
            var constructable = Constructable.ConstructableVertices[i];
            this._dropdown.AddIconItem(
                constructable.ConstructionSprite,
                constructable.Name,
                i
            );
        }

        this._dropdown.Selected = -1;
        this._dropdown.ItemSelected += index => {
            this._progress = 0;
            this._acceptButton.Disabled = this._progress != MaxProgress || index == -1;
        };
        body.AddChild(treasuryBody);

        await this._progressBar.ToSignal(
            this._progressBar,
            Node.SignalName.Ready
        );
        this._progressBar.UpdateProgress((float) this._progress / MaxProgress);
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

        this._progressBar.UpdateProgress((float) this._progress / MaxProgress);
    }
}