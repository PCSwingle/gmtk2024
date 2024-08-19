using Godot;

namespace GMTK2024.scripts;

public static class Palette {
    public static readonly Color FillFull = new("005d32");
    public static readonly Color FillEmpty = new("84292a");

    public static readonly Color LineColor = new("272727");

    public static readonly Color InvalidColor = new("770001");

    // Vertex Colors
    public static readonly Color ConstructionColor = new("913a07");
    public static readonly Color MaterialProducersColor = new("8d7d6f");
    public static readonly Color LogisticsColor = new("dbc700");
    public static readonly Color ManufacturingColor = new("616161");
    public static readonly Color MoneyColor = new("fffc40");

    public static Color FillGradient(float weight) {
        return FillEmpty.Lerp(
            FillFull,
            weight
        );
    }
}