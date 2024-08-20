using System;
using System.Collections.Generic;
using Godot;

namespace GMTK2024.scripts;

public readonly struct Resource {
    public readonly string Name;
    public readonly Texture2D Sprite;
    public readonly float? BasePrice;
    private readonly Func<Resource, int, (Resource, int)?>? _match;

    public Resource(
        string name,
        float? basePrice = null,
        Func<Resource, int, (Resource, int)?>? match = null
    ) {
        this.Name = name;
        this.Sprite = Utils.LoadSprite(
            name,
            "resources",
            match != null
        );
        this.BasePrice = basePrice;
        this._match = match;

        Resources.RegisteredResources[name] = this;
    }

    public (Resource, int)? MatchResource(
        Resource? res,
        int amount
    ) {
        if (res == null || this._match == null) {
            return this == res ? (this, 1) : null;
        } else {
            return this._match(
                (Resource) res,
                amount
            );
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
    public static readonly Resource IronOre = new(
        "Iron Ore",
        50
    );

    public static readonly Resource Iron = new(
        "Iron",
        60
    );

    public static readonly Resource Coin = new(
        "Coin"
    );


    // Resource Matchers
    public static readonly Resource Any = new(
        "Any",
        match: (
            res,
            amount
        ) => (res, amount)
    );

    public static readonly Resource Ore = new(
        "Ore",
        match: (
            res,
            amount
        ) => {
            if (res == IronOre) {
                return (Iron, amount);
            } else {
                return null;
            }
        }
    );
}