using System.Collections.Generic;
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

    public virtual bool Process(List<VertexIO> inputs, List<VertexIO> outputs) {
        // Assume already matched

        // Check for blockages
        // TODO: If multiple inputs with same resource, ok taking all from one
        for (var i = 0; i < this._recipeInputs.Count; i++) {
            if (inputs[i].Storage < this._recipeInputs[i].Item2) {
                return false;
            }
        }

        // TODO: If multiple outputs with same resource, ok sending all to one
        for (var i = 0; i < this._recipeOutputs.Count; i++) {
            if (VertexIO.MaxStorage - outputs[i].Storage < this._recipeOutputs[i].Item2) {
                return false;
            }
        }

        // Process recipe
        for (var i = 0; i < this._recipeInputs.Count; i++) {
            inputs[i].Storage -= this._recipeInputs[i].Item2;
        }

        for (var i = 0; i < this._recipeOutputs.Count; i++) {
            outputs[i].Storage += this._recipeOutputs[i].Item2;
        }

        return true;
    }
}

public static class Recipes {
    public static readonly Recipe IronMineRecipe = new([], [(Resources.IronOre, 4)]);
    public static readonly Recipe SmeltingRecipe = new([(Resources.Ore, 2)], [(Resources.Ore, 1)]);
    public static readonly Recipe SplittingRecipe = new([(Resources.Any, 2)], [(Resources.Any, 1), (Resources.Any, 1)]);
    public static readonly Recipe MergingRecipe = new([(Resources.Any, 1), (Resources.Any, 1)], [(Resources.Any, 2)]);
}