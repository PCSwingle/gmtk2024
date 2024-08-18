using Godot;
using Resource = GMTK2024.scripts.Resource;

namespace GMTK2024.scenes;

// ReSharper disable once InconsistentNaming
public partial class VertexIO : Node2D {
    private const int Gap = 2;
    public const int MaxStorage = 30;
    private const int TransferRate = 5;

    private static VertexIO? _hoveredVertexIo;

    private static readonly PackedScene VertexIoScene =
        ResourceLoader.Load<PackedScene>("res://scenes/vertex_io.tscn");

    private static readonly Color[] FillColors =
        [new Color("005d32"), new Color("2e5933"), new Color("485230"), new Color("84292a")];

    private VertexLine? _dragLine;

    private int _lastStorage = -1;

    public Connection? Connected;
    public string DefaultLabel = "";
    public bool IsInput;
    public Vertex Parent = null!;
    public int Storage;
    public Resource? ConnectedRes { get; private set; }

    public static VertexIO CreateVertexIo(Vertex parent, string defaultLabel, bool isInput) {
        var vertexIo = VertexIoScene.Instantiate<VertexIO>();
        vertexIo.Parent = parent;
        vertexIo.Position = new Vector2(isInput ? 0 : 200, 0);
        vertexIo.IsInput = isInput;
        vertexIo.DefaultLabel = defaultLabel;
        vertexIo.Reset();
        return vertexIo;
    }

    public void Reset() {
        var label = this.GetNode<Label>("VertexIOLabel");
        label.Text = this.DefaultLabel;
        var labelSize = label.GetMinimumSize().X;
        label.Position = new Vector2(this.IsInput ? Gap : -(Gap - 2) - labelSize, -16);

        var resourceSprite = this.GetNode<Sprite2D>("VertexIOResourceSprite");
        resourceSprite.Visible = false;

        var storageSprite = this.GetNode<Sprite2D>("VertexIoStorageSprite");
        storageSprite.Visible = false;

        this.Storage = 0;
        this.ConnectedRes = null;
    }

    public void SetResource(Resource res) {
        if (res == this.ConnectedRes) {
            return;
        }

        this.ConnectedRes = res;

        var label = this.GetNode<Label>("VertexIOLabel");
        label.Text = res.Name;
        var labelSize = label.GetMinimumSize().X;
        label.Position = new Vector2(this.IsInput ? 2 * Gap + 16 : -(Gap - 1) - labelSize, -16);

        var resourceSprite = this.GetNode<Sprite2D>("VertexIOResourceSprite");
        resourceSprite.Visible = true;
        resourceSprite.Texture = res.Sprite;
        resourceSprite.Position = new Vector2(this.IsInput ? Gap + 8 : -8 - (Gap * 2 - 1) - labelSize, 0);

        var storageSprite = this.GetNode<Sprite2D>("VertexIoStorageSprite");
        storageSprite.Visible = true;
        var storagePosition =
            new Vector2(this.IsInput ? 3 * Gap - 1 + 16 + labelSize : -7 - 16 - (Gap * 3 - 1) - labelSize, -8);
        storageSprite.Position = storagePosition;
        this.Storage = 0;
        this._ModifyFillLevel();
    }

    public void SendResources() {
        if (!this.IsInput && this.Connected != null) {
            var other = this.Connected.Other(this);
            if (other.IsInput && this.ConnectedRes != null) {
                other.SetResource((Resource) this.ConnectedRes);

                var moved = Mathf.Min(this.Storage, Mathf.Min(MaxStorage - other.Storage, TransferRate));
                this.Storage -= moved;
                other.Storage += moved;
                this.Connected.Line.SetStatus(moved == 0 ? VertexLine.Status.Blocked :
                    moved < TransferRate ? VertexLine.Status.Slow : VertexLine.Status.Moving);
            }

            if (!other.IsInput) {
                this.Connected.Line.SetStatus(VertexLine.Status.Invalid);
            }
        }

        if (this.Storage != this._lastStorage) {
            this._ModifyFillLevel();
        }

        this._lastStorage = this.Storage;
    }


