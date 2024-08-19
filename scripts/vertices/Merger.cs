using GMTK2024.scripts.recipes;
using VertexIO = GMTK2024.scenes.vertex.VertexIO;

namespace GMTK2024.scripts.vertices;

public class Merger() : VertexType(
    "Merger",
    Palette.VertexControlColor,
    ["Input", "Input"],
    ["Output"],
    [new MergingRecipe()]
) {
    public override int AllowedMultiples() {
        return VertexIO.TransferRate;
    }
}