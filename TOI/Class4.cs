using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOI
{
    public class lab6
    {
        /// <summary>
        /// Метод подсчета Рангов
        /// </summary>
        /// <param name="arrX"></param>
        /// <param name="arrY"></param>
        /// <returns></returns>
        public static double[][] Ranking(double[] arrX, double[] arrY)
        {
            int n = arrX.Count(); 
            double[][] Result = new double[2][];
            Result[0] = new double[n];
            Result[1] = new double[n];
            double[] x = new double[n]; Array.Copy(arrX, x, n);
            double[] y = new double[n]; Array.Copy(arrY, y, n); 
            int[] iX = new int[n]; //массив индексов для массива х
            int[] iY = new int[n]; //массив индексов для массива у
            int[] y1 = new int[n]; //ранговый массив для у

            for (int i = 0; i < n; i++)
            {
                iX[i] = i;
                iY[i] = i;
            }

            Array.Sort(x, iX);
            Array.Sort(y, iY);

            for (int i = 0; i < n; i++)
                y1[iY[n - 1 - i]] = i + 1; //установка рангов для элементов массива у

            for (int i = 0; i < n; i++)
            {
                Result[0][i] = i + 1; //ранги для элементов массива х
                Result[1][i] = y1[iX[n - 1 - i]]; //ранги элементов массива у
            }

            return Result;
        }

        /// <summary>
        /// Метод расчета коэффициента корреляции Спирмена
        /// </summary>
        /// <param name="arrX"></param>
        /// <param name="arrY"></param>
        /// <returns></returns>
        public static double pB(double[] arrX, double[] arrY)
        {
            int n = arrX.Count();
            double Result = 0;
            for (int i = 0; i < n; i++)
            {
                Result += Math.Pow((Ranking(arrX, arrY)[0][i] - Ranking(arrX, arrY)[1][i]), 2);
            }
            return Math.Round((1 - 6 * Result / (n * n * n - n)), 5);
        }

        /// <summary>
        /// Метод подсчета коэффициента корреляции Кендала
        /// </summary>
        /// <param name="arrX"></param>
        /// <param name="arrY"></param>
        /// <returns></returns>
        public static double tB(double[] arrX, double[] arrY)
        {
            int n = arrX.Count();
            double Result = 0;
            for (int i = 0; i < n; i++)
            {
                int sum = 0;
                for (int j = i; j < n; j++)
                    if (Ranking(arrX, arrY)[1][i] < Ranking(arrX, arrY)[1][j])
                        sum++;
                Result += sum;
            }
            return Math.Round((4 * Result / (n * (n - 1)) - 1), 5);
        }

        /// <summary>
        /// Значимость коэффициента корреляции Спирмена
        /// </summary>
        /// <param name="arrX"></param>
        /// <param name="arrY"></param>
        /// <param name="l"></param>
        /// <returns></returns>
        public static string pBZnach(double[] arrX, double[] arrY, double l)
        {
            double Tkrit = lab3.TNorm(l, arrX.Count() - 2) * Math.Sqrt((1 - Math.Pow(pB(arrX, arrY), 2)) / (arrX.Count() - 2));
            if (Tkrit > Math.Abs(pB(arrX, arrY)))
                return "Крит " + Math.Round(Tkrit, 5) + " Гипотеза о равенстве нулю генерального коэффицента ранговой\n корреляции Спирмена принимается. Ранговая корреляционная связь между качественными\nпризнаками не значима.";
            else
                return "Крит " + Math.Round(Tkrit, 5) + " Гипотеза о равенстве нулю генерального коэффицента ранговой\n корреляции Спирмена отвергается. Ранговая корреляционная связь между качественными\nпризнаками значимая.";
        }

        /// <summary>
        /// Значимость коэффициента корреляции Кендала
        /// </summary>
        /// <param name="arrX"></param>
        /// <param name="arrY"></param>
        /// <param name="l"></param>
        /// <returns></returns>
        public static string tbZnach(double[] arrX, double[] arrY, double l)
        {
            double n = 30; //arrX.Count();
            double z = MathNet.Numerics.ExcelFunctions.NormSInv(1 - l / 2);
            double Tkrit = z * Math.Sqrt( (2 * ((2 * n) + 5) ) / ((9 * n) * (n - 1)) );
            if (Tkrit > 0.64)// Math.Abs(tB(arrX, arrY)))
                return "Крит " + Math.Round(Tkrit, 5) + " Гипотеза о равенстве нулю генерального коэффицента ранговой\n корреляции Кендалла принимается. Ранговая корреляционная связь между качественными\nпризнаками не значима.";
            else
                return "Крит " + Math.Round(Tkrit, 5) + " Гипотеза о равенстве нулю генерального коэффицента ранговой\n корреляции Кендалла отвергается. Ранговая корреляционная связь между качественными\nпризнаками значимая.";
        }
    }
}
