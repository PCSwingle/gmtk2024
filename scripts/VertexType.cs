using Godot;

namespace GMTK2024.scripts;

public struct VertexType(string name, Color color, string[] inputLabels, string[] outputLabels, Recipe[] recipes) {
    public readonly string Name = name;
    public readonly Color Color = color;
    public readonly string[] InputLabels = inputLabels;
    public readonly string[] OutputLabels = outputLabels;
    public readonly Recipe[] Recipes = recipes;
}

public static class VertexTypes {
    private static readonly Color ControlVertexColor = new("ded16d");

    public static readonly VertexType IronMine = new("Iron Mine", new Color("8d7d6f"), [], ["Iron Ore"],
        [Recipes.IronMineRecipe]);

    public static readonly VertexType Smeltery = new("Smeltery", new Color("616161"), ["Ore"], ["Ingot"],
        [Recipes.SmeltingRecipe]);

    public static readonly VertexType Splitter = new("Splitter", ControlVertexColor, [""], ["", ""],
        [Recipes.SplittingRecipe]);

    public static readonly VertexType Merger = new("Merger", ControlVertexColor, ["", ""], [""],
        [Recipes.MergingRecipe]);
}