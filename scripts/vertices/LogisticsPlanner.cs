using GMTK2024.scripts.recipes;

namespace GMTK2024.scripts.vertices;

public class LogisticsPlanner()
    : ConstructorWithDropdown(
        "Logistics Planner",
        Palette.ConstructionColor,
        ["Coin"],
        [],
        [new LogisticsPlannerRecipe()],
        Constructable.LogisticsVertices
    ) { }