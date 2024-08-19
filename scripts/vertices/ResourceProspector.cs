using GMTK2024.scripts.recipes;

namespace GMTK2024.scripts.vertices;

public class ResourceProspector() : ConstructorWithDropdown(
    "Resource Prospector",
    Palette.ConstructionColor,
    ["Coin"],
    [],
    [new ResourceProspectorRecipe()],
    Constructable.ProspectableVertices
) {
    public override void Create() {
        base.Create();
        this.Dropdown.Icon = null;
    }
}