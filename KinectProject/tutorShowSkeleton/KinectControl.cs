using Microsoft.Kinect;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using LightBuzz.Vitruvius;
using DotNetMatrix;

namespace tutorShowSkeleton
{
  class KinectControl
  {
    public KinectControl(Func<IBodyDrawer> bodyDrawerFactory, Image image, 
        TextBox textUpperArm, TextBox textLowerArm, TextBox textWristArm, TextBox textNeck, TextBox textTrunk,
        TextBlock finalScore, TextBlock finalScoreMsg)
    {
      this.bodyDrawerFactory = bodyDrawerFactory;
      this.camera = image;
      this.textUpperArm = textUpperArm;
      this.textLowerArm = textLowerArm;
      this.textWristArm = textWristArm;
      this.textNeck = textNeck;
      this.textTrunk = textTrunk;
      this.finalScore = finalScore;
      this.finalScoreMsg = finalScoreMsg;
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

    public void openReader()
    {
        this.colorReader = this.sensor.OpenMultiSourceFrameReader(FrameSourceTypes.Color | FrameSourceTypes.Depth);
        this.colorReader.MultiSourceFrameArrived += OnColorFrameArrived;
    }

    void OnColorFrameArrived(Object sender, MultiSourceFrameArrivedEventArgs e)
    {
        // Warna
        var reference = e.FrameReference.AcquireFrame();
        using (var frame = reference.ColorFrameReference.AcquireFrame())
        {
            if (frame != null)
            {
                if (String.Equals("color", tutorShowSkeleton.MainWindow._mode))
                {
                    this.camera.Source = frame.ToBitmap();
                }
            }
        }

        // Depth
        var referenceDept = e.FrameReference.AcquireFrame();
        using (var frame = referenceDept.DepthFrameReference.AcquireFrame())
        {
            if (frame != null)
            {
                if (String.Equals("depth", tutorShowSkeleton.MainWindow._mode))
                {
                    this.camera.Source = frame.ToBitmap();
                }
            }
        }
    }

    public void OpenReaderBodySkeleton()
    {
      initializeKalmanVar();
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

          frame.GetAndRefreshBodyData(this.bodies); // Sensor will reveive 6 body -> this mean for 6 people

          for (int i = 0; i < brushes.Length; i++)
          {           
            if (this.bodies[i].IsTracked) // For this testing will only get 1 body that is tracked
            {
              this.bodyDrawers[i].DrawFrame(this.bodies[i]);
              
              if (count < 30)
              {
                  rBody[count] = this.bodies[i];
              }
              else
              {
                  // Calculate R Variance every 30 Frame Body
                  //calcRVariance();

                  // Set the counter back to 0 (restart the frame tracked count)
                  // Set body hist
                  count = 0;
                  rBody[count] = this.bodies[i];
              }

              if ((count + 1) % 5 == 0 && count != 0) // Calculate every 5 frame
              {
                  // Hitung sudut dari sensor kamera RULA
                  this.calculateAngle(this.bodies[i]);
                  // Lakukan perhitungan RULA
                  this.calculateRulaEngine();
              }

              // Initialize Xk-1
              if (first)
              {
                  if (count +1 == 4)
                  {
                      for (int c = 0; c < Xk.Length; c++)
                      {
                          GeneralMatrix StateMatrix = GeneralMatrix.Random(6, 1);
                          StateMatrix.SetElement(0, 0, rBody[count].Joints[GlobalVal.BodyPart[c]].Position.X);
                          StateMatrix.SetElement(1, 0, rBody[count].Joints[GlobalVal.BodyPart[c]].Position.Y);
                          StateMatrix.SetElement(2, 0, rBody[count].Joints[GlobalVal.BodyPart[c]].Position.Z);
                          StateMatrix.SetElement(3, 0, 1);
                          StateMatrix.SetElement(4, 0, 1);
                          StateMatrix.SetElement(5, 0, 1);
                          Xk[c] = StateMatrix;
                      }
                      first = false;
                  }
              }

              // Count the frame
              count += 1;
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
        int sisiBadan = tutorShowSkeleton.MainWindow.sisiBadan;
        Joint spineShoulder = kalmanFilterFull(convertJoinToMatrix(body.Joints[JointType.SpineShoulder]),
            getBodyTypeSeq(JointType.SpineShoulder));
        Joint spineBase = kalmanFilterFull(convertJoinToMatrix(body.Joints[JointType.SpineBase]),
            getBodyTypeSeq(JointType.SpineBase));
        Vector3D trunk = convertJointoVector(spineBase) - convertJointoVector(spineShoulder);

        // Group A
        calculateUpperArm(body, trunk, sisiBadan);
        calculateLowerArm(body, sisiBadan);
        calculateWrist(body, sisiBadan);
        
        // Group B
        calculateNeck(body);
        calculateTrunk(body);
    }

    /******************* Method Angle Calc Body Part ******************/

    private void calculateUpperArm(Body body, Vector3D trunk, int sisiBadan)
    {
        Joint start, poros;
        double angle;

        if (0 == sisiBadan) // sisi Kiri
        {
            // Upper arm correspondent to sagittal Plane
            start = kalmanFilterFull(convertJoinToMatrix(body.Joints[JointType.ElbowLeft]), 
                getBodyTypeSeq(JointType.ElbowLeft));

            poros = kalmanFilterFull(convertJoinToMatrix(body.Joints[JointType.ShoulderLeft]), 
                getBodyTypeSeq(JointType.ShoulderLeft));
            
            // Calculate Angle
            angle = this.calculateAngle(poros.Position.Y, poros.Position.Z,
                start.Position.Y, start.Position.Z, -1, poros.Position.Z);
            this.textUpperArm.Text = angle.ToString("0");

            // Upper Arm Abduction
            double angleAbduction = this.calculateAngle(poros.Position.X, poros.Position.Y,
                start.Position.X, start.Position.Y, poros.Position.X, -1);
           
            RulaCalculation.calculateUpperArm(angle, angleAbduction);
        }
        else
        {
            // Lengan atas Oke
            start = kalmanFilterFull(convertJoinToMatrix(body.Joints[JointType.ElbowRight]), 
                getBodyTypeSeq(JointType.ElbowRight));

            poros = kalmanFilterFull(convertJoinToMatrix(body.Joints[JointType.ShoulderRight]), 
                getBodyTypeSeq(JointType.ShoulderRight));

            // Calculate Angle
            angle = this.calculateAngle(poros.Position.Y, poros.Position.Z,
                start.Position.Y, start.Position.Z, -1, poros.Position.Z);

            // Upper Arm Abduction
            double angleAbduction = this.calculateAngle(poros.Position.X, poros.Position.Y,
                start.Position.X, start.Position.Y, poros.Position.X, 1);

            RulaCalculation.calculateUpperArm(angle, angleAbduction);
        }
    }

    private void calculateLowerArm(Body body, int sisiBadan)
    {
        Joint start, end, poros;
        double angle;
        if (0 == sisiBadan)
        {
            // Lengan Bawah - oke
            start = kalmanFilterFull(convertJoinToMatrix(body.Joints[JointType.ShoulderLeft]), 
                getBodyTypeSeq(JointType.ShoulderLeft));

            end = kalmanFilterFull(convertJoinToMatrix(body.Joints[JointType.WristLeft]), 
                getBodyTypeSeq(JointType.WristLeft));

            poros = kalmanFilterFull(convertJoinToMatrix(body.Joints[JointType.ElbowLeft]), 
                getBodyTypeSeq(JointType.ElbowLeft));

            angle = poros.Angle(start, end) - 180;
            this.textLowerArm.Text = angle.ToString("0");

            // Cek arah lengan bwah apakah keluar dari batas midlane
            poros = start;
            start = kalmanFilterFull(convertJoinToMatrix(body.Joints[JointType.SpineShoulder]),
                getBodyTypeSeq(JointType.SpineShoulder));

            double deviasiWrist = poros.Angle(start, end) - 260; // dalam rentang -10 -> 10 masih dalam posisi tengah
            
            RulaCalculation.calculateLowerArm(angle, deviasiWrist);
        }
        else
        {
            // Lengan Bawah - oke
            start = kalmanFilterFull(convertJoinToMatrix(body.Joints[JointType.ShoulderRight]),
                getBodyTypeSeq(JointType.ShoulderRight));

            end = kalmanFilterFull(convertJoinToMatrix(body.Joints[JointType.WristRight]),
                getBodyTypeSeq(JointType.WristRight));

            poros = kalmanFilterFull(convertJoinToMatrix(body.Joints[JointType.ElbowRight]),
                getBodyTypeSeq(JointType.ElbowRight));

            angle = poros.Angle(start, end) - 180;
            this.textLowerArm.Text = angle.ToString("0");

            // Cek arah lengan bwah apakah keluar dari batas midlane
            poros = start;
            start = kalmanFilterFull(convertJoinToMatrix(body.Joints[JointType.SpineShoulder]),
                getBodyTypeSeq(JointType.SpineShoulder));

            double deviasiWrist = poros.Angle(start, end) - 260; // dalam rentang -10 -> 10 masih dalam posisi tengah
            RulaCalculation.calculateLowerArm(angle, deviasiWrist);
        }
    }

    private void calculateWrist(Body body, int sisiBadan)
    {
        Joint start, end, poros;
        double angle;
        if (0 == sisiBadan)
        {
            // Wrist Angle coresponding Horizontall plane
            start = kalmanFilterFull(convertJoinToMatrix(body.Joints[JointType.ElbowLeft]),
                getBodyTypeSeq(JointType.ElbowLeft));

            end = kalmanFilterFull(convertJoinToMatrix(body.Joints[JointType.HandTipLeft]),
                getBodyTypeSeq(JointType.HandTipLeft));

            poros = kalmanFilterFull(convertJoinToMatrix(body.Joints[JointType.WristLeft]),
                getBodyTypeSeq(JointType.HandLeft));

            angle = calculateAngle(poros.Position.X, poros.Position.Y, start.Position.X, poros.Position.Y,
                end.Position.X, end.Position.Y);
            if (angle > 100)
            {
                angle = 180 - angle;
            }

            RulaCalculation.calculateWrist(angle);
            this.textWristArm.Text = angle.ToString("0");
        }
        else // sisi Kanan
        {
            // Pergelangan Tangan - oke
            start = kalmanFilterFull(convertJoinToMatrix(body.Joints[JointType.ElbowRight]),
                getBodyTypeSeq(JointType.ElbowRight));

            end = kalmanFilterFull(convertJoinToMatrix(body.Joints[JointType.HandTipRight]),
                getBodyTypeSeq(JointType.HandTipRight));

            poros = kalmanFilterFull(convertJoinToMatrix(body.Joints[JointType.WristRight]),
                getBodyTypeSeq(JointType.WristRight));

            angle = calculateAngle(poros.Position.X, poros.Position.Y, start.Position.X, poros.Position.Y,
                end.Position.X, end.Position.Y);
            if (angle > 100)
            {
                angle = 180 - angle;
            }
            RulaCalculation.calculateWrist(angle);
            this.textWristArm.Text = angle.ToString("0");
        }
    }

    private void calculateNeck(Body body)
    {
        Joint start, end, poros;
        double angle;

        // Neck Angle corespondent to sagittal plane
        start = kalmanFilterFull(convertJoinToMatrix(body.Joints[JointType.Head]),
                getBodyTypeSeq(JointType.Head));

        end = kalmanFilterFull(convertJoinToMatrix(body.Joints[JointType.SpineShoulder]),
            getBodyTypeSeq(JointType.SpineShoulder));

        poros = kalmanFilterFull(convertJoinToMatrix(body.Joints[JointType.Neck]),
            getBodyTypeSeq(JointType.Neck));

        angle = calculateAngle(poros.Position.X, poros.Position.Y, start.Position.X, start.Position.Y,
            end.Position.X, end.Position.Y);
        if (angle < 20)
        {
            angle *= -1;
        }
        else if (angle > 20)
        {
            angle = 180 - angle;
        }

        // Neck Bending
        poros = end;
        end = kalmanFilterFull(convertJoinToMatrix(body.Joints[JointType.ShoulderLeft]),
            getBodyTypeSeq(JointType.ShoulderLeft));

        double bendingAngle = calculateAngle(poros.Position.Y, poros.Position.Z, 
            start.Position.Y, start.Position.Z, poros.Position.Y, poros.Position.Z + start.Position.Z);
        this.textUpperArm.Text = bendingAngle.ToString("0");
        RulaCalculation.calculateNeck(angle, bendingAngle);
        this.textNeck.Text = angle.ToString("0");
    }

    private void calculateTrunk(Body body)
    {
        Joint start, poros;
        double angle;

        start = kalmanFilterFull(convertJoinToMatrix(body.Joints[JointType.SpineShoulder]),
                getBodyTypeSeq(JointType.SpineShoulder));

        poros = kalmanFilterFull(convertJoinToMatrix(body.Joints[JointType.SpineMid]),
            getBodyTypeSeq(JointType.SpineMid));

        angle = this.calculateAngle(poros.Position.X, poros.Position.Z,
                start.Position.X, start.Position.Z, 3, poros.Position.Z);
        if (angle <= 90)
        {
            angle = 90 - angle;
        }
        else
        {
            angle = (angle - 90) * -1;
        }
        RulaCalculation.calculateTrunk(angle);
        this.textTrunk.Text = angle.ToString("0");
    }

      // Calculate between 2 vector 2D using Cross Join
    private double calculateAngle(double P1X, double P1Y, double P2X, double P2Y,
            double P3X, double P3Y)
    {   
        double numerator = P2Y * (P1X - P3X) + P1Y * (P3X - P2X) + P3Y * (P2X - P1X);
        double denominator = (P2X - P1X) * (P1X - P3X) + (P2Y - P1Y) * (P1Y - P3Y);
        double ratio = numerator / denominator;

        double angleRad = Math.Atan(ratio);
        double angleDeg = (angleRad * 180) / Math.PI;

        if (angleDeg < 0)
        {
            angleDeg = 180 + angleDeg;
        }

        return angleDeg;
    }

      // Set Measurement Error Base on Joint Type
    private int getBodyTypeSeq(JointType type)
    {
        int pos = 0;
        foreach (JointType t in GlobalVal.BodyPart)
        {
            if (type == t)
            {
                return pos;
            }
            pos += 1;
        }
        return -1;
    }

    private GeneralMatrix convertJoinToMatrix(Joint j)
    {
        GeneralMatrix matrix = GeneralMatrix.Identity(3,1);
        matrix.SetElement(0, 0, j.Position.X);
        matrix.SetElement(1, 0, j.Position.Y);
        matrix.SetElement(2, 0, j.Position.Z);
        return matrix;
    }

    private Joint convertMatrixTojoint(GeneralMatrix x)
    {
        Joint j = new Joint();
        j.Position.X = (float) x.GetElement(0, 0);
        j.Position.Y = (float) x.GetElement(1, 0);
        j.Position.Z = (float) x.GetElement(2, 0);

        return j;
    }

    private Vector3D convertJointoVector(Joint j)
    {
        Vector3D v = new Vector3D();
        v.X = j.Position.X;
        v.Y = j.Position.Y;
        v.Z = j.Position.Z;
        return v;
    }

    private void calculateRulaEngine()
    {
        // Hitung nilai keseluruhan
        finalResult = RulaCalculation.calculateRula();
        finalMsg = RulaCalculation.getFinalScoreMsg(finalResult);

        // Set Ke TexBlocknya
        this.finalScore.Text = finalResult.ToString();
        // Set pewarnaan logo nya
        if (finalResult <= 2)
        {
            this.finalScore.Foreground = Brushes.Green;
        }
        else if (finalResult > 2 && finalResult <= 4)
        {
            this.finalScore.Foreground = Brushes.GreenYellow;
        }
        else if (finalResult > 4 && finalResult <= 7)
        {
            this.finalScore.Foreground = Brushes.Yellow;
        }
        else
        {
            this.finalScore.Foreground = Brushes.Red;
        }

        this.finalScoreMsg.Text = finalMsg;
    }
      


    /******************* Method Kalman Filter ******************/

    // Variable A,B, and H we will asumtion this is a constant value
    // Most probably is 1.
    private Joint kalmanFilter(GeneralMatrix z, int pos)
    {
        // Prepare 
        Joint join = new Joint();
        GeneralMatrix r = GeneralMatrix.Identity(3, 3);
        r.SetElement(0, 0, Rv[pos, 0]); // Variance Base on Joint Type , X
        r.SetElement(1, 1, Rv[pos, 1]); // Variance Base on Joint Type , Z
        r.SetElement(2, 2, Rv[pos, 2]); // Variance Base on Joint Type , Y
        R = r;

        // Predict
        GeneralMatrix Xp = Xk[pos];
        GeneralMatrix Pp = P[pos];

        // Measurement Update (correction
        GeneralMatrix s = Pp + R;
        K = Pp * s.Inverse();        // Calculate Kalman Gain
        Xk[pos] = Xp + ( K * (z - Xp) );
        GeneralMatrix I = GeneralMatrix.Identity(Pp.RowDimension, Pp.ColumnDimension);
        P[pos] = (I - K) * Pp;

        estimationX = Xk[pos].GetElement(0, 0);
        estimationY = Xk[pos].GetElement(1, 0);
        estimationZ = Xk[pos].GetElement(2, 0);

        join.Position.X = (float) estimationX;
        join.Position.Y = (float) estimationY;
        join.Position.Z = (float) estimationZ;

        return join;
    }

    // Here define A,B, and H Matrice
    private Joint kalmanFilterFull(GeneralMatrix z, int pos)
    {
        // Prepare 
        GeneralMatrix r = GeneralMatrix.Identity(3, 3);
        R = r.MultiplyEquals(Rv[pos, 0]);

        // Predict
        GeneralMatrix Xp = F * Xk[pos];
        GeneralMatrix Pp = F * P[pos] * F.Transpose() + Q;

        // Measurement update ( Correction )
        K = Pp * H.Transpose() * (H * Pp * H.Transpose() + R).Inverse();
        Xk[pos] = Xp + (K * (z - (H * Xp)));
        
        GeneralMatrix I = GeneralMatrix.Identity(Pp.RowDimension, Pp.ColumnDimension);
        P[pos] = (I - (K * H)) * Pp;

        return convertMatrixTojoint(Xk[pos]);
    }

    /***********************************************************/

    Body[] bodies;
    KinectSensor sensor;
    BodyFrameReader reader;
    IBodyDrawer[] bodyDrawers;
    Func<IBodyDrawer> bodyDrawerFactory;
    Image camera;
    MultiSourceFrameReader colorReader;
    
    // FinalScore Item
    int finalResult;
    String finalMsg;

    // Kinect Filter (Default Smoothing vector for minimalize Jitter on Kinect)
    KinectJointFilter filter = new KinectJointFilter();

    //------------------------------------------
    // Kalman variabel
    //-------------------------------------------

    GeneralMatrix F, H, Q, R, K;
    GeneralMatrix[] P, Xk;

    double dt = 30; // 30 Frame
    double[] estimationVector = new double[6];
    double[,] Rv = new double[GlobalVal.BodyPart.Length, 3];   // 3 -> x,y,z
    double estimationX, estimationY, estimationZ;
    int count = 0;
    Body[] rBody = new Body[30]; // Save Body every frame for calculating Measurement Variance
    bool first = true;

    // Inisiasi Variabel
    private void initializeKalmanVar()
    {
        // Prepare 
        int length = GlobalVal.BodyPart.Length;
        P = new GeneralMatrix[length];
        Xk = new GeneralMatrix[length];
        GeneralMatrix i = GeneralMatrix.Identity(6, 6);

        // Inisialisasi matrik fungsi transisi (F / A)
        double[][] matrikA = { new double[]  {1,   0,   0,    dt,  0,   0}, 
                                new double[] {0,   1,   0,    0,   dt,  0}, 
                                new double[] {0,   0,   1,    0,   0,   dt}, 
                                new double[] {0,   0,   0,    1,   0,   0}, 
                                new double[] {0,   0,   0,    0,   1,   0}, 
                                new double[] {0,   0,   0,    0,   0,   1}};
        F = new GeneralMatrix(matrikA); // Initialize A variable

        // Initialize State (H)
        H = GeneralMatrix.Random(3, 6);
        H.SetElement(0, 0, 1); H.SetElement(0, 1, 0); H.SetElement(0, 2, 0); H.SetElement(0, 3, 0); H.SetElement(0, 4, 0); H.SetElement(0, 5, 0);
        H.SetElement(1, 0, 0); H.SetElement(1, 1, 1); H.SetElement(1, 2, 0); H.SetElement(1, 3, 0); H.SetElement(1, 4, 0); H.SetElement(1, 5, 0);
        H.SetElement(2, 0, 0); H.SetElement(2, 1, 0); H.SetElement(2, 2, 1); H.SetElement(2, 3, 0); H.SetElement(2, 4, 0); H.SetElement(2, 5, 0);

        // Initialize Process Noise (Q) -> ini bisa diabaikan
        // Sedikit lebih sulit karena harus dipas"kan berdasarkan pengujian serta proses yang berlangsung
        GeneralMatrix Qq = GeneralMatrix.Identity(6, 6);
        Q = Qq.MultiplyEquals(0.1);

        for (int c = 0; c < length; c++)
        {
            // Prior error covariance matrix (P)
            P[c] = i;

            // Initialize Rv -> init: 0.01
            Rv[c, 0] = 10;
            Rv[c, 1] = 10;
            Rv[c, 2] = 10;
        }

    }

      // Calculate Rv[] Variance for every JointType
      // This methods come from Erick Pranata Thesis (2016)
    private void calcRVariance()
    {
        // Remove Rv existing value
        Rv = new double[GlobalVal.BodyPart.Length, 3];

        double[] sumX = new double[GlobalVal.BodyPart.Length];
        double[] sumY = new double[GlobalVal.BodyPart.Length];
        double[] sumZ = new double[GlobalVal.BodyPart.Length];

        int counter = 0;
        double totalData = rBody.Length;
        foreach (JointType bodyPart in GlobalVal.BodyPart)
        {
            // Get Sum
            foreach (Body body in rBody) // 30 Frame
            {
                if (null == body)
                {
                    totalData -= 1;
                    continue;
                }
                sumX[counter] += body.Joints[bodyPart].Position.X;
                sumY[counter] += body.Joints[bodyPart].Position.Y;
                sumZ[counter] += body.Joints[bodyPart].Position.Z;
            }
            counter += 1;
        }

        // Get mean for every x,y,z in BodyPart
        for (int i = 0; i < GlobalVal.BodyPart.Length; i++)
        {
            sumX[i] = sumX[i] / totalData;
            sumY[i] = sumY[i] / totalData;
            sumZ[i] = sumZ[i] / totalData;
        }

        // Calculate Variance
        counter = 0;
        foreach (JointType bodyPart in GlobalVal.BodyPart)
        {
            // Get Σ[X¯ - Xi]^2
            foreach (Body body in rBody) // 30 Frame
            {
                if (null == body) continue;

                Rv[counter, 0] += Math.Pow(sumX[counter] - (double) body.Joints[bodyPart].Position.X, 2);
                Rv[counter, 1] += Math.Pow(sumY[counter] - (double) body.Joints[bodyPart].Position.Y, 2);
                Rv[counter, 2] += Math.Pow(sumZ[counter] - (double) body.Joints[bodyPart].Position.Z, 2);
            }

            Rv[counter, 0] = Math.Sqrt(Rv[counter, 0] / (totalData - 1)); // x
            Rv[counter, 1] = Math.Sqrt(Rv[counter, 1] / (totalData - 1)); // y
            Rv[counter, 2] = Math.Sqrt(Rv[counter, 2] / (totalData - 1)); // z
            counter += 1;
        }

        counter = 0;
    }

    /******************************************************/

    //Text box and Information collumn
    TextBox textUpperArm;
    TextBox textLowerArm;
    TextBox textWristArm;
    TextBox textNeck;
    TextBox textTrunk;
    TextBlock finalScore;
    TextBlock finalScoreMsg;

      // 6 Color -> 6 Body
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