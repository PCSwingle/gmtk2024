using GMTK2024.scenes.vertex;
using Godot;

namespace GMTK2024.scripts.vertices;

public class ConstructorWithDropdown(
    string name,
    Color color,
    string[] inputLabels,
    string[] outputLabels,
    Recipe[] recipes,
    Constructable[] constructableVertices
) : ConstructionVertex(
    name,
    color,
    inputLabels,
    outputLabels,
    recipes
) {
    private static readonly PackedScene ConstructorBodyScene =
        ResourceLoader.Load<PackedScene>("res://scenes/vertices/constructor_with_dropdown.tscn");


    protected Button AcceptButton = null!;
    protected OptionButton Dropdown = null!;
    protected VertexProgress ProgressBar = null!;

    protected int Progress;

    private Constructable _SelectedConstructable() {
        return constructableVertices[this.Dropdown.Selected];
    }

    public override void Create() {
        var body = this.VertexNode.GetNode<Control>("VertexBody");
        var constructorBody = ConstructorBodyScene.Instantiate<Control>();

        this.ProgressBar = constructorBody.GetNode<VertexProgress>("VertexProgress");

        this.AcceptButton = constructorBody.GetNode<Button>("AcceptButton");
        this.AcceptButton.Pressed += () => {
            this.CreateVertex(this._SelectedConstructable());
            this.Progress = 0;
            this.AcceptButton.Disabled = true;
            this.Dropdown.Selected = -1;
        };

        this.Dropdown = constructorBody.GetNode<OptionButton>("ConstructorDropdown");
        for (var i = 0; i < constructableVertices.Length; i++) {
            var constructable = constructableVertices[i];
            this.Dropdown.AddIconItem(
                constructable.ConstructionSprite,
                constructable.Name,
                i
            );
        }

        this.Dropdown.Selected = -1;
        this.Dropdown.ItemSelected += index => {
            this.Progress = 0;
            this.AcceptButton.Disabled =
                this.Progress != this._SelectedConstructable().RequiredProgress || index == -1;
        };
        body.AddChild(constructorBody);
    }

    public override int AllowedMultiples() {
        return this.Dropdown.Selected != -1
            ? Mathf.Min(
                2,
                this._SelectedConstructable().RequiredProgress - this.Progress
            )
            : 0;
    }

    public override void ProcessSideEffect(int multiple) {
        this.Progress += multiple;
        if (this.Progress == this._SelectedConstructable().RequiredProgress && this.Dropdown.Selected != -1) {
            this.AcceptButton.Disabled = false;
        }

        this.ProgressBar.UpdateProgress((float) this.Progress / this._SelectedConstructable().RequiredProgress);
    }
}