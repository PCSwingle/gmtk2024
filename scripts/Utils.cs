using Godot;

namespace GMTK2024.scripts;

public static class Utils {
    public static Texture2D LoadSprite(
        string name,
        string directory,
        bool failOk = false
    ) {
        var resource = $"res://sprites/{directory}/{name.ToLower().Replace(" ", "_")}.png";
        if (!ResourceLoader.Exists(resource)) {
            if (!failOk) {
                GD.PrintErr($"Could not find sprite {resource}!");
            }
            return new PlaceholderTexture2D();
        }
        return ResourceLoader.Load<CompressedTexture2D>(
            resource
        );
    }
}