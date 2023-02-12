using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;

namespace WpfCustomControls
{
    public class CustomBezier2 : Control
    {
        static CustomBezier2()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomBezier2), new FrameworkPropertyMetadata(typeof(CustomBezier2)));
        }

        //private static DependencyProperty ShowCountPointsProperty;


        //public int ShowCountPoints
        //{
        //    get { return (int)GetValue(ShowCountPointsProperty); }
        //    set { SetValue(ShowCountPointsProperty, value); }
        //}
        public double StrokeThickness
        {
            get => (double)GetValue(StrokeThicknessProperty);
            set => SetValue(StrokeThicknessProperty, value);
        }
        public readonly static DependencyProperty StrokeThicknessProperty = DependencyProperty.Register(
            nameof(StrokeThickness),
            typeof(double),
            typeof(CustomBezier2),
            new FrameworkPropertyMetadata(0.0, OnStrokeThicknessChanged));

        public bool IsVisiblePoint
        {
            get => (bool)GetValue(IsVisiblePointProperty);
            set => SetValue(IsVisiblePointProperty, value);
        }
        public readonly static DependencyProperty IsVisiblePointProperty = DependencyProperty.Register(
            nameof(IsVisiblePoint),
            typeof(bool),
            typeof(CustomBezier2),
            new FrameworkPropertyMetadata(false, OnIsVisiblePointChanged));

        public bool HeadPointVisibled
        {
            get => (bool)GetValue(HeadPointVisibledProperty);
            set => SetValue(HeadPointVisibledProperty, value);
        }
        public readonly static DependencyProperty HeadPointVisibledProperty = DependencyProperty.Register(
            nameof(HeadPointVisibled),
            typeof(bool),
            typeof(CustomBezier2),
            new FrameworkPropertyMetadata(false, OnHeadPointVisibledChanged));

        public event RoutedEventHandler StartAnimation
        {
            add { AddHandler(StartAnimationEvent, value); }
            remove { RemoveHandler(StartAnimationEvent, value); }
        }
        public static readonly RoutedEvent StartAnimationEvent = EventManager.RegisterRoutedEvent(
            nameof(StartAnimation),
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(CustomBezier2));

        public ObservableCollection<Point>? _SoursePoints
        {
            get => (ObservableCollection<Point>?)GetValue(_SoursePointsProperty);
            set => SetValue(_SoursePointsProperty, value);
        }
        public readonly static DependencyProperty _SoursePointsProperty = DependencyProperty.Register(
            nameof(_SoursePoints),
            typeof(ObservableCollection<Point>),
            typeof(CustomBezier2),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(OnSoursePointsPropertyChanged)));

        //static Bezier2()
        //{
        //    //ShowCountPointsProperty = DependencyProperty.Register("ShowCountPoints", typeof(int), typeof(Bezier2), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnShowCountPointsPropertyChanged)));
        //}
        //private static void OnShowCountPointsPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        //{
        //    Bezier2 CurrentBezier = (Bezier2)sender;
        //    if (CurrentBezier._SoursePoints == null)
        //        return;
        //    CurrentBezier.polyBezier.Points = WpfAppTestBezier.Model.Bezier.GetBezierPointCollection(CurrentBezier._SoursePoints, (int)e.NewValue);
        //    if (CurrentBezier.IsVisiblePoint)
        //    {
        //        CurrentBezier.point.Center = CurrentBezier._SoursePoints[(int)e.NewValue - 1];
        //    }
        //}



        public double PointDiameter
        {
            get => (double)GetValue(PointDiameterProperty);
            private set => SetValue(PointDiameterPropertyKey, value);
        }

        // Using a DependencyProperty as the backing store for PointDiameter.  This enables animation, styling, binding, etc...
        private static readonly DependencyPropertyKey PointDiameterPropertyKey =
            DependencyProperty.RegisterReadOnly(
                nameof(PointDiameter),
                typeof(double),
                typeof(CustomBezier2),
                new PropertyMetadata(0.0));
        public static readonly DependencyProperty PointDiameterProperty = PointDiameterPropertyKey.DependencyProperty;


        private static void OnStrokeThicknessChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            CustomBezier2 CurrentBezier = (CustomBezier2)sender;
            //CurrentBezier.point.RadiusX = CurrentBezier.StrokeThickness * 1.5d;
            //CurrentBezier.point.RadiusY = CurrentBezier.StrokeThickness * 1.5d;

            CurrentBezier.PointDiameter = CurrentBezier.StrokeThickness * 3.0;
        }
        private static void OnSoursePointsPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            CustomBezier2 CurrentBezier = (CustomBezier2)sender;
            var old = (ObservableCollection<Point>?)e.OldValue;
            if (old is not null)
            {
                old.CollectionChanged -= CurrentBezier.OnSourcePointsChanged;
            }
            var @new = (ObservableCollection<Point>?)e.NewValue;
            if (@new is not null)
            {
                @new.CollectionChanged += CurrentBezier.OnSourcePointsChanged;
            }
            CurrentBezier.private_SoursePoints = @new;
            CurrentBezier.OnSourcePointsChanged(@new, new(NotifyCollectionChangedAction.Reset));
        }
        public Func<IList<Point>?, int, List<Point>> PointsFunc
        {
            get => (Func<IList<Point>?, int, List<Point>>)GetValue(PointsFuncProperty);
            set => SetValue(PointsFuncProperty, value);
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PointsFuncProperty =
            DependencyProperty.Register(
                nameof(PointsFunc),
                typeof(Func<IList<Point>?, int, List<Point>>),
                typeof(CustomBezier2),
                new PropertyMetadata(new Func<IList<Point>?, int, List<Point>>((_, _) => new List<Point>()), OnPointsFuncChanged, CoercePointsFunc));

        private static object CoercePointsFunc(DependencyObject sender, object baseValue)
        {
            CustomBezier2 CurrentBezier = (CustomBezier2)sender;
            return baseValue ?? new Func<IList<Point>?, int, List<Point>>((_, _) => new List<Point>());
        }

        private static void OnPointsFuncChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            CustomBezier2 CurrentBezier = (CustomBezier2)sender;
            Func<IList<Point>?, int, List<Point>> func = (Func<IList<Point>?, int, List<Point>>)e.NewValue;
            CurrentBezier.privatePointsFunc = func;
            CurrentBezier.fullPointsCollections = func(CurrentBezier.private_SoursePoints, CurrentBezier.private_SoursePoints?.Count ?? -1);
            CurrentBezier.Rebuild();
        }
        private Func<IList<Point>?, int, List<Point>> privatePointsFunc = new((_, _) => new List<Point>());

        private void OnSourcePointsChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            fullPointsCollections = privatePointsFunc(private_SoursePoints, private_SoursePoints?.Count ?? -1); /*Model.Bezier.GetBezierPointCollection(_SoursePoints, _SoursePoints.Count).ToList<Point>();*/

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add: // если добавление
                    if (e.NewItems?.Count > 1)
                    {
                        Rebuild();
                    }
                    else
                    {
                        AddBezierSegment(e.NewStartingIndex - 1);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove: // если удаление
                    if (e.OldItems?.Count > 1)
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
            PathPointVisibility = (!HeadPointVisibled || !IsVisiblePoint || private_SoursePoints == null || private_SoursePoints?.Count == 0) ? Visibility.Collapsed : Visibility.Visible;
            if (IsVisiblePoint)
            {
                CenterPoint = private_SoursePoints?[^1] ?? new Point();
            }
        }



        public Visibility PathPointVisibility
        {
            get { return (Visibility)GetValue(PathPointVisibilityProperty); }
            private set { SetValue(PethPointVisibilityPropertyKey, value); }
        }

        // Using a DependencyProperty as the backing store for PethPointVisibility.  This enables animation, styling, binding, etc...
        private static readonly DependencyPropertyKey PethPointVisibilityPropertyKey =
            DependencyProperty.RegisterReadOnly(
                nameof(PathPointVisibility),
                typeof(Visibility),
                typeof(CustomBezier2),
                new PropertyMetadata(Visibility.Collapsed));
        public static readonly DependencyProperty PathPointVisibilityProperty = PethPointVisibilityPropertyKey.DependencyProperty;



        public Point CenterPoint
        {
            get => (Point)GetValue(CenterPointProperty);
            private set => SetValue(CenterPointPropertyKey, value);
        }

        // Using a DependencyProperty as the backing store for CenterPoint.  This enables animation, styling, binding, etc...
        private static readonly DependencyPropertyKey CenterPointPropertyKey =
            DependencyProperty.RegisterReadOnly(
                nameof(CenterPoint),
                typeof(Point),
                typeof(CustomBezier2),
                new PropertyMetadata(new Point()));
        public static readonly DependencyProperty CenterPointProperty = CenterPointPropertyKey.DependencyProperty;



        private static void OnIsVisiblePointChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
        }
        private static void OnHeadPointVisibledChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            CustomBezier2 CurrentBezier = (CustomBezier2)sender;
            CurrentBezier.PathPointVisibility = (bool)e.NewValue ? Visibility.Visible : Visibility.Collapsed;
        }
        public void DoStartAnimation()
        {
            RoutedEventArgs args = new(StartAnimationEvent);
            RaiseEvent(args);
        }
        public CustomBezier2()
        {
            privateBezierSegments = new();
            BezierSegments = new(privateBezierSegments);
            //this.DataContextChanged += Bezier2_DataContextChanged;
            //BezierSegments.Add(new(new(0,0),new(0,0),new(0,0),new(0,0)));
            //ItemsBezierSegment.ItemsSource = BezierSegments;
            StrokeThickness = 1.001;
            HeadPointVisibled = true;
            PathPointVisibility = Visibility.Collapsed;
        }

        //private void Bezier2_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        //{
        //    // throw new NotImplementedException();
        //    ItemsBezierSegment.DataContext = DataContext;
        //}

        private void Rebuild()
        {
            privateBezierSegments.Clear();
            if (private_SoursePoints == null)
            {
                return;
            }
            for (int i = 0; i < private_SoursePoints?.Count - 2; i++)
            {
                AddBezierSegment(i);
            }
        }
        private void AddBezierSegment(int startPointIndex)
        {
            if (private_SoursePoints?.Count >= 3 && startPointIndex == private_SoursePoints.Count - 2)
            {
                DeleteBezierSegment(startPointIndex - 1);
                privateBezierSegments.Add(new(private_SoursePoints[startPointIndex - 1], fullPointsCollections[(startPointIndex - 1) * 3], fullPointsCollections[(startPointIndex - 1) * 3 + 1], private_SoursePoints[startPointIndex]));
            }

            if (private_SoursePoints?.Count > 1)
            {
                privateBezierSegments.Add(new(private_SoursePoints[startPointIndex], fullPointsCollections[startPointIndex * 3], fullPointsCollections[startPointIndex * 3 + 1], private_SoursePoints[startPointIndex + 1]));
            }
            //GC.Collect();
        }
        private void DeleteBezierSegment(int startPointIndex) => privateBezierSegments.RemoveAt(startPointIndex);
        //public ObservableCollection<BezierSegmentData> BezierSegments { get; } = new(new());


        private readonly ObservableCollection<BezierSegmentData> privateBezierSegments;
        public ReadOnlyObservableCollection<BezierSegmentData> BezierSegments
        {
            get => (ReadOnlyObservableCollection<BezierSegmentData>)GetValue(BezierSegmentsProperty);
            private set => SetValue(BezierSegmentsPropertyKey, value);
        }

        // Using a DependencyProperty as the backing store for BezierSegments.  This enables animation, styling, binding, etc...
        private static readonly DependencyPropertyKey BezierSegmentsPropertyKey =
            DependencyProperty.RegisterReadOnly(
                nameof(BezierSegments),
                typeof(ReadOnlyObservableCollection<BezierSegmentData>),
                typeof(CustomBezier2),
                new PropertyMetadata(null));
        public static readonly DependencyProperty BezierSegmentsProperty = BezierSegmentsPropertyKey.DependencyProperty;




        private List<Point> fullPointsCollections = new();
        private ObservableCollection<Point>? private_SoursePoints;
    }

}
