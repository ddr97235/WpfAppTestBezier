using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace WpfAppTestBezier.Model
{
    internal class Bezier
    {
        /// <summary>
        ///  Расчет опорных точек для среней точки B для кубической кривой Безье
        /// </summary>
        /// <param name="A"> Стартовая точка расчитвыемого фрагмента кривой</param>
        /// <param name="B">Средняя точка расчитвыемого фрагмента кривой, для которой и вычисляем опорные точки</param>
        /// <param name="C">Конечная точка расчитвыемого фрагмента кривой</param>
        /// <param name="k"> коофициент определяющий в сколько раз B1B2 меньше AC/ </param>
        /// <returns> B1 B2 опорные точки расположенные ДО и ПОСЛЕ точки В при построении кривой</returns>
        public static (Point B1, Point B2) SupportPoints(Point A, Point B, Point C, double k = 0.25d)
        {
            // k_AC, b_AC и др - соответсвующие коофициенты k и b в уровнении прямой y=kx+b для прямой AB или других используемых
            if (C.X - A.X == 0 || C.Y - A.Y == 0)
                A.Offset(0.0001d, 0.0001d); // такой вот способ ухода от деления на ноль
            double k_AC =  (C.Y - A.Y) / (C.X - A.X);
            double k_BD = -(C.X - A.X) / (C.Y - A.Y);
            double b_AC = A.Y - k_AC * A.X;
            double b_BD = B.Y - k_BD * B.X;
            Point D = new(); // проекция точки B на AC под прямым углом.
            D.X = (b_AC - b_BD) / (k_BD - k_AC);
            D.Y = k_BD * D.X + b_BD;

            double b_B1B2 = B.Y - k_AC * B.X;
            double k_B1B2 = k_AC;

            Point B1 = new();
            //B1.X = B.X - k * (D.X - A.X); // вариант с пропроциональным удалением опорных точек от B, аналогично B2.X
            B1.X = B.X - k * (C.X - A.X); //// вариант с равным удалением опорных точек от B, аналогично B2.X
            B1.Y = k_B1B2 * B1.X + b_B1B2;
            Point B2 = new();
          //  B2.X = B.X + k * (C.X - D.X);
            B2.X = B.X + k * (C.X - A.X);
            B2.Y = k_B1B2 * B2.X + b_B1B2;
            return (B1, B2);
        }
        /// <summary>
        /// Формирует необходимую последовательность точек для посроение кубической кривой Безье. Можно использовать в WPF PolyBezierSegment, свойство Points
        /// </summary>
        /// <param name="list"> Коллекция точек для которой вычисляем все необходимые данные для построения кривой Безье</param>
        /// <param name="maxCount"> Количество используемых точек. По умолчанию -1, т.е. используются все точки</param>
        /// <returns> Последовательность точек необходимая для построения кривой</returns>
        /// <remarks>ВНИМАНИЕ! При использовании PolyBezierSegment необходимо указывать начальную точку PathFigure.StartPoint </remarks>
        public static PointCollection GetBezierPointCollection(IList<Point> list,int maxCount=-1)
        { // максимальное время исполнения 0,00001170сек, реальное в 2-а раза ниже.
            //System.Diagnostics.Stopwatch stopWatch = new();
            //stopWatch.Start();

            int listCount = maxCount == -1 ? list.Count : maxCount;
            if (listCount == 2)
                return new PointCollection(new Point[3] {list[0],list[1],list[1]}); // прямая на 2-х точках
            if (listCount < 3)
                return new PointCollection();            
            Point[] points = new Point[3 * (listCount - 1)];
            points[0] = list[0]; // коллекция начинается с "нерасчитываемой опорной точки для стартовой точки кривой                      
            for (int i = 1; i < listCount - 1; i++) // i- номер центральной точки
            {
                int pi = 3 * i - 1;
                points[pi] = list[i];
                (points[pi - 1], points[pi + 1]) = SupportPoints(list[i - 1], list[i], list[i + 1]);
            }
            points[^1] = list[listCount - 1]; // нерасчитываемая опорная точка для конечной точки кривой
            points[^2] = list[listCount - 1]; // конечная точка кривой
            var res = new PointCollection(points);
            //stopWatch.Stop();
            //double jj = (double)stopWatch.ElapsedTicks / System.Diagnostics.Stopwatch.Frequency;
            //System.Diagnostics.Debug.WriteLine("GetBezierPointCollection на "+ listCount.ToString()+ " точек, за " + string.Format("{0:F8}сек", jj) /*jj.ToString()*/);
            return res/*new PointCollection(points)*/;
        }
    }
}
