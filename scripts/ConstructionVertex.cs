using System.Collections.Generic;
using GMTK2024.scenes.hud;
using GMTK2024.scenes.vertex;
using Godot;

namespace GMTK2024.scripts;

public class ConstructionVertex(
    string name,
    Color color,
    string[] inputLabels,
    string[] outputLabels,
    Recipe[] recipes
) : VertexType(
    name,
    color,
    inputLabels,
    outputLabels,
    recipes
) {
    private static readonly Vector2 Offset = new(
        50,
        50
    );

    private readonly Dictionary<int, Vertex> _takenSpots = [];

    protected void CreateVertex(Constructable constructable) {
        for (var i = 1;; i++) {
            if (!this._takenSpots.TryGetValue(
                    i,
                    out var v
                ) || v.Position != this.VertexNode.Position + i * Offset) {
                this._takenSpots[i] = Command.CreateVertex(
                    constructable.VertexConstructor(),
                    this.VertexNode.Position + i * Offset
                );
                break;
            }
        }
    }
}