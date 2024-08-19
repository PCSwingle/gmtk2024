using System;
using Godot;

namespace GMTK2024.scripts;

public readonly struct Resource(string name, Func<Resource, Resource?>? match = null) {
    public readonly string Name = name;

    public readonly Texture2D Sprite = match == null
        ? ResourceLoader.Load<CompressedTexture2D>($"res://sprites/resources/{name.ToLower().Replace(" ", "_")}.png")
        : new PlaceholderTexture2D();


    public Resource? MatchResource(Resource? res) {
        if (res == null || match == null) {
            return this == res ? this : null;
        } else {
            return match((Resource) res);
        }
    }

    public static bool operator ==(Resource r1, Resource r2) {
        return r1.Name == r2.Name;
    }

    public static bool operator !=(Resource r1, Resource r2) {
        return r1.Name != r2.Name;
    }

    private bool Equals(Resource other) {
        return this.Name == other.Name;
    }

    public override bool Equals(object? obj) {
        return obj is Resource other && this.Equals(other);
    }

    public override int GetHashCode() {
        return HashCode.Combine(this.Name);
    }
}

public static class Resources {
    // Resources
    public static readonly Resource IronOre = new("Iron Ore");
    public static readonly Resource Iron = new("Iron");
    public static readonly Resource Coin = new("Coin");


    // Resource Matchers
    public static readonly Resource Any = new("Any", res => res);

    public static readonly Resource Ore = new("Ore", res => {
        if (res == IronOre) {
            return Iron;
        } else {
            return null;
        }
    });
}