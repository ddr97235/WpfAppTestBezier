using System.Windows;

namespace WpfCustomControls
{
    public readonly struct BezierSegmentData
    {
        public Point StartPoint { get; }
        public Point Point1 { get; }
        public Point Point2 { get; }
        public Point FinishPoint { get; }
        public BezierSegmentData(Point startPoint, Point point1, Point point2, Point finishPoint)
        {
            StartPoint = startPoint;
            Point1 = point1;
            Point2 = point2;
            FinishPoint = finishPoint;
        }
    }

}
