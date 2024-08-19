using System;
using System.Collections.Generic;
using Godot;

namespace GMTK2024.scripts;

public readonly struct Resource {
    public readonly string Name;
    public readonly Texture2D Sprite;
    private readonly Func<Resource, Resource?>? _match;

    public Resource(
        string name,
        Func<Resource, Resource?>? match = null
    ) {
        this.Name = name;
        this.Sprite = match == null
            ? Utils.LoadSprite(
                name,
                "resources"
            )
            : new PlaceholderTexture2D();
        this._match = match;

        Resources.RegisteredResources[name] = this;
    }

    public Resource? MatchResource(Resource? res) {
        if (res == null || this._match == null) {
            return this == res ? this : null;
        } else {
            return this._match((Resource) res);
        }
    }

    public static bool operator ==(
        Resource r1,
        Resource r2
    ) {
        return r1.Name == r2.Name;
    }

    public static bool operator !=(
        Resource r1,
        Resource r2
    ) {
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
    public static readonly Dictionary<string, Resource> RegisteredResources = new();

    // Resources
    public static readonly Resource IronOre = new("Iron Ore");
    public static readonly Resource Iron = new("Iron");
    public static readonly Resource Coin = new("Coin");


    // Resource Matchers
    public static readonly Resource Any = new(
        "Any",
        res => res
    );

    public static readonly Resource Ore = new(
        "Ore",
        res => {
            if (res == IronOre) {
                return Iron;
            } else {
                return null;
            }
        }
    );
}