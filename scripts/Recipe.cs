using System.Collections.Generic;
using System.Linq;
using GMTK2024.scenes;

namespace GMTK2024.scripts;

public class Recipe(List<(Resource, int)>? recipeInputs = null, List<(Resource, int)>? recipeOutputs = null) {
    private readonly List<(Resource, int)> _recipeInputs = recipeInputs ?? [];
    private readonly List<(Resource, int)> _recipeOutputs = recipeOutputs ?? [];

    public (int, int) IoCount() {
        return (this._recipeInputs.Count, this._recipeOutputs.Count);
    }

    public virtual bool Match(List<VertexIO> inputs,
        List<VertexIO> outputs) {
        // Check to see if inputs are in order and ensure no extra inputs still connected
        Dictionary<Resource, Resource> matched = [];
        List<VertexIO> unusedInputs = [];
        for (var i = 0; i < inputs.Count; i++) {
            var v = inputs[i];
            if (i >= this._recipeInputs.Count) {
                if (v.Connected is not null) {
                    return false;
                } else {
                    unusedInputs.Add(v);
                }
            } else {
                var (res, amount) = this._recipeInputs[i];
                var matchedResource = res.MatchResource(v.ConnectedRes);
                if (v.ConnectedRes == null || matchedResource == null ||
                    matchedResource != matched.GetValueOrDefault(res, (Resource) matchedResource)) {
                    return false;
                } else {
                    matched[res] = (Resource) matchedResource;
                }
            }
        }

        // Clear unused inputs and all outputs that are not correct already
        foreach (var v in unusedInputs) {
            v.Reset();
        }

        for (var i = 0; i < outputs.Count; i++) {
            var v = outputs[i];
            if (i >= this._recipeOutputs.Count) {
                v.Reset();
            } else {
                var outputResource = this._recipeOutputs[i].Item1;
                var producedResource = matched.GetValueOrDefault(outputResource, outputResource);
                v.SetResource(producedResource);
            }
        }

        return true;
    }

    private List<(VertexIO, int)>? _GetSiphons(List<VertexIO> vs, bool siphoning) {
        Dictionary<Resource, List<VertexIO>> sortedVs = [];
        Dictionary<Resource, int> amounts = [];
        var siphoningFor = siphoning ? this._recipeInputs : this._recipeOutputs;
        for (var i = 0; i < siphoningFor.Count; i++) {
            var (res, amount) = siphoningFor[i];
            if (!sortedVs.ContainsKey(res)) {
                sortedVs[res] = [];
                amounts[res] = 0;
            }

            sortedVs[res].Add(vs[i]);
            amounts[res] += amount;
        }

        List<(VertexIO, int)> allSiphons = [];
        foreach (var res in sortedVs.Keys) {
            // Try to split it evenly, but always give all if possible
            (VertexIO, int)[] siphons = [..sortedVs[res].Select(vertexIo => (vertexIo, 0))];
            var amount = amounts[res];
            while (amount > 0) {
                var lastAmount = amount;
                for (var i = 0; i < siphons.Length; i++) {
                    var (v, alreadySiphoned) = siphons[i];
                    var remaining = (siphoning ? v.Storage : VertexIO.MaxStorage - v.Storage) - alreadySiphoned;
                    if (remaining > 0) {
                        amount -= 1;
                        siphons[i] = (v, alreadySiphoned + 1);
                    }
                }

                if (lastAmount == amount) {
                    return null;
                }
            }

            allSiphons.AddRange(siphons);
        }

        return allSiphons;
    }

    public virtual bool Process(List<VertexIO> inputs, List<VertexIO> outputs) {
        // Assume already matched

        // Check for blockage
        var allSiphons = this._GetSiphons(inputs, true);
        var allDumps = this._GetSiphons(outputs, false);
        if (allSiphons == null || allDumps == null) {
            return false;
        }

        // Process recipe
        foreach (var (v, amount) in allSiphons) {
            v.Storage -= amount;
        }

        foreach (var (v, amount) in allDumps) {
            v.Storage += amount;
        }

        return true;
    }
}

public static class Recipes {
    public static readonly Recipe IronMineRecipe = new([], [(Resources.IronOre, 4)]);
    public static readonly Recipe SmeltingRecipe = new([(Resources.Ore, 2)], [(Resources.Ore, 1)]);
    public static readonly Recipe SplittingRecipe = new([(Resources.Any, 6)], [(Resources.Any, 3), (Resources.Any, 3)]);
    public static readonly Recipe MergingRecipe = new([(Resources.Any, 3), (Resources.Any, 3)], [(Resources.Any, 6)]);
}