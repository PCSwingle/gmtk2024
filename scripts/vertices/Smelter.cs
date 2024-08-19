using GMTK2024.scripts.recipes;

namespace GMTK2024.scripts.vertices;

public class Smelter() : VertexType("Smelter", Palette.SmelterColor, ["Ore"], ["Ingot"],
    [new SmeltingRecipe()]) { }