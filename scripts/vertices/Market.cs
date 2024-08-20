using GMTK2024.scripts.recipes;

namespace GMTK2024.scripts.vertices;

public class Market() : VertexType(
    "Market",
    Palette.MoneyColor,
    [],
    ["Coin"],
    [new MarketRecipe()]
) { }