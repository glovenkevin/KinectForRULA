using System.Windows;
using System.Windows.Media;
using Microsoft.Kinect;

namespace tutorShowSkeleton
{
    interface IBodyDrawer
    {
        Brush Brush { get; set; }
        void Init(CoordinateMapper mapper, Int32Rect colourFrameSize);
        void ClearFrame();
        void DrawFrame(Body body);
    }
}
