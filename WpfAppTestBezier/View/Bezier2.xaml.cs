using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.Pkcs;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WpfAppTestBezier.ViewModel;

namespace WpfAppTestBezier.View
{
    /// <summary> Внимание, это не кривая Безье, а кривая безье с "Головой" </summary>
    public partial class Bezier2 : UserControl
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
        public ObservableCollection<Point>? _SoursePoints
        {
            get { return (ObservableCollection<Point>?)GetValue(_SoursePointsProperty); }
            set { SetValue(_SoursePointsProperty, value); }
        }
        static Bezier2()
        {
            ShowCountPointsProperty = DependencyProperty.Register("ShowCountPoints", typeof(int), typeof(Bezier2), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnShowCountPointsPropertyChanged)));
            StrokeThicknessProperty = DependencyProperty.Register("StrokeThickness", typeof(double), typeof(Bezier2), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnStrokeThicknessChanged)));
            IsVisiblePointProperty = DependencyProperty.Register("IsVisiblePoint", typeof(bool), typeof(Bezier2), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnIsVisiblePointChanged)));
            HeadPointVisibledProperty = DependencyProperty.Register("HeadPointVisibled", typeof(bool), typeof(Bezier2), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnHeadPointVisibledChanged)));

            _SoursePointsProperty = DependencyProperty.Register("_SoursePoints", typeof(ObservableCollection<Point>), typeof(Bezier2), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnSoursePointsPropertyChanged)));

            StartAnimationEvent = EventManager.RegisterRoutedEvent("StartAnimation", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Bezier2));
        }
        private static void OnShowCountPointsPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Bezier CurrentBezier = (Bezier)sender;
            if (CurrentBezier._SoursePoints == null)
                return;
            CurrentBezier.polyBezier.Points = WpfAppTestBezier.Model.Bezier.GetBezierPointCollection(CurrentBezier._SoursePoints, (int)e.NewValue);
            if (CurrentBezier.IsVisiblePoint)
            {
                CurrentBezier.point.Center = CurrentBezier._SoursePoints[(int)e.NewValue - 1];
            }
        }
        private static void OnStrokeThicknessChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Bezier2 CurrentBezier = (Bezier2)sender;
            CurrentBezier.point.RadiusX = CurrentBezier.StrokeThickness * 1.5d;
            CurrentBezier.point.RadiusY = CurrentBezier.StrokeThickness * 1.5d;
        }
        private static void OnSoursePointsPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Bezier2 CurrentBezier = (Bezier2)sender;            
            var oldCollection = (ObservableCollection<Point>?)e.OldValue;
            if(oldCollection!=null)
            {
                oldCollection.CollectionChanged -= CurrentBezier.OnCollectionChanged;
            }
            if (CurrentBezier._SoursePoints != null)
            {
                CurrentBezier._SoursePoints.CollectionChanged += CurrentBezier.OnCollectionChanged;
            }
        }
        
        private void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            fullPointsCollections = Model.Bezier.GetBezierPointCollection(_SoursePoints, _SoursePoints.Count).ToList<Point>();
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add: // если добавление
                    if(e.NewItems.Count>1)
                    {
                        Rebuild();
                    }
                    else
                    {
                        AddBezierSegment(e.NewStartingIndex - 1);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove: // если удаление
                    if (e.OldItems.Count > 1)
                    {
                        Rebuild();
                    }
                    else
                    {
                        DeleteBezierSegment(e.OldStartingIndex);
                    }
                    break;
                case NotifyCollectionChangedAction.Replace: // если замена
                    Rebuild();
                    break;
                case NotifyCollectionChangedAction.Reset: // если сильно всё изменилось.
                    Rebuild();
                    break;
            }
            pathPoint.Visibility = (!HeadPointVisibled || !IsVisiblePoint || _SoursePoints == null || _SoursePoints.Count == 0) ? Visibility.Collapsed : Visibility.Visible;
            if (IsVisiblePoint)
            {
                point.Center = _SoursePoints[_SoursePoints.Count - 1];
            }
        }
        private static void OnIsVisiblePointChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
        }
        private static void OnHeadPointVisibledChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Bezier2 CurrentBezier = (Bezier2)sender;
            CurrentBezier.pathPoint.Visibility = (bool)e.NewValue ? Visibility.Visible : Visibility.Collapsed;
        }
        public void DoStartAnimation()
        {
            RoutedEventArgs args = new(StartAnimationEvent);
            RaiseEvent(args);
        }
        public Bezier2()
        {
            BezierSegments = new();
            this.DataContextChanged += Bezier2_DataContextChanged;
            //BezierSegments.Add(new(new(0,0),new(0,0),new(0,0),new(0,0)));
            InitializeComponent();
            ItemsBezierSegment.ItemsSource = BezierSegments;
            StrokeThickness = 1.001;
            HeadPointVisibled = true;
            pathPoint.Visibility = Visibility.Collapsed;            
        }

        private void Bezier2_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            // throw new NotImplementedException();
            ItemsBezierSegment.DataContext= DataContext;
        }

        private void Rebuild()
        {
            BezierSegments.Clear();            
            if (_SoursePoints==null)
            {
                return;
            }
            for(int i=0; i< _SoursePoints.Count-2;i++)
            {
                AddBezierSegment(i);
            }
        }
        private void AddBezierSegment(int startPointIndex)
        {
            if (_SoursePoints.Count >= 3 && startPointIndex== _SoursePoints.Count-2)
            {
                DeleteBezierSegment(startPointIndex-1);
                BezierSegments.Add(new(_SoursePoints[startPointIndex-1], fullPointsCollections[(startPointIndex-1) * 3], fullPointsCollections[(startPointIndex-1) * 3 + 1], _SoursePoints[startPointIndex]));
            }

            if (_SoursePoints.Count > 1)
            {
                BezierSegments.Add(new(_SoursePoints[startPointIndex], fullPointsCollections[startPointIndex * 3], fullPointsCollections[startPointIndex * 3 + 1], _SoursePoints[startPointIndex + 1]));
            }
            //GC.Collect();
        }
        private void DeleteBezierSegment(int startPointIndex) => BezierSegments.RemoveAt(startPointIndex);
        public ObservableCollection<BezierSegmentViewModel> BezierSegments { get; private set; } = new(new());
        private List<Point> fullPointsCollections =new();
    }    
}
