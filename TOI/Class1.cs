using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOI
{
    public class lab1
    {
        /// <summary>
        /// Вычисление значения k
        /// </summary>
        /// <param name="arr">Выборка</param>
        /// <returns></returns>
        public static int k(double[] arr)
        {
            return Convert.ToInt32(Math.Sqrt(arr.Count()));
        }

       
        /// <summary>
        /// Вычисление выборочной средней x
        /// </summary>
        /// <param name="arr">Выборка</param>
        /// <returns></returns>
        public static double x(double[] arr)
        {
            double sum = 0;
            for (int i = 0; i < arr.Count(); i++)
            {
                sum += arr[i];
            }

            return Math.Round((sum / arr.Count()), 4);
        }

        /// <summary>
        /// Вычисление разброса R
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static double R(double[] arr)
        {
            return (arr.Max() - arr.Min());
        }

        /// <summary>
        /// Вычисление интервального ряда
        /// </summary>
        /// <param name="arr">Выборка</param>
        /// <returns></returns>
        public static double[][] RowInterval(double[] arr)
        {
            double[][] mas = new double[3][];
            double[] xi1 = new double[] { };
            double[] xi2 = new double[] { };
            double[] ni = new double[] { };

            double h = R(arr) / k(arr);
            double x0 = arr.Min() - (0.5 * h);
            double xk = arr.Max() + (0.5 * h);

            double buf = x0;
            while(buf <= xk + h)
            {
                Array.Resize(ref xi1, xi1.Length + 1);
                Array.Resize(ref xi2, xi2.Length + 1);
                Array.Resize(ref ni, ni.Length + 1);
                xi1[xi1.Length - 1] = buf;
                xi2[xi2.Length - 1] = buf + h;
                buf += h;
            }

            Array.Resize(ref xi1, xi1.Length - 1);
            Array.Resize(ref xi2, xi2.Length - 1);

            for (int i = 0; i < xi1.Length; i++)
            {
                double k = 0;
                for(int j = 0; j < arr.Length; j++)
                {
                    if (arr[j] >= xi1[i] && arr[j] <= xi2[i])
                        k++;
                }
                ni[i] = k;
            }

            mas[0] = xi1;
            mas[1] = xi2;
            mas[2] = ni;
            return mas;
        }

        /// <summary>
        /// Вычисление дискретного ряда
        /// </summary>
        /// <param name="arr">Выборка</param>
        /// <returns></returns>
        public static double[][] RowDiscret(double[] arr)
        {
            double[][] mas = new double[2][];
            mas[0] = new double[] { };

            double[][] iter = RowInterval(arr);

            for (int i = 0; i < iter[0].Length; i++)
            {
                Array.Resize(ref mas[0], mas[0].Length + 1);
                mas[0][i] = (iter[0][i] + iter[1][i]) / 2;
            }

            mas[1] = iter[2];

            return mas;
        }

        /// <summary>
        /// Вычисление кумулятивного ряда
        /// </summary>
        /// <param name="arr">Выборка</param>
        /// <returns></returns>
        public static double[][] RowCumulation(double[] arr)
        {
            double[][] mas = new double[4][];
            double[][] disc = RowDiscret(arr);
            mas[0] = disc[0];
            mas[1] = disc[1];
            mas[2] = new double[] { };
            mas[3] = new double[] { };

            for(int i = 0; i < mas[0].Length; i++)
            {
                Array.Resize(ref mas[2], mas[2].Length + 1);
                Array.Resize(ref mas[3], mas[3].Length + 1);
                mas[2][i] = mas[1][i] / arr.Length;
                if (i == 0)
                    mas[3][i] = mas[2][i];
                else
                    mas[3][i] = mas[3][i - 1] + mas[2][i];
            }

            return mas;
        }

        /// <summary>
        /// Вычисление моды
        /// </summary>
        /// <param name="arr">Выборка</param>
        /// <returns></returns>
        public static double Moda(double[] arr)
        {
            double[][] disc = RowDiscret(arr);

            int index = 0;

            for(int i = 0; i < disc[1].Length; i++)
            {
                if (disc[1][i] > disc[1][index])
                    index = i;
            }

            return disc[0][index];
        }
       
        /// <summary>
        /// Вычисление медианы Me
        /// </summary>
        /// <param name="arr">Выборка</param>
        /// <returns></returns>
        public static double Me(double[] arr)
        {
            Array.Sort(arr);
            if (arr.Count() % 2 != 0)
                return ((arr[arr.Count() / 2] + arr[arr.Count() / 2 + 1]) / 2);
            else
                return arr[arr.Count() / 2 + 1];
        }

        /// <summary>
        /// Вычисление выборочной среднеквадратической S
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static double S(double[] arr)
        {
            double summ = 0;
            for (int i = 0; i < arr.Count(); i++)
                summ += Math.Pow((arr[i] - x(arr)), 2);
            return Math.Round((Math.Sqrt(summ / (arr.Count() - 1))), 4);
        }


        /// <summary>
        /// Вычисление выборочной дисперсии S^2
        /// </summary>
        /// <param name="arr">Выборка</param>
        /// <returns></returns>
        public static double S2(double[] arr)
        {
            return Math.Round(Math.Pow(S(arr), 2), 4);
        }

        /// <summary>
        /// Вычисление коэффициента вариации V
        /// </summary>
        /// <param name="arr">Выборка</param>
        /// <returns></returns>
        public static double V(double[] arr)
        {
            return Math.Round((S(arr) / x(arr) * 100), 4);
        }

        /// <summary>
        /// Вычисление m3
        /// </summary>
        /// <param name="arr">Выборка</param>
        /// <returns></returns>
        public static double m3(double[] arr)
        {
            double[][] disc = RowDiscret(arr);
            double m3 = 0;

            double x = lab1.x(arr);
            for(int i = 0; i < disc[0].Length; i++)
            {
               m3 += (Math.Pow(disc[0][i] - x, 3) * disc[1][i]) / arr.Length;
            }

            return Math.Round(m3, 2);
        }

        /// <summary>
        /// Вычисление ассиметрии
        /// </summary>
        /// <param name="arr">Выборка</param>
        /// <returns></returns>
        public static double As(double[] arr)
        {
            return Math.Round(lab1.m3(arr) / Math.Pow(lab1.S(arr), 3), 2);
        }

        /// <summary>
        /// Эксцес
        /// </summary>
        /// <param name="arr">Выборка</param>
        /// <returns></returns>
        public static double Ex(double[] arr)
        {
            double summ = 0;
            double[][] disc = RowDiscret(arr);

            for (int i = 0; i < disc[0].Length; i++)
                summ += (Math.Pow((disc[0][i] - x(arr)), 4) * disc[1][i]) / arr.Length;
            return Math.Round(((summ / arr.Count()) / Math.Pow(S(arr), 4) - 3), 4);
        }

        /// <summary>
        /// Доверительные интервалы
        /// </summary>
        /// <param name="arr">Выборка</param>
        /// <returns></returns>
        public static double[] trustedInter(double[] arr)
        {
            double[] result = new double[7];
            result[0] = MathNet.Numerics.ExcelFunctions.TInv(1 - 0.95, arr.Length - 1);
            result[1] = lab1.x(arr) - (result[0] * (lab1.S(arr) / Math.Sqrt(10)));
            result[2] = lab1.x(arr) + (result[0] * (lab1.S(arr) / Math.Sqrt(10)));
            result[3] = MathNet.Numerics.Distributions.ChiSquared.InvCDF(arr.Length - 1, (1 + 0.95) / 2);
            result[4] = MathNet.Numerics.Distributions.ChiSquared.InvCDF(arr.Length - 1, (1 - 0.95) / 2);
            result[5] = (arr.Length * lab1.S2(arr)) / result[4];
            result[6] = (arr.Length * lab1.S2(arr)) / result[3];

            return result;
        }
    }
}
