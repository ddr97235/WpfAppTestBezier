using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Threading.Tasks;
using System.Windows.Threading;
using WpfAppTestBezier.Model;
using System.Collections.ObjectModel;

namespace WpfAppTestBezier.ViewModel
{
    class MainWindowViewModel:INotifyPropertyChanged
    {
        public MainWindowViewModel()
        {
            cpuusageTimer.Tick += new EventHandler(OnCPUusageTimerEvent);
            cpuusageTimer.Interval = new TimeSpan(0, 0, 0, 0, 250);
            cpuusageTimer.Start();
            bezierUpdateTimer.Tick += new EventHandler(OnBezierUpdateTimerEvent);
            bezierUpdateTimer.Interval = new TimeSpan(0, 0, 0, 0, 33);
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }
        private RelayCommand? testcpuusage;
        public RelayCommand Testcpuusage
        {
            get
            {
                return testcpuusage ??
                    (testcpuusage = new RelayCommand(obj =>
                    {
                       for(int i=0;i< Environment.ProcessorCount; i++) 
                        {
                            Task.Run(() =>
                            {
                                for (long i = 0; i < 1_000_000_000; i++)
                                {
                                    var x = ((i + 1) / 2) * 7; // пустая нагрузка
                                }
                            });                        
                        }
                    },

                      (obj) => true));
            }
        }
        private RelayCommand? testBezierSoursePointsStart;
        public RelayCommand TestBezierSoursePointsStart
        {
            get
            {
                return testBezierSoursePointsStart ??
                    (testBezierSoursePointsStart = new RelayCommand(obj =>
                    {
                        bezierUpdateTimer.Start();
                    },

                      (obj) => true));
            }
        }
        private RelayCommand? testBezierSoursePointsStop;
        public RelayCommand TestBezierSoursePointsStop
        {
            get
            {
                return testBezierSoursePointsStop ??
                    (testBezierSoursePointsStop = new RelayCommand(obj =>
                    {
                        bezierUpdateTimer.Stop();
                    },

                      (obj) => true));
            }
        }
        private DispatcherTimer cpuusageTimer =new();
        private DispatcherTimer bezierUpdateTimer = new();
        private ProcessorName cpuName = new();
        private PerformanceCounter cpuCounter = new PerformanceCounter("Process", "% Processor Time", Process.GetCurrentProcess().ProcessName);
        private List<Point>? bezierSoursePoints;
        public List<Point>? BezierSoursePoints
        {
            get => bezierSoursePoints;
            set
            {
                bezierSoursePoints = value;
                OnPropertyChanged(nameof(BezierSoursePoints));
            }
        }
        public string CPUName => cpuName.CPUName;
        public string CPUUsage => (int)(cpuCounter.NextValue()) + "%";
        private int lengthIndex = 0;
        public int LengthIndex
        {
            get => lengthIndex;
            set
            {
                lengthIndex = value;
                OnPropertyChanged(nameof(LengthIndex));
            }
        }
        private void OnCPUusageTimerEvent(object? sender, EventArgs e) => OnPropertyChanged(nameof(CPUUsage));                 
        private void OnBezierUpdateTimerEvent(object? sender, EventArgs e) => PushValue();
        private void PushValue()
        {
            if (BezierSoursePoints == null)
            {
                BezierSoursePoints = new() { new Point(0, 0) };                
            }
            if (BezierSoursePoints.Count() == 60)
            {
                BezierSoursePoints.RemoveAt(0);
            }
            Random rnd = new Random();
            Point newPoint = lengthIndex == 0 ? new(rnd.Next(950), rnd.Next(550)) : GetNewPoint(BezierSoursePoints[^1], lengthIndex * 50);
            BezierSoursePoints.Add(newPoint);
            //((MainWindow)(Application.Current.MainWindow)).trackingLine.SoursePoints = BezierSoursePoints;
            //OnPropertyChanged("BezierSoursePoints");
            OnPropertyChangedBezierSoursePoints();
        }
        private Point GetNewPoint(Point prevPoint,int length, int maxX=950, int maxY=550)
        {
            Vector vector = new();
            Random rnd = new Random();
            Point res;
            do
            {
                vector = new Vector((rnd.Next(2) == 0 ? -1 : 1) * rnd.Next(1000), (rnd.Next(2) == 0 ? -1 : 1) * rnd.Next(1000));
                vector = TriangleGeometry.NormalizeToLength(vector, length);
                res = prevPoint+ vector;
            }
            while (!(res.X>=0 && res.X<= maxX && res.Y >= 0 && res.Y <= maxY));
            return res;
        }
        private void OnPropertyChangedBezierSoursePoints()
        {
            var list = BezierSoursePoints;
            BezierSoursePoints = null;
            OnPropertyChanged(nameof(BezierSoursePoints));
            BezierSoursePoints = list;
            OnPropertyChanged(nameof(BezierSoursePoints));
        }
    }
}
