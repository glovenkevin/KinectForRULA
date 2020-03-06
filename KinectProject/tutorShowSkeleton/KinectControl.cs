using Microsoft.Kinect;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using LightBuzz.Vitruvius;

namespace tutorShowSkeleton
{
  class KinectControl
  {
    public KinectControl(Func<IBodyDrawer> bodyDrawerFactory, Image image, 
        TextBox textUpperArm, TextBox textLowerArm, TextBox textWristArm, TextBox textNeck, TextBox textTrunk)
    {
      this.bodyDrawerFactory = bodyDrawerFactory;
      this.camera = image;
      this.textUpperArm = textUpperArm;
      this.textLowerArm = textLowerArm;
      this.textWristArm = textWristArm;
      this.textNeck = textNeck;
      this.textTrunk = textTrunk;
    }

    public void GetSensor(Canvas canvas)
    {
      this.sensor = KinectSensor.GetDefault();
      this.sensor.Open();

      Int32Rect colorFrameSize = new Int32Rect()
      {
        Width = this.sensor.DepthFrameSource.FrameDescription.Width ,
        Height = this.sensor.DepthFrameSource.FrameDescription.Height
      };
      this.bodyDrawers = new IBodyDrawer[brushes.Length];

      for (int i = 0; i < brushes.Length; i++)
      {
        this.bodyDrawers[i] = bodyDrawerFactory();
        this.bodyDrawers[i].Brush = brushes[i];
        this.bodyDrawers[i].Init(this.sensor.CoordinateMapper, colorFrameSize);
      }
    }

    public void openColorReader()
    {
        this.colorReader = this.sensor.OpenMultiSourceFrameReader(FrameSourceTypes.Color);
        this.colorReader.MultiSourceFrameArrived += OnColorFrameArrived;
    }

    void OnColorFrameArrived(Object sender, MultiSourceFrameArrivedEventArgs e)
    {
        var reference = e.FrameReference.AcquireFrame();
        using (var frame = reference.ColorFrameReference.AcquireFrame())
        {
            if (frame != null)
            {
                this.camera.Source = frame.ToBitmap();
            }
        }
    }

    public void OpenReaderBodySkeleton()
    {
      this.reader = this.sensor.BodyFrameSource.OpenReader();
      this.reader.FrameArrived += OnFrameArrived;
    }
    public void CloseReader()
    {
      this.reader.Dispose();
      this.colorReader.Dispose();
    }
    void OnFrameArrived(object sender, BodyFrameArrivedEventArgs e)
    {
      using (var frame = e.FrameReference.AcquireFrame())
      {
        if ((frame != null) && (frame.BodyCount > 0))
        {
          if ((this.bodies == null) || (this.bodies.Length != frame.BodyCount))
          {
            this.bodies = new Body[frame.BodyCount];
          }
          frame.GetAndRefreshBodyData(this.bodies);

          for (int i = 0; i < brushes.Length; i++)
          {           
            if (this.bodies[i].IsTracked)
            {
              this.bodyDrawers[i].DrawFrame(this.bodies[i]);
              this.calculateAngle(this.bodies[i]);
            }
            else
            {
              this.bodyDrawers[i].ClearFrame();
            }
          }
        }
      }
    }
    public void ReleaseSensor()
    {
      this.sensor.Close();
      this.sensor = null;
    }
        
    public void calculateAngle(Body body)
    {
            // Lengan atas - dibawah 40 - 50
            Joint start = body.Joints[JointType.SpineMid];

            Joint batangAtas = body.Joints[JointType.SpineShoulder];
            Joint batangBawah = body.Joints[JointType.SpineBase];
            Joint end = new Joint();
            end.Position.X = batangAtas.Position.X - batangBawah.Position.X;
            end.Position.Y = batangAtas.Position.Y - batangBawah.Position.Y;
            end.Position.Z = batangAtas.Position.Z - batangBawah.Position.Z;

            Joint poros = body.Joints[JointType.ElbowLeft];
            double angle = poros.Angle(start, end) - 90;
            this.textUpperArm.Text = angle.ToString("0");

            // Lengan Bawah - oke
            start = body.Joints[JointType.ShoulderLeft];
            end = body.Joints[JointType.WristLeft];
            poros = body.Joints[JointType.ElbowLeft];
            angle = poros.Angle(start, end) - 180;
            this.textLowerArm.Text = angle.ToString("0");

            // Pergelangan Tangan - oke
            start = body.Joints[JointType.ElbowLeft];
            end = body.Joints[JointType.HandLeft];
            poros = body.Joints[JointType.WristLeft];
            angle = poros.Angle(start, end) - 180;
            this.textWristArm.Text = angle.ToString("0");

            // Leher - oke 
            start = body.Joints[JointType.Head];
            end = body.Joints[JointType.SpineShoulder];
            poros = body.Joints[JointType.Neck];
            angle = poros.Angle(start, end) - 180;
            this.textNeck.Text = angle.ToString("0");
            
            // Batang tubuh
            start = body.Joints[JointType.SpineShoulder];
            end = body.Joints[JointType.SpineBase];
            poros = body.Joints[JointType.SpineMid];
            angle = poros.Angle(start, end) - 180;
            this.textTrunk.Text = angle.ToString("0");
    }

    Body[] bodies;
    KinectSensor sensor;
    BodyFrameReader reader;
    IBodyDrawer[] bodyDrawers;
    Func<IBodyDrawer> bodyDrawerFactory;
    Image camera;
    MultiSourceFrameReader colorReader;

    //Text box untuk badan kiri
    TextBox textUpperArm;
    TextBox textLowerArm;
    TextBox textWristArm;
    TextBox textNeck;
    TextBox textTrunk;

    static Brush[] brushes = 
    {
      Brushes.Green,
      Brushes.Blue,
      Brushes.Red,
      Brushes.Orange,
      Brushes.Purple,
      Brushes.Yellow
    };
  }
}