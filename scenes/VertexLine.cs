using System.Collections.Generic;
using Godot;

namespace GMTK2024.scenes;

public partial class VertexLine : Line2D {
    public enum Status {
        None,
        Invalid,
        Blocked,
        Slow,
        Moving
    }

    private const float ProtrusionLength = 25f;
    private const float MoveTime = 0.03f;
    private const int StepsPerArrow = 15;

    public static readonly Color RegularColor = new("272727");
    public static readonly Color InvalidColor = new("770001");
    public static readonly Color BlockedColor = new("754e22");
    public static readonly Color MovingColor = new("0e4d00");

    private static readonly PackedScene VertexLineScene =
        ResourceLoader.Load<PackedScene>("res://scenes/vertex_line.tscn");

    private static readonly PackedScene ConnectionArrowScene =
        ResourceLoader.Load<PackedScene>("res://scenes/connection_arrow.tscn");

    private readonly HashSet<ConnectionArrow> _arrows = new();
    private int _curSteps = StepsPerArrow - 1;

    private Status _status = Status.None;

    private float _tillMove;


    public static VertexLine CreateDragLine(
        VertexIO c1,
        Vector2 endPoint
    ) {
        var line = VertexLineScene.Instantiate<VertexLine>();
        line.Points = GetDragPoints(
            c1,
            endPoint
        );
        return line;
    }

    public static Vector2[] GetDragPoints(
        VertexIO c1,
        Vector2 endPoint
    ) {
        var curve = new Curve2D();
        curve.AddPoint(
            Vector2.Zero,
            Vector2.Zero,
            new Vector2(
                endPoint.X / 2,
                0
            )
        );
        curve.AddPoint(
            endPoint,
            new Vector2(
                -endPoint.X / 2,
                0
            )
        );
        return curve.GetBakedPoints();
    }

    public static VertexLine CreateConnectionLine(
        VertexIO c1,
        VertexIO c2
    ) {
        var line = VertexLineScene.Instantiate<VertexLine>();
        line.Points = GetConnectionPoints(
            c1,
            c2
        );
        if (c1.IsInput == c2.IsInput) {
            line.SetStatus(Status.Invalid);
        }

        return line;
    }

    public static Vector2[] GetConnectionPoints(
        VertexIO c1,
        VertexIO c2
    ) {
        var c2Loc = c1.ToLocal(c2.GlobalPosition);
        return GetDragPoints(
            c1,
            c2Loc
        );
    }

    private void _AddArrow(int index) {
        var arrow = ConnectionArrowScene.Instantiate<ConnectionArrow>();
        arrow.Position = this.Points[index];
        arrow.pointsIndex = index;
        this.GetNode<Node2D>("ArrowContainer").AddChild(arrow);
        this._arrows.Add(arrow);
    }

    public override void _Ready() {
        if (this._status is Status.None) {
            this.SelfModulate = RegularColor;
        }
    }

    public void AdjustArrows(bool move) {
        foreach (var arrow in this._arrows) {
            if (move) {
                arrow.pointsIndex += 1;
            }

            if (arrow.pointsIndex >= this.Points.Length - 1) {
                arrow.QueueFree();
                this._arrows.Remove(arrow);
            } else {
                arrow.Position = this.Points[arrow.pointsIndex];
                var dir = this.Points[arrow.pointsIndex + 1] - this.Points[arrow.pointsIndex];
                arrow.Rotation = dir.Angle();
            }
        }
    }

    public override void _Process(double delta) {
        if (this._status is Status.Moving) {
            this._tillMove += (float) delta * (int) Timebar.CurTime;
        } else if (this._status is Status.Slow) {
            this._tillMove += (float) delta * (int) Timebar.CurTime / 2;
        }

        if (this._tillMove > MoveTime) {
            this._tillMove = Mathf.Min(
                this._tillMove - MoveTime,
                MoveTime
            );
            this.AdjustArrows(true);

            this._curSteps += 1;
            if (this._curSteps == StepsPerArrow) {
                this._AddArrow(0);
                this._curSteps = 0;
            }
        }
    }


    public void SetStatus(Status status) {
        if (this._status == status) {
            return;
        }

        this._status = status;

        if (status == Status.Invalid) {
            this.Modulate = InvalidColor;
            this.SelfModulate = Colors.White;
        } else {
            this.Modulate = Colors.White;
            this.SelfModulate = RegularColor;
        }

        var arrowContainer = this.GetNode<Node2D>("ArrowContainer");
        if (status == Status.Blocked) {
            arrowContainer.Modulate = BlockedColor;
        } else if (status == Status.Moving) {
            arrowContainer.Modulate = MovingColor;
        } else if (status == Status.Slow) {
            arrowContainer.Modulate = BlockedColor;
        } else {
            arrowContainer.Modulate = Colors.White;
        }
    }
}