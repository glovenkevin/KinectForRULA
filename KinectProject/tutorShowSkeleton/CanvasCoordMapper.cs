using Microsoft.Kinect;
using System.Windows;
using System.Windows.Controls;

namespace tutorShowSkeleton
{
    class CanvasCoordMapper
    {
        public CanvasCoordMapper(
          Canvas canvas,
          CoordinateMapper mapper,
          Int32Rect colourFrameSize)
        {
            this.canvas = canvas;
            this.mapper = mapper;
            this.colourFrameSize = colourFrameSize;
        }
        public Point MapCameraSpacePoint(CameraSpacePoint point)
        {
            double canvasWidth = this.canvas.Width;
            double canvasHeight = this.canvas.Height;

            var colourSpacePosition = this.mapper.MapCameraPointToDepthSpace(point);

            return (new Point()
            {
                X = (colourSpacePosition.X / colourFrameSize.Width ) * canvasWidth,
                Y = (colourSpacePosition.Y / colourFrameSize.Height ) * canvasHeight
            });
        }
        Canvas canvas;
        CoordinateMapper mapper;
        Int32Rect colourFrameSize;
    }
}
