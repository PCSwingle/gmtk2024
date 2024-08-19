using GMTK2024.scripts.recipes;

namespace GMTK2024.scripts.vertices;

public class IronMine() : VertexType(
    "Iron Mine",
    Palette.MaterialProducersColor,
    [],
    ["Iron Ore"],
    [
        new ProducerRecipe(
            Resources.IronOre,
            4
        )
    ]
) { }