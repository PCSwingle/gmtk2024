using GMTK2024.scripts.recipes;

namespace GMTK2024.scripts.vertices;

public class ManufacturingIndustry() : ConstructorWithDropdown(
    "Manufacturing Industry",
    Palette.ConstructionColor,
    ["Coin"],
    [],
    [new ManufacturingIndustryRecipe()],
    Constructable.ManufacturingVertices
) { }