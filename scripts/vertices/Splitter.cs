using GMTK2024.scenes;
using GMTK2024.scripts.recipes;

namespace GMTK2024.scripts.vertices;

public class Splitter() : VertexType(
    "Splitter",
    Palette.VertexControlColor,
    [""],
    ["", ""],
    [new SplittingRecipe()]
) {
    public static Constructable Constructor = new(
        "Splitter",
        () => new Splitter()
    );

    public override int AllowedMultiples() {
        return VertexIO.TransferRate;
    }
}