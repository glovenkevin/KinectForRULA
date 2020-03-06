using Microsoft.Kinect;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System;

namespace tutorShowSkeleton
{
  class CanvasBodyDrawer : IBodyDrawer
  {
    public CanvasBodyDrawer(Canvas canvas)
    {
      this.canvas = canvas;
      this.drawnJointElements = new Dictionary<JointType, Shape>();
      this.drawnLineElements = new List<Line>();
    }
    public Brush Brush
    {
      get;
      set;
    }
    public void Init(
      CoordinateMapper mapper,
      Int32Rect colourFrameSize)
    {
      this.canvasCoordMapper = new CanvasCoordMapper(this.canvas,
        mapper, colourFrameSize);
    }
    public void DrawFrame(Body body)
    {
      this.RemoveLinesFromPreviousFrame();

      var jointPositions = this.DrawJoints(body);

      this.DrawLines(jointPositions);
    }
    public void ClearFrame()
    {
      this.RemoveJointsFromPreviousFrame();
      this.RemoveLinesFromPreviousFrame();
    }
    void RemoveJointsFromPreviousFrame()
    {
      foreach (var item in this.drawnJointElements)
      {
        this.canvas.Children.Remove(item.Value);
      }
      this.drawnJointElements.Clear();
    }
    void RemoveLinesFromPreviousFrame()
    {
      foreach (var item in this.drawnLineElements)
      {
        this.canvas.Children.Remove(item);
      }
      this.drawnLineElements.Clear();
    }
    Dictionary<JointType, Point> DrawJoints(Body body)
    {
      Dictionary<JointType, Point> jointPositions = new Dictionary<JointType, Point>();

      foreach (var item in body.Joints)
      {
        JointType jointType = item.Key;
        Joint joint = item.Value.ScaleTo(Convert.ToInt32(canvas.ActualWidth),
            Convert.ToInt32(canvas.ActualHeight));

        //Point jointCanvasPosition = this.canvasCoordMapper.MapCameraSpacePoint(joint.Position);        
        Point jointCanvasPosition = new Point()
        {
            X = joint.Position.X,
            Y = joint.Position.Y
        };

        bool draw = IsJointForDrawing(joint, jointCanvasPosition);

        Shape shape = null;

        if (draw && !this.drawnJointElements.TryGetValue(jointType, out shape))
        {
          shape = MakeShapeForJointType(jointType);
          this.drawnJointElements[jointType] = shape;
          this.canvas.Children.Add(shape);
        }
        if (draw)
        {
          shape.Fill =
            joint.TrackingState == TrackingState.Tracked ? this.Brush : InferredBrush;

          Canvas.SetLeft(shape, jointCanvasPosition.X - (shape.Width / 2));
          Canvas.SetTop(shape, jointCanvasPosition.Y - (shape.Height / 2));

          jointPositions[jointType] = jointCanvasPosition;
        }
        else if (shape != null)
        {
          this.canvas.Children.Remove(shape);
        }
      }
      return (jointPositions);
    }
    void DrawLines(IReadOnlyDictionary<JointType,Point> jointPositions)
    {
      foreach (var jointConnection in jointConnections)
      {
        // that little data structure either contains a list of joints to work through or
        // a start joint and an element count. it's discriminated and shouldn't contain
        // both!
        jointConnection.ForEachPair(
          (j1, j2) =>
          {
            if (jointPositions.ContainsKey(j1) && jointPositions.ContainsKey(j2))
            {
              Point p1 = jointPositions[j1];
              Point p2 = jointPositions[j2];
              Line line = MakeLineForPositions(p1, p2);
              this.canvas.Children.Add(line);
              this.drawnLineElements.Add(line);
            }
          }
        );
      }
    }
    static bool IsJointForDrawing(Joint joint, Point p)
    {
      return (
          (joint.TrackingState != TrackingState.NotTracked) &&
          (!double.IsInfinity(p.X)) &&
          (!double.IsInfinity(p.Y)));
    }
    static Line MakeLineForPositions(Point p1, Point p2)
    {
      return (new Line()
      {
        X1 = p1.X,
        Y1 = p1.Y,
        X2 = p2.X,
        Y2 = p2.Y,
        Stroke = LineBrush,
        StrokeThickness = 1
      });
    }
    static Shape MakeShapeForJointType(JointType jointType)
    { 
      JointType[] leafTypes =
      {
        JointType.Head,
        JointType.FootRight,
        JointType.FootLeft
      };
      bool large = leafTypes.Contains(jointType);
      int size = large ? LargeJointEllipseDiamater : JointEllipseDiameter;

      Shape element = new Ellipse()
      {
        Width = size,
        Height = size,
        VerticalAlignment = VerticalAlignment.Center,
        HorizontalAlignment = HorizontalAlignment.Center
      };
      return (element);
    }
    Dictionary<JointType, Shape> drawnJointElements;
    List<Line> drawnLineElements;
    Canvas canvas;
    CanvasCoordMapper canvasCoordMapper;

    static readonly Brush LineBrush = Brushes.Black;
    static readonly Brush InferredBrush = Brushes.LightGray;
    static readonly int JointEllipseDiameter = 10;
    static readonly int LargeJointEllipseDiamater = 30;

    // This is bad really because it depends on the actual enum values for the JointType
    // type in the SDK not changing.
    // It's easier though than having some massive array of all the connections but that 
    // would be the right thing to do I think.
    static JointConnection[] jointConnections = 
    {
      new JointConnection(JointType.SpineBase, 2),
      new JointConnection(JointType.ShoulderLeft, 4),
      new JointConnection(JointType.ShoulderRight, 4),
      new JointConnection(JointType.HipLeft, 4),
      new JointConnection(JointType.HipRight, 4),
      new JointConnection(JointType.Neck, 2),
      new JointConnection(JointType.SpineMid, JointType.SpineShoulder, JointType.Neck),
      new JointConnection(JointType.ShoulderLeft, JointType.SpineShoulder, JointType.ShoulderRight),
      new JointConnection(JointType.HipLeft, JointType.SpineBase, JointType.HipRight),
      new JointConnection(JointType.HandTipLeft, JointType.HandLeft),
      new JointConnection(JointType.HandTipRight, JointType.HandRight),
      new JointConnection(JointType.WristLeft, JointType.ThumbLeft),
      new JointConnection(JointType.WristRight, JointType.ThumbRight)
    };
  }
}