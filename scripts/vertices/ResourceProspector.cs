using GMTK2024.scripts.recipes;

namespace GMTK2024.scripts.vertices;

public class ResourceProspector() : VertexType(
    "Resource Prospector",
    Palette.ConstructionColor,
    ["Coin"],
    [],
    [new ResourceProspectorRecipe()]
) { }