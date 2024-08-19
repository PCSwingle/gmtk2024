using GMTK2024.scripts.recipes;
using VertexIO = GMTK2024.scenes.vertex.VertexIO;

namespace GMTK2024.scripts.vertices;

public class Splitter() : VertexType(
    "Splitter",
    Palette.LogisticsColor,
    ["Input"],
    ["Output", "Output"],
    [new SplittingRecipe()]
) {
    public override int AllowedMultiples() {
        return VertexIO.TransferRate;
    }
}