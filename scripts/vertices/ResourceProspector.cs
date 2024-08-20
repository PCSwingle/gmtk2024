using GMTK2024.scripts.recipes;
using Godot;

namespace GMTK2024.scripts.vertices;

public class ResourceProspector() : ConstructorWithDropdown(
    "Resource Prospector",
    Palette.ConstructionColor,
    ["Coin"],
    [],
    [new ResourceProspectorRecipe()],
    Constructable.ProspectableVertices
) {
    private void _SelectRandomVertex() {
        this.Dropdown.Select((int) GD.Randi() % Constructable.ProspectableVertices.Length);
    }

    public override void Create() {
        base.Create();
        this.Dropdown.AddThemeIconOverride(
            "arrow",
            Utils.LoadSprite(
                "blank_menu_arrow",
                "icons"
            )
        );
        this.Dropdown.MouseFilter = Control.MouseFilterEnum.Ignore;
        this._SelectRandomVertex();
    }

    protected override void OnBuild() {
        base.OnBuild();
        this._SelectRandomVertex();
    }
}