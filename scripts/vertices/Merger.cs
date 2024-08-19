using GMTK2024.scenes;
using GMTK2024.scripts.recipes;

namespace GMTK2024.scripts.vertices;

public class Merger() : VertexType(
    "Merger",
    Palette.VertexControlColor,
    ["", ""],
    [""],
    [new MergingRecipe()]
) {
    public static Constructable Constructor = new(
        "Merger",
        () => new Merger()
    );

    public override int AllowedMultiples() {
        return VertexIO.TransferRate;
    }
}