    private void _ModifyFillLevel() {
        var level = Mathf.Clamp(this.Storage / (float) MaxStorage, 0.126f, 1f);
        var pixelsMissing = 16 - (int) (level * 16);

        var sprite = this.GetNode<Sprite2D>("VertexIOSprite");
        var storageSprite = this.GetNode<Sprite2D>("VertexIoStorageSprite");
        storageSprite.Position = new Vector2(storageSprite.Position.X, -8 + pixelsMissing);
        storageSprite.RegionRect = new Rect2(new Vector2(storageSprite.RegionRect.Position.X, pixelsMissing),
            new Vector2(storageSprite.RegionRect.Size.X, 16 - pixelsMissing));

        var color = FillColors[pixelsMissing / 4];
        storageSprite.Modulate = color;
        sprite.Modulate = color;
    }

    public override void _Ready() {
        var control = this.GetNode<Control>("VertexIOControl");
        control.GuiInput += this._GuiInput;
        control.MouseEntered += () => { _hoveredVertexIo = this; };
        control.MouseExited += () => {
            if (_hoveredVertexIo == this) {
                _hoveredVertexIo = null;
            }
        };
    }

    public override void _UnhandledInput(InputEvent e) {
        if (this._dragLine != null && e is InputEventMouseButton { Pressed: false, ButtonIndex: MouseButton.Left }) {
            if (_hoveredVertexIo != null && _hoveredVertexIo.Parent != this.Parent) {
                this.Connected?.Delete();
                _hoveredVertexIo.Connected?.Delete();
                this.Connected = new Connection(this, _hoveredVertexIo);
            }

            this._dragLine.QueueFree();
            this._dragLine = null;
        }

        if (this._dragLine != null && e is InputEventMouseMotion me) {
            this._dragLine.Points = VertexLine.GetDragPoints(this, this.ToLocal(me.Position));
            if (_hoveredVertexIo == null) {
                this._dragLine.SetStatus(VertexLine.Status.None);
            } else if (this.IsInput == _hoveredVertexIo.IsInput) {
                this._dragLine.SetStatus(VertexLine.Status.Invalid);
            }
        }
    }

    private void _GuiInput(InputEvent e) {
        if (e is InputEventMouseButton { Pressed: true, ButtonIndex: MouseButton.Left } me &&
            this._dragLine == null) {
            this.Connected?.Delete();

            this._dragLine = VertexLine.CreateDragLine(this, me.Position);
            this.AddChild(this._dragLine);
        }
    }

    public class Connection {
        private readonly VertexIO _c1;
        private readonly VertexIO _c2;
        public readonly VertexLine Line;

        public Connection(VertexIO c1, VertexIO c2) {
            if (c1.IsInput) {
                this._c1 = c2;
                this._c2 = c1;
            } else {
                this._c1 = c1;
                this._c2 = c2;
            }

            this._c1.Connected = this;
            this._c2.Connected = this;
            this.Line = VertexLine.CreateConnectionLine(this._c1, this._c2);
            this._c1.AddChild(this.Line);
            this.Dirtyify();
        }

        private void Dirtyify() {
            if (this._c1.IsInput) {
                this._c1.Parent.MarkDirty();
            }

            if (this._c2.IsInput) {
                this._c2.Parent.MarkDirty();
            }
        }

        public void Delete() {
            // You got this GC :D
            this._c1.Connected = null;
            this._c2.Connected = null;
            this.Line.QueueFree();
            this.Dirtyify();
        }

        public void OnMove() {
            this.Line.Points = VertexLine.GetConnectionPoints(this._c1, this._c2);
            this.Line.AdjustArrows(false);
        }

        public VertexIO Other(VertexIO thisone) {
            return thisone == this._c1 ? this._c2 : this._c1;
        }
    }
}