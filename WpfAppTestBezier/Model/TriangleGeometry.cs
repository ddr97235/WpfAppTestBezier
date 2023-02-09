using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfAppTestBezier.Model
{
    internal class TriangleGeometry
    {
        /// <summary>
        /// Нормализует(пропорционально изменяет) вектор до нужной длины
        /// </summary>
        /// <param name="src">исходный вектор</param>
        /// <param name="NeeddedLenth">требуемая длина вектора</param>
        /// <returns>нормализованный вектор</returns>
        /// <remarks>Аналог Vector.Normalize, которые нормализует вектор до длины в 1(единицу), а эта функция позволяет задавать требуюемую длину</remarks>
        public static Vector NormalizeToLength(Vector src, double NeeddedLenth)
        {
            if (NeeddedLenth == 0)
            {
                return new Vector(0, 0);
            }
            double c = Math.Sqrt((src.X * src.X + src.Y * src.Y) / (NeeddedLenth * NeeddedLenth));
            return src / c;
        }

    }
}
