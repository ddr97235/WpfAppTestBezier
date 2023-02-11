using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WpfAppTestBezier.ViewModel
{
    public class BezierSegmentViewModel : INotifyPropertyChanged      
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }
        public Point StartPoint { get;}
        public Point Point1 { get; }
        public Point Point2 { get; }
        public Point FinishPoint { get; }
        public BezierSegmentViewModel(Point startPoint, Point point1, Point point2, Point finishPoint)
        {
            StartPoint = startPoint;
            Point1 = point1;
            Point2 = point2;
            FinishPoint = finishPoint;
        }
    }
}
