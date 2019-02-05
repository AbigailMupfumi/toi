using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOI
{
    static public class lab3
    {
        /// <summary>
        /// Среднее значение x и у
        /// </summary>
        /// <param name="arr">Выборка</param>
        /// <returns></returns>
        public static double Sr(double[] arr)
        {
            return arr.Sum() / arr.Count();
        }

        /// <summary>
        /// Коэффициент корреляции r
        /// </summary>
        /// <param name="arrX"></param>
        /// <param name="arrY"></param>
        /// <returns></returns>
        public static double r(double[] arrX, double[] arrY)
        {
            double r;
            double Sum1 = 0;
            double Sum2 = 0;
            double Sum3 = 0;
            for (int i = 0; i < arrX.Count(); i++)
            {
                Sum1 += arrX[i] * arrY[i];
                Sum2 += Math.Pow((arrX[i] - Sr(arrX)), 2);
                Sum3 += Math.Pow((arrY[i] - Sr(arrY)), 2);
            }
            r = (Sum1 - arrX.Count() * Sr(arrX) * Sr(arrY)) / Math.Sqrt(Sum2 * Sum3);
            return Math.Round(r, 6);
        }

        /// <summary>
        /// Значимость коэффициента корреляции
        /// </summary>
        /// <param name="arrX"></param>
        /// <param name="arrY"></param>
        /// <param name="l"></param>
        /// <returns></returns>
        public static string rZnach(double[] arrX, double[] arrY, double l)
        {
            string Result;
            //Console.WriteLine("t=" + t(MassivX, MassivY));
            if (t(arrX, arrY) > TNorm(l, arrX.Count() - 2))
                Result = "Выборочный коэффициент корреляции значимо отличается от нуля." +
                    "\nВыборки Y и Х связаны линейной корреляционной зависимостью.";
            else
                Result = "Выборочный коэффициент корреляции близок к нулю." +
                    "\nВыборки Y и Х связаны не линейной корреляционной зависимостью.";
            return Result;
        }

        /// <summary>
        /// Доверительный интервал для коэффициента корреляции
        /// </summary>
        /// <param name="arrX"></param>
        /// <param name="arrY"></param>
        /// <param name="l"></param>
        /// <returns></returns>
        public static double[] rInt(double[] arrX, double[] arrY, double l)
        {
            double[] ab = new double[2];
            double Atanh = Math.Log((1 + r(arrX, arrY)) / (1 - r(arrX, arrY))) / 2;
            ab[0] = Atanh; ab[1] = Atanh;
            ab[0] -= MathNet.Numerics.ExcelFunctions.NormSInv(1 - l / 2) / Math.Sqrt(arrX.Count() - 3); // ...NormSInv(1-0.05/2) ~ =НОРМ.СТ.ОБР(1-0,05/2)
            ab[1] += MathNet.Numerics.ExcelFunctions.NormSInv(1 - l / 2) / Math.Sqrt(arrX.Count() - 3);
            ab[0] = Math.Round((Math.Exp(ab[0]) - Math.Exp(-ab[0])) / (Math.Exp(ab[0]) + Math.Exp(-ab[0])), 6);
            ab[1] = Math.Round((Math.Exp(ab[1]) - Math.Exp(-ab[1])) / (Math.Exp(ab[1]) + Math.Exp(-ab[1])), 6);
            return ab;
        }

        /// <summary>
        /// Значение S для гипотезы о значимости коэффициентов уравнения регрессии
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static double S(double[] arr)
        {
            double Sum = 0;
            for (int i = 0; i < arr.Count(); i++)
                Sum += Math.Pow(arr[i] - Sr(arr), 2);
            return Math.Sqrt(Sum / (arr.Count() - 1));
        }

        /// <summary>
        /// Значение t коэффициента корреляции
        /// </summary>
        /// <param name="arrX"></param>
        /// <param name="arrY"></param>
        /// <returns></returns>
        public static double t(double[] arrX, double[] arrY)
        {
            double t1 = Math.Sqrt(1 - Math.Pow(r(arrX, arrY), 2));
            t1 = Math.Abs(r(arrX, arrY)) / t1;
            t1 *= Math.Sqrt(arrX.Count() - 2);
            return t1;
        }

        /// <summary>
        /// Эмпирическое уравнение регрессии Y на X
        /// </summary>
        /// <param name="arrX"></param>
        /// <param name="arrY"></param>
        /// <returns></returns>
        public static double[] Emp(double[] arrX, double[] arrY)
        {
            double a;
            double b;
            double SumXY = 0, SumX2 = 0;
            for (int i = 0; i < arrX.Count(); i++)
            {
                SumXY += arrX[i] * arrY[i];
                SumX2 += Math.Pow(arrX[i], 2);
            }
            a = (arrY.Sum() - SumXY * arrX.Sum() / SumX2) / (arrX.Count() - Math.Pow(arrX.Sum(), 2) / SumX2);
            b = (SumXY - a * arrX.Sum()) / SumX2;
            double[] AB = { Math.Round(a, 5), Math.Round(b, 5) };
            return AB;
        }

        /// <summary>
        /// Доверительные интервалы для коэффициентов а и b уравнения регрессии
        /// </summary>
        /// <param name="arrX"></param>
        /// <param name="arrY"></param>
        /// <param name="l"></param>
        /// <returns></returns>
        public static double[,] EmpInt(double[] arrX, double[] arrY, double l)
        {
            double Sum = 0;
            double S1, Sa, Sb;
            double a = Emp(arrX, arrY)[0];
            double b = Emp(arrX, arrY)[1];
            for (int i = 0; i < arrX.Count(); i++)
                Sum += Math.Pow(arrY[i] - a - b * arrX[i], 2);
            S1 = Math.Sqrt(Sum / (arrX.Count() - 2));
            Sa = S1 * Math.Sqrt(1 / arrX.Count() + Math.Pow(Sr(arrX), 2) / ((arrX.Count() - 1) * Math.Pow(S(arrX), 2)));
            Sb = S1 / (S(arrX) * Math.Sqrt(arrX.Count() - 1));
            double Tn = TNorm(l, arrX.Count() - 2);
            double[,] Interval = new double[2, 2];
            Interval[0, 0] = Math.Round(a - Tn * Sa, 6);
            Interval[0, 1] = Math.Round(a + Tn * Sa, 6);
            Interval[1, 0] = Math.Round(b - Tn * Sb, 6);
            Interval[1, 1] = Math.Round(b + Tn * Sb, 6);
            return Interval;
        }

        /// <summary>
        /// Значимость коэффициентов а и b уравнения регрессии
        /// </summary>
        /// <param name="arrX"></param>
        /// <param name="arrY"></param>
        /// <param name="l"></param>
        /// <returns></returns>
        public static string[] EmpZnach(double[] arrX, double[] arrY, double l)
        {
            string[] Result = new string[2];
            double tSa = Emp(arrX, arrY)[0] - EmpInt(arrX, arrY, l)[0, 0];
            double tSb = Emp(arrX, arrY)[1] - EmpInt(arrX, arrY, l)[1, 0];
            if (Math.Abs(Emp(arrX, arrY)[0]) > tSa)
                Result[0] = "Коэффициент a является значимым";
            else
                Result[0] = "Коэффициент a является незначимым";
            if (Math.Abs(Emp(arrX, arrY)[1]) > tSb)
                Result[1] = "Коэффициент b является значимым";
            else
                Result[1] = "Коэффициент b является незначимым";
            return Result;
        }

        /// <summary>
        /// Коэффициент детерминации R^2
        /// </summary>
        /// <param name="arrX"></param>
        /// <param name="arrY"></param>
        /// <returns></returns>
        public static double R2(double[] arrX, double[] arrY)
        {
            double Sum1 = 0, Sum2 = 0;
            for (int i = 0; i < arrX.Count(); i++)
            {
                Sum1 += Math.Pow(arrY[i] - (Emp(arrX, arrY)[0] + Emp(arrX, arrY)[1] * arrX[i]), 2);
                Sum2 += Math.Pow(arrY[i] - Sr(arrY), 2);
            }
            //Console.WriteLine("Sum1=" + Sum1 + " Sum2=" + Sum2);
            return Math.Round(1 - Sum1 / Sum2, 4);
        }

        public static string R2Znach(double[] arrX, double[] arrY)
        {
            string Result = "";
            if (R2(arrX, arrY) * 100 >= 50)
                Result = "Модель описывает дисперсию на " + R2(arrX, arrY) * 100 + "%";
            else
                Result = "Модель описывает дисперсию на " + R2(arrX, arrY) * 100 + "%";
            return Result;
        }

        /// <summary>
        /// Адекватность уравнения регрессии Y на X
        /// </summary>
        /// <param name="arrX"></param>
        /// <param name="arrY"></param>
        /// <param name="l"></param>
        /// <returns></returns>
        public static string Adec(double[] arrX, double[] arrY, double l)
        {
            double Fv = R2(arrX, arrY) * (arrX.Count() - 2) / (1 - R2(arrX, arrY));
            if (Fv > Fkrit(l, 1, arrX.Count() - 2))
                return "Уравнение линейной регрессии\nстатистически значимо\nописывает результаты";
            else
                return "Уравнение линейной регрессии\nстатистически незначимо\nописывает результаты";
        }

        /// <summary>
        /// Распределение Стьюдента
        /// </summary>
        /// <param name="l"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static double TNorm(double l, int n)
        {
            return Math.Round(MathNet.Numerics.ExcelFunctions.TInv(l, n), 5); // ...TInv(l,n) ~ =СТЬЮДЕНТ.ОБР(1-0,05/2;8)
        }

        /// <summary>
        /// Фишер-Снедекор
        /// </summary>
        /// <param name="l"></param>
        /// <param name="k"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static double Fkrit(double l, int k, int n)
        {
            return Math.Round(MathNet.Numerics.ExcelFunctions.FInv(l, k, n), 5); // ...FInv(l,k,n) ~ =F.ОБР.ПХ(0,05;1;8)
        }


        //Лаба 4

        /// <summary>
        /// Средние точки
        /// </summary>
        /// <param name="arrX"></param>
        /// <param name="arrY"></param>
        /// <returns></returns>
        public static double[,] SrToch(double[] arrX, double[] arrY)
        {
            double[,] Toch = new double[6, 2];
            double x1 = arrX[0], xn = arrX[arrX.Count() - 1];
            double y1 = arrY[0], yn = arrY[arrY.Count() - 1];

            //Средние точки для y=ax^b
            Toch[0, 0] = Math.Round(Math.Sqrt(x1 * xn), 4); 
            Toch[0, 1] = Math.Round(Math.Sqrt(y1 * yn), 4);

            //Средние точки для y=ab^x
            Toch[1, 0] = Math.Round((x1 + xn) / 2, 4);        
            Toch[1, 1] = Math.Round(Math.Sqrt(y1 * yn), 4);

            //Средние точки для y=1/(a+bx)
            Toch[2, 0] = Math.Round((x1 + xn) / 2, 4);    
            Toch[2, 1] = Math.Round(2 * y1 * yn / (y1 + yn), 4);

            //Средние точки для y=a+b*lgx
            Toch[3, 0] = Math.Round(Math.Sqrt(x1 * xn), 4); 
            Toch[3, 1] = Math.Round((y1 + yn) / 2, 4);

            //Средние точки для y=a+b/x
            Toch[4, 0] = Math.Round(2 * x1 * xn / (x1 + xn), 4);
            Toch[4, 1] = Math.Round((y1 + yn) / 2, 4);

            //Средние точки для y=ax/(b+x)
            Toch[5, 0] = Math.Round(2 * x1 * xn / (x1 + xn), 4); 
            Toch[5, 1] = Math.Round(2 * y1 * yn / (y1 + yn), 4);

            return Toch;
        }

        /// <summary>
        /// Коэффициенты уравнений регрессии
        /// </summary>
        /// <param name="arrX"></param>
        /// <param name="arrY"></param>
        /// <returns></returns>
        public static double[,] Regr(double[] arrX, double[] arrY)
        {
            double[,] AB = new double[6, 2];
            double[,] g = new double[6, arrY.Count()];
            double[,] t = new double[6, arrY.Count()];
            for (int i = 0; i < arrX.Count(); i++)
            {
                g[0, i] = Math.Log(arrY[i], Math.E);
                t[0, i] = Math.Log(arrX[i], Math.E);

                g[1, i] = Math.Log(arrY[i], Math.E);
                t[1, i] = arrX[i];

                g[2, i] = 1 / arrY[i];
                t[2, i] = arrX[i];

                g[3, i] = arrY[i];
                t[3, i] = Math.Log10(arrX[i]); //

                g[4, i] = arrX[i] * arrY[i];
                t[4, i] = arrX[i];

                g[5, i] = 1 / arrY[i];
                t[5, i] = 1 / arrX[i];
            }
            for (int i = 0; i < 6; i++)
            {
                double Sumgt = 0, Sumt2 = 0, Sumg = 0, Sumt = 0;
                for (int j = 0; j < arrX.Count(); j++)
                {
                    Sumg += g[i, j];
                    Sumt += t[i, j];
                    Sumgt += g[i, j] * t[i, j];
                    Sumt2 += Math.Pow(t[i, j], 2);
                }
                AB[i, 0] = Math.Round((Sumg - Sumgt * Sumt / Sumt2) / (arrX.Count() - Math.Pow(Sumt, 2) / Sumt2), 5);
                AB[i, 1] = Math.Round((Sumgt - AB[i, 0] * Sumt) / Sumt2, 5);
            }
            AB[0, 0] = Math.Round(Math.Exp(AB[0, 0]), 5);
            AB[1, 0] = Math.Round(Math.Exp(AB[1, 0]), 5);
            AB[1, 1] = Math.Round(Math.Exp(AB[1, 1]), 5);
            AB[5, 0] = Math.Round(1 / AB[5, 0], 5);
            AB[5, 1] = Math.Round(AB[5, 1] * AB[5, 0], 5);
            return AB;
        }

        /// <summary>
        /// Значения функций для кривых
        /// </summary>
        /// <param name="arrX"></param>
        /// <param name="arrY"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        private static double[] Y(double[] arrX, double[] arrY, int i)
        {
            double[] Y = new double[6];
            Y[0] = Regr(arrX, arrY)[0, 0] * Math.Pow(arrX[i], Regr(arrX, arrY)[0, 1]);
            Y[1] = Regr(arrX, arrY)[1, 0] * Math.Pow(Regr(arrX, arrY)[1, 1], arrX[i]);
            Y[2] = 1 / (Regr(arrX, arrY)[2, 0] + Regr(arrX, arrY)[2, 1] * arrX[i]);
            Y[3] = Regr(arrX, arrY)[3, 0] + Math.Log10(arrX[i]) * Regr(arrX, arrY)[3, 1];
            Y[4] = Regr(arrX, arrY)[4, 0] + Regr(arrX, arrY)[4, 1] / arrX[i];
            Y[5] = Regr(arrX, arrY)[5, 0] * arrX[i] / (Regr(arrX, arrY)[5, 1] + arrX[i]);
            return Y;
        }

        public static int choice(double[] arrX, double[] arrY)
        {
            double min = Math.Pow(arrY[0] - Y(arrX, arrY, 0)[0], 2);
            int imin = 0;
            for (int i = 0; i < 6; i++)
            {
                double Sum1 = 0;
                for (int j = 0; j < arrX.Count(); j++)
                {
                    Sum1 += Math.Pow(arrY[i] - Y(arrX, arrY, j)[i], 2);
                    if (Math.Abs(Sum1) < Math.Abs(min))
                    {
                        //imin = i;
                    }
                }
            }

            return imin;
        }

        //public static int choice1(double[] arrX, double[] arrY)
        //{
        //    double min = Math.Pow(arrY[0] - Y(arrX, arrY, 0)[0], 2);
        //    double[] imin = new;
        //    for (int i = 0; i < 6; i++)
        //    {
        //        double Sum1 = 0;
        //        for (int j = 0; j < arrX.Count(); j++)
        //        {
        //            Sum1 += Math.Pow(arrY[j] - Y(arrX, arrY, j)[i], 2);
        //            if (Math.Abs(Sum1) < Math.Abs(min))
        //                imin = i;
        //        }
        //    }

        //    return imin;
        //}

        /// <summary>
        /// Коэффициенты детерминации
        /// </summary>
        /// <param name="arrX"></param>
        /// <param name="arrY"></param>
        /// <returns></returns>
        public static double R_2(double[] arrX, double[] arrY, int index)
        {
            double[] R2 = new double[6];
            for (int i = 0; i < 6; i++)
            {
                double Sum1 = 0, Sum2 = 0;
                for (int j = 0; j < arrX.Count(); j++)
                {
                    Sum1 += Math.Pow(arrY[j] - Y(arrX, arrY, j)[i], 2);
                    Sum2 += Math.Pow(arrY[j] - Sr(arrY), 2);
                }
                R2[i] = Math.Round(1 - Sum1 / Sum2, 5);
            }

            return R2[index];
        }

        /// <summary>
        /// Индекс корреляции и корреляциооное отношение
        /// </summary>
        /// <param name="arrX"></param>
        /// <param name="arrY"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static double ik(double[] arrX, double[] arrY, int index)
        {
            double Sy2 = 0, Syx2 = 0;
            for (int i = 0; i < arrX.Count(); i++)
            {
                Sy2 += Math.Pow((arrY[i] - Sr(arrY)), 2);
                Syx2 += Math.Pow(arrY[i] - Y(arrX, arrY, i)[index], 2);
            }
            Sy2 = Sy2 / arrY.Count();
            Syx2 = Syx2 / arrY.Count();
            return Math.Round(Math.Sqrt(1 - Syx2 / Sy2), 5);
        }

        /// <summary>
        /// Адекватность модели 
        /// </summary>
        /// <param name="arrX"></param>
        /// <param name="arrY"></param>
        /// <param name="index"></param>
        /// <param name="l"></param>
        /// <returns></returns>
        public static string Adec2(double[] arrX, double[] arrY, double l, int index)
        {
            double Fv = R_2(arrX, arrY, index) * (arrX.Count() - 2) / (1 - R_2(arrX, arrY, index));
            if (Fv > Fkrit(l, 1, arrX.Count() - 2))
                return "Fвыб=" + Fv + " > " + Fkrit(l, 1, arrX.Count() - 2) + "=Fкрит\nУравнение регрессии статистически значимо описывает результаты";
            else
                return "Fвыб=" + Fv + " < " + Fkrit(l, 1, arrX.Count() - 2) + "=Fкрит\nУравнение регрессии статистически незначимо описывает результаты";
        }
    }
}