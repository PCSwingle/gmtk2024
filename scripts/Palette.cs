using Godot;

namespace GMTK2024.scripts;

public static class Palette {
    public static readonly Color IoFillFull = new("005d32");
    public static readonly Color IoFillAlmostFull = new("2e5933");
    public static readonly Color IoFillAlmostEmpty = new("485230");
    public static readonly Color IoFillEmpty = new("84292a");

    public static readonly Color[] IoFillColors =
        [IoFillFull, IoFillAlmostFull, IoFillAlmostEmpty, IoFillEmpty];

    // Vertex Colors
    public static readonly Color VertexControlColor = new("f9a31b");
    public static readonly Color IronMineColor = new("8d7d6f");
    public static readonly Color SmelterColor = new("616161");
    public static readonly Color ConstructionPlannerColor = new("cc7f1b");
    public static readonly Color TreasuryColor = new("fffc40");
}