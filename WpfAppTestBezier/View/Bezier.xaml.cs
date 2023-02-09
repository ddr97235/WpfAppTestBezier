using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfAppTestBezier.View
{
    /// <summary> Внимание, это не кривая Безье, а кривая безье с "Головой" </summary>
    public partial class Bezier : UserControl
    {
        private static DependencyProperty ShowCountPointsProperty;
        private static DependencyProperty StrokeThicknessProperty;
        private static DependencyProperty IsVisiblePointProperty;
        private static DependencyProperty HeadPointVisibledProperty;
        private static DependencyProperty _SoursePointsProperty;

        public static readonly RoutedEvent StartAnimationEvent;

        public int ShowCountPoints
        {
            get { return (int)GetValue(ShowCountPointsProperty); }
            set { SetValue(ShowCountPointsProperty, value); }
        }
        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }
        public bool IsVisiblePoint
        {
            get { return (bool)GetValue(IsVisiblePointProperty); }
            set { SetValue(IsVisiblePointProperty, value); }
        }
        public bool HeadPointVisibled
        {
            get { return (bool)GetValue(HeadPointVisibledProperty); }
            set { SetValue(HeadPointVisibledProperty, value); }
        }

        public event RoutedEventHandler StartAnimation
        {
            add { AddHandler(StartAnimationEvent, value); }
            remove { RemoveHandler(StartAnimationEvent, value); }
        }
        public List<Point>? _SoursePoints
        {
            get { return (List<Point>?)GetValue(_SoursePointsProperty); }
            set { SetValue(_SoursePointsProperty, value); }
        }
        static Bezier()
        {
            ShowCountPointsProperty = DependencyProperty.Register("ShowCountPoints", typeof(int), typeof(Bezier), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnShowCountPointsPropertyChanged)));
            StrokeThicknessProperty = DependencyProperty.Register("StrokeThickness", typeof(double), typeof(Bezier), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnStrokeThicknessChanged)));
            IsVisiblePointProperty = DependencyProperty.Register("IsVisiblePoint", typeof(bool), typeof(Bezier), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnIsVisiblePointChanged)));
            HeadPointVisibledProperty = DependencyProperty.Register("HeadPointVisibled", typeof(bool), typeof(Bezier), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnHeadPointVisibledChanged)));

            _SoursePointsProperty = DependencyProperty.Register("_SoursePoints", typeof(List<Point>), typeof(Bezier), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnSoursePointsPropertyChanged)));

            StartAnimationEvent = EventManager.RegisterRoutedEvent("StartAnimation", RoutingStrategy.Bubble,typeof(RoutedEventHandler), typeof(Bezier));
        }
        private static void OnShowCountPointsPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Bezier CurrentBezier = (Bezier)sender;
            if (CurrentBezier.soursePoints == null)
                return;
            CurrentBezier.polyBezier.Points = WpfAppTestBezier.Model.Bezier.GetBezierPointCollection(CurrentBezier.soursePoints, (int)e.NewValue);
            if (CurrentBezier.IsVisiblePoint)
            {
                CurrentBezier.point.Center = CurrentBezier.soursePoints[(int)e.NewValue-1];
            }
        }
        private static void OnStrokeThicknessChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Bezier CurrentBezier = (Bezier)sender;
            CurrentBezier.point.RadiusX = CurrentBezier.StrokeThickness * 1.5d;
            CurrentBezier.point.RadiusY = CurrentBezier.StrokeThickness * 1.5d;
        }
        private static void OnSoursePointsPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Bezier CurrentBezier = (Bezier)sender;
            var currentList = (List<Point>?)e.NewValue;
            CurrentBezier.SoursePoints = (currentList==null || currentList!.Count==0) ? null : currentList;
        }
        private static void OnIsVisiblePointChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
        }
        private static void OnHeadPointVisibledChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Bezier CurrentBezier = (Bezier)sender;
            CurrentBezier.pathPoint.Visibility = (bool)e.NewValue ? Visibility.Visible : Visibility.Collapsed;
        }
        private List<Point>? soursePoints;
        public List<Point>? SoursePoints
        {
            get=> soursePoints;
            set
            {
                soursePoints =value;
                pathPoint.Visibility = (!HeadPointVisibled || !IsVisiblePoint || soursePoints == null || soursePoints.Count == 0) ? Visibility.Collapsed : Visibility.Visible;
                if (soursePoints == null)
                {
                    polyBezier.Points=null;
                    return;
                }
                //System.Diagnostics.Stopwatch stopWatch = new();
                //stopWatch.Start();
                this.pathFigure.StartPoint = soursePoints[0];
                if (ShowCountPoints == soursePoints.Count)
                {
                    //61 точек, за 0,00002460 сек
                    polyBezier.Points = WpfAppTestBezier.Model.Bezier.GetBezierPointCollection(soursePoints, soursePoints.Count);
                    if (IsVisiblePoint)
                    {
                        point.Center = soursePoints[soursePoints.Count-1];
                    }
                }
                else
                {                  
                    ShowCountPoints = soursePoints.Count;
                }
                //stopWatch.Stop();
                //double jj = (double)stopWatch.ElapsedTicks / System.Diagnostics.Stopwatch.Frequency;
                //System.Diagnostics.Debug.WriteLine("new SoursePoints на " + SoursePoints.Count.ToString() + " точек, за " + string.Format("{0:F8}сек", jj) /*jj.ToString()*/);
            }
        }
        public void DoStartAnimation()
        {
            RoutedEventArgs args = new(StartAnimationEvent);
            RaiseEvent(args);
        }
        public Bezier()
        {
            InitializeComponent();
            StrokeThickness = 1.001;
            HeadPointVisibled = true;
            pathPoint.Visibility = Visibility.Collapsed;
        }
    }
}
