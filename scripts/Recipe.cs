using System.Collections.Generic;
using System.Linq;
using Godot;
using VertexIO = GMTK2024.scenes.vertex.VertexIO;

namespace GMTK2024.scripts;

public class Recipe(
    List<(Resource, int)>? recipeInputs = null,
    List<(Resource, int)>? recipeOutputs = null
) {
    private readonly List<(Resource, int)> _recipeInputs = recipeInputs ?? [];
    private readonly List<(Resource, int)> _recipeOutputs = recipeOutputs ?? [];

    private Dictionary<Resource, (Resource, int)> _matched = [];

    private Dictionary<Resource, List<VertexIO>> _SortIos(
        List<VertexIO> vs,
        List<(Resource, int)> siphoningFor
    ) {
        Dictionary<Resource, List<VertexIO>> sortedVs = [];
        for (var i = 0; i < siphoningFor.Count; i++) {
            var (res, _) = siphoningFor[i];
            sortedVs.TryAdd(
                res,
                []
            );
            sortedVs[res].Add(vs[i]);
        }

        return sortedVs;
    }

    private Dictionary<Resource, int> _SortAmounts(
        List<(Resource, int)> siphoningFor
    ) {
        Dictionary<Resource, int> amounts = [];
        foreach (var (res, amount) in siphoningFor) {
            amounts.TryAdd(
                res,
                0
            );
            amounts[res] += amount * this._matched.GetValueOrDefault(
                res,
                (res, 1)
            ).Item2;
        }

        return amounts;
    }

    public virtual bool Match(
        List<VertexIO> inputs,
        List<VertexIO> outputs
    ) {
        // Check to see if inputs are in order and ensure no extra inputs still connected
        if (inputs.Count < this._recipeInputs.Count || outputs.Count < this._recipeOutputs.Count) {
            return false;
        }

        this._matched = [];
        List<VertexIO> unusedInputs = [];
        HashSet<Resource> usedInputs = [];
        HashSet<Resource> missingInputs = [];
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
                var matchedResource = res.MatchResource(
                    v.ConnectedRes,
                    amount
                );

                if (v.ConnectedRes == null) {
                    missingInputs.Add(res);
                } else if (matchedResource == null ||
                           matchedResource != this._matched.GetValueOrDefault(
                               res,
                               ((Resource, int)) matchedResource
                           )) {
                    return false;
                } else {
                    usedInputs.Add(res);
                    this._matched[res] = ((Resource, int)) matchedResource;
                }
            }
        }

        foreach (var missing in missingInputs) {
            if (!usedInputs.Contains(missing)) {
                return false;
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
                var (producedResource, _) = this._matched.GetValueOrDefault(
                    outputResource,
                    (outputResource, 1)
                );
                v.SetResource(producedResource);
            }
        }

        return true;
    }

    private int _GetMultiple(
        VertexType vertex,
        Dictionary<Resource, List<VertexIO>> sortedVs,
        Dictionary<Resource, int> amounts,
        bool siphoning
    ) {
        var curMin = vertex.AllowedMultiples();
        foreach (var (res, vs) in sortedVs) {
            var total = vs.Sum(v => siphoning ? v.Storage : VertexIO.MaxStorage - v.Storage);
            curMin = Mathf.Min(
                curMin,
                total / amounts[res]
            );
        }

        return curMin;
    }

    private List<(VertexIO, int)>? _GetSiphons(
        Dictionary<Resource, List<VertexIO>> sortedVs,
        Dictionary<Resource, int> amounts,
        bool siphoning,
        int multiple
    ) {
        List<(VertexIO, int)> allSiphons = [];
        foreach (var res in sortedVs.Keys) {
            // Try to split it evenly, but always give all if possible
            (VertexIO, int)[] siphons = [..sortedVs[res].Select(vertexIo => (vertexIo, 0))];
            var amount = amounts[res] * multiple;
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
                    GD.PrintErr("This shouldn't happen. Look into it.");
                    return null;
                }
            }

            allSiphons.AddRange(siphons);
        }

        return allSiphons;
    }

    public virtual bool Process(
        VertexType vertex,
        List<VertexIO> inputs,
        List<VertexIO> outputs
    ) {
        // Assume already matched
        // Check for blockage
        var sortedInputs = this._SortIos(
            inputs,
            this._recipeInputs
        );
        var inputAmounts = this._SortAmounts(this._recipeInputs);

        var sortedOutputs = this._SortIos(
            outputs,
            this._recipeOutputs
        );
        var outputAmounts = this._SortAmounts(this._recipeOutputs);

        var multiple = Mathf.Min(
            this._GetMultiple(
                vertex,
                sortedInputs,
                inputAmounts,
                true
            ),
            this._GetMultiple(
                vertex,
                sortedOutputs,
                outputAmounts,
                false
            )
        );
        if (multiple == 0) {
            return false;
        }

        var allSiphons = this._GetSiphons(
            sortedInputs,
            inputAmounts,
            true,
            multiple
        );
        var allDumps = this._GetSiphons(
            sortedOutputs,
            outputAmounts,
            false,
            multiple
        );
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

        vertex.ProcessSideEffect(
            this,
            multiple
        );
        return true;
    }
}