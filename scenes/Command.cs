using GMTK2024.scripts;
using GMTK2024.scripts.vertices;
using Godot;

namespace GMTK2024.scenes;

public partial class Command : Node2D {
    public const float RealSize = 1000f;
    public const float MaxZoom = 3f;
    public const float MinZoom = 0.5f;
    public const float MaxExtraZoom = 3.3f;
    public const float MinExtraZoom = 0.45f;
    private const float ZoomInSpeed = 4.5f;
    private const float ZoomOutSpeed = 4f;
    public const float CommandSize = RealSize / MinZoom;

    private bool _dragging;


    private float _zoom = 1;
    private bool _zoomIn;
    private Node2D _zoomNode = null!;
    private bool _zoomOut;

    public static void CreateVertex(
        VertexType type,
        Vector2 position
    ) {
        var v = Vertex.CreateVertex(type);
        v.Position = position;
        Main.CommandNode._zoomNode.AddChild(v);
    }

    public override void _Ready() {
        this._zoomNode = this.GetNode<Node2D>("ZoomContainer/Zoom");
        var control = this.GetNode<Control>("CommandControl");
        control.GuiInput += this._GuiInput;

        this._zoomNode.AddChild(Vertex.CreateVertex(new IronMine()));
        this._zoomNode.AddChild(Vertex.CreateVertex(new Smelter()));
        this._zoomNode.AddChild(Vertex.CreateVertex(new Smelter()));
        this._zoomNode.AddChild(Vertex.CreateVertex(new Splitter()));
        this._zoomNode.AddChild(Vertex.CreateVertex(new Merger()));
        this._zoomNode.AddChild(Vertex.CreateVertex(new Treasury()));
        this._zoomNode.AddChild(Vertex.CreateVertex(new ConstructionPlanner()));

        Main.CommandNode = this;
    }

    private void _ClampPosition() {
        var viewportSize = RealSize / this._zoom;
        var cantSee = CommandSize - viewportSize;
        var min = -(cantSee / 2);
        var max = cantSee / 2;
        if (max < min) {
            min = 0;
            max = 0;
        }

        // WHY ARE THESE 2 LINES NECESSARY
        // I ASSUME IT'S BECAUSE THE ZOOM TRANSFORM IS APPLIED FIRST BUT HOLY SHIT I HATE THIS
        // 3+ hours :(
        min *= this._zoom;
        max *= this._zoom;

        min += 500;
        max += 500;
        this._zoomNode.Position = this._zoomNode.Position.Clamp(
            new Vector2(
                min,
                min
            ),
            new Vector2(
                max,
                max
            )
        );
    }

    public override void _UnhandledInput(InputEvent e) {
        if (e is InputEventMouseButton { Pressed: false, ButtonIndex: MouseButton.Middle or MouseButton.Left }) {
            this._dragging = false;
        }

        if (this._dragging && e is InputEventMouseMotion me) {
            this._zoomNode.Translate(me.Relative);
            this._ClampPosition();
        }
    }

    private void _GuiInput(InputEvent e) {
        if (e is InputEventMouseButton { Pressed: true, ButtonIndex: MouseButton.Middle or MouseButton.Left }) {
            this._dragging = true;
        }
    }

    public override async void _Input(InputEvent e) {
        if (e is InputEventMouseButton { Pressed: true } me) {
            switch (me.ButtonIndex) {
                case MouseButton.WheelUp:
                    if (!this._zoomIn) {
                        this._zoomOut = true;
                    }

                    break;
                case MouseButton.WheelDown:
                    if (!this._zoomOut) {
                        this._zoomIn = true;
                    }

                    break;
                case MouseButton.Middle:
                    var mousePosition = this.ToLocal(me.GlobalPosition);
                    if (mousePosition.X is < RealSize / 2 and > -(RealSize / 2) && mousePosition.Y is < RealSize / 2
                            and > -
                                (RealSize / 2)) {
                        this._dragging = true;
                    }

                    break;
                default:
                    return;
            }

            await this.ToSignal(
                this.GetTree().CreateTimer(0.06f),
                SceneTreeTimer.SignalName.Timeout
            );

            this._zoomIn = false;
            this._zoomOut = false;
        }
    }

    private static float Logerp(
        float a,
        float b,
        float t,
        bool inverse
    ) {
        if (inverse) {
            return b * Mathf.Pow(
                a / b,
                1 - t
            );
        } else {
            return a * Mathf.Pow(
                b / a,
                t
            );
        }
    }

    private void _scale(
        float to,
        bool up,
        double delta
    ) {
        var previousZoom = this._zoom;
        this._zoom = Logerp(
            this._zoom,
            to + (up ? 0.05f : -0.05f),
            (up ? ZoomInSpeed : ZoomOutSpeed) * (float) delta,
            !up
        );
        this._zoom = up
            ? Mathf.Min(
                this._zoom,
                to
            )
            : Mathf.Max(
                this._zoom,
                to
            );

        var zoomRatio = this._zoom / previousZoom;
        var mouse = this.GetGlobalMousePosition();
        this._zoomNode.Transform = this._zoomNode.Transform.Translated(-mouse)
            .Scaled(
                new Vector2(
                    zoomRatio,
                    zoomRatio
                )
            ).Translated(mouse);
        this._ClampPosition();
    }

    public override void _PhysicsProcess(double delta) {
        if (this._zoomIn) {
            this._scale(
                MinExtraZoom,
                false,
                delta
            );
        } else if (this._zoomOut) {
            this._scale(
                MaxExtraZoom,
                true,
                delta
            );
        } else if (this._zoom is > MaxZoom) {
            this._scale(
                MaxZoom,
                false,
                delta
            );
        } else if (this._zoom < MinZoom) {
            this._scale(
                MinZoom,
                true,
                delta
            );
        }
    }
}