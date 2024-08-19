using Godot;

namespace GMTK2024.scripts;

public static class Utils {
    public static Texture2D LoadSprite(
        string name,
        string directory
    ) {
        return ResourceLoader.Load<CompressedTexture2D>(
            $"res://sprites/{directory}/{name.ToLower().Replace(" ", "_")}.png"
        );
    }
}