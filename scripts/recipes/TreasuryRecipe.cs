namespace GMTK2024.scripts.recipes;

public class TreasuryInputRecipe()
    : Recipe(
        [(Resources.Coin, 1)],
        []
    ) { }

public class TreasuryOutputRecipe()
    : Recipe(
        [],
        [(Resources.Coin, 1)]
    ) { }