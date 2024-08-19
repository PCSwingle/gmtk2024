using System.Collections.Generic;
using GMTK2024.scenes.hud;
using GMTK2024.scripts;
using Godot;

namespace GMTK2024.scenes.vertex;

public partial class Vertex : Node2D {
    private static readonly PackedScene VertexScene =
        ResourceLoader.Load<PackedScene>("res://scenes/vertex/vertex.tscn");

    public static readonly List<Vertex> FocusOrder = new();

    private bool _dirty = true;
    private Recipe? _curRecipe;

    private VertexType _type = null!;
    private readonly List<VertexIO> _inputs = new();
    private readonly List<VertexIO> _outputs = new();

    private bool _dragging;
    private Vector2 _dragLocation;
    private Vector2 _lastMousePosition;

    public Dictionary<string, object> Metadata = [];

    private static void Refocus() {
        for (var i = 0; i < FocusOrder.Count; i++) {
            var v = FocusOrder[i];
            v.ZIndex = i * 2;
            v.MoveToFront();
        }
    }

    public static Vertex CreateVertex(VertexType type) {
        var v = VertexScene.Instantiate<Vertex>();
        v._type = type;
        type.VertexNode = v;
        v.GetNode<Label>("VertexLabel").Text = type.Name;
        v.GetNode<Node2D>("VertexSprites").Modulate = type.Color;

        foreach (var inputLabel in type.InputLabels) {
            v._AddVertexIo(
                inputLabel,
                true
            );
        }

        foreach (var outputLabel in type.OutputLabels) {
            v._AddVertexIo(
                outputLabel,
                false
            );
        }

        v._type.Create();
        return v;
    }

    public override void _Ready() {
        var control = this.GetNode<Control>("VertexControl");
        control.GuiInput += this._GuiInput;

        FocusOrder.Insert(
            0,
            this
        );
        Refocus();
        this._type.Ready();
    }

    public override void _ExitTree() {
        this._type.Delete();
    }

    private void _AddVertexIo(
        string label,
        bool input
    ) {
        var v = VertexIO.CreateVertexIo(
            this,
            label,
            input
        );
        v.Position = new Vector2(
            v.Position.X,
            50 + 40 * (input ? this._inputs.Count : this._outputs.Count)
        );
        this.AddChild(v);
        (input ? this._inputs : this._outputs).Add(v);
    }

    public void MarkDirty() {
        this._dirty = true;
    }

    public void ProcessResources() {
        if (this._dirty) {
            this._dirty = false;
            this._curRecipe = null;

            foreach (var recipe in this._type.Recipes) {
                if (recipe.Match(
                        this._inputs,
                        this._outputs
                    )) {
                    this._curRecipe = recipe;
                    break;
                }
            }
        }

        if (this._curRecipe != null) {
            var succeeded = this._curRecipe.Process(
                this._type,
                this._inputs,
                this._outputs
            );
            this.GetNode<Sprite2D>("VertexSprites/VertexBodySprite").Modulate = Colors.White; // new Color("eab99a");
        } else {
            this.GetNode<Sprite2D>("VertexSprites/VertexBodySprite").Modulate = new Color("f9ada8");
        }
    }

    public void SendResources() {
        foreach (var v in this._outputs) {
            v.SendResources();
        }

        foreach (var v in this._inputs) {
            v.SendResources();
        }
    }

    private void _ClampPosition() {
        var halfSize = Command.CommandSize / 2;
        var control = this.GetNode<Control>("VertexControl");
        this.Position = this.Position.Clamp(
            new Vector2(
                -halfSize,
                -halfSize
            ),
            new Vector2(
                halfSize - control.Size.X,
                halfSize - control.Size.Y
            )
        );
    }

    public override void _UnhandledInput(InputEvent e) {
        if (e is InputEventMouseButton { Pressed: false, ButtonIndex: MouseButton.Left }) {
            this._dragging = false;
        }

        if (e is InputEventMouseMotion me) {
            if (this._dragging) {
                var mp = this.GetParent<Node2D>().ToLocal(me.GlobalPosition);
                this.Position = mp - this._dragLocation;
                this._ClampPosition();

                foreach (var v in this._inputs) {
                    v.Connected?.OnMove();
                }

                foreach (var v in this._outputs) {
                    v.Connected?.OnMove();
                }
            }

            this._lastMousePosition = this.GetParent<Node2D>().ToLocal(me.Position);
        }
    }

    private void _GuiInput(InputEvent e) {
        if (e is InputEventMouseButton { Pressed: true, ButtonIndex: MouseButton.Left } me) {
            this._dragging = true;
            this._dragLocation = this.ToLocal(me.GlobalPosition);
            FocusOrder.Remove(this);
            FocusOrder.Add(this);
            Refocus();
        }
    }
}