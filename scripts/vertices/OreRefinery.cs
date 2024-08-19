using GMTK2024.scripts.recipes;

namespace GMTK2024.scripts.vertices;

public class OreRefinery() : VertexType(
    "Ore Refinery",
    Palette.ManufacturingColor,
    ["Ore"],
    ["Ingot"],
    [new RefiningRecipe()]
) { }