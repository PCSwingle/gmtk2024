using System;
using GMTK2024.scripts.vertices;
using Godot;

namespace GMTK2024.scripts;

public struct Constructable(
    string name,
    int requiredProgress,
    Func<VertexType> vertexConstructor
) {
    public readonly string Name = name;
    public readonly int RequiredProgress = requiredProgress;

    public readonly Texture2D ConstructionSprite = Utils.LoadSprite(
        name,
        "vertices",
        true
    );

    public readonly Func<VertexType> VertexConstructor = vertexConstructor;

    public static readonly Constructable[] LogisticsVertices = [
        new Constructable(
            "Splitter",
            50,
            () => new Splitter()
        ),
        new Constructable(
            "Merger",
            50,
            () => new Merger()
        ),
        new Constructable(
            "Logistics Planner",
            200,
            () => new LogisticsPlanner()
        ),
        new Constructable(
            "Resource Prospector",
            200,
            () => new ResourceProspector()
        ),
        new Constructable(
            "Manufacturing Industry",
            200,
            () => new ManufacturingIndustry()
        )
    ];

    public static readonly Constructable[] ProspectableVertices = [
        new Constructable(
            "Iron Mine",
            50,
            () => new IronMine()
        )
    ];

    public static readonly Constructable[] ManufacturingVertices = [
        new Constructable(
            "Ore Refinery",
            50,
            () => new OreRefinery()
        )
    ];
}