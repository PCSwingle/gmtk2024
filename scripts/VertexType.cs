using Godot;
using Vertex = GMTK2024.scenes.vertex.Vertex;

namespace GMTK2024.scripts;

public abstract class VertexType(
    string name,
    Color color,
    string[] inputLabels,
    string[] outputLabels,
    Recipe[] recipes
) {
    public readonly Color Color = color;
    public readonly string[] InputLabels = inputLabels;
    public readonly string Name = name;
    public readonly string[] OutputLabels = outputLabels;
    public readonly Recipe[] Recipes = recipes;
    public Vertex VertexNode = null!;

    public virtual void Create() { }
    public virtual void Ready() { }
    public virtual void Delete() { }

    public virtual int AllowedMultiples() {
        return 1;
    }

    public virtual void ProcessSideEffect(
        Recipe recipe,
        int multiple
    ) { }
}