using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOI
{
    public class lab2
    {
        static double Fk(double x, double xSr, double S)
        {
            return Math.Exp(-Math.Pow((x - xSr), 2) / (2 * Math.Pow(S, 2)));
        }

        static double Integral(double t, double xSr, double S)
        {
            double Sum = 0, x, a, b, h;
            a = -1000;
            b = t;
            h = (b - a) / 10000;
            x = a + h;
            while (x < b)
            {
                Sum += 4 * Fk(x, xSr, S);
                x += h;
                if (x >= b) break;
                Sum += 2 * Fk(x, xSr, S);
                x += h;
            }
            Sum = 1 / (S * Math.Sqrt(2 * 3.14)) * (h / 3) * (Sum + Fk(a, xSr, S) + Fk(b, xSr, S));
            return Sum;
        }

        /// <summary>
        /// Метод подсчета Критерия Пирсона
        /// </summary>
        /// <param name="arr">Выборка</param>
        /// <returns></returns>
        public static double[] Pirs(double[] arr, double ver)
        {
            double npk = 0;
            double[] a3 = new double[2];
            double Sum = 0;
            int i = 0, k = 0;
            while (i < lab1.RowInterval(arr)[0].Length)
            {
                int j = i; //начало интервала
                double y = lab1.RowInterval(arr)[2][i]; //значение в интервале
                while (y < 5)
                {
                    i++; //переход на следующий интервал для сложения
                    if (i == lab1.RowInterval(arr)[0].Length)
                    {
                        i--;
                        break;
                    }
                    y += lab1.RowInterval(arr)[2][i]; //сложение значения интервала
                    if (i == lab1.RowInterval(arr)[0].Length - 1)
                        break;
                }
                npk = arr.Count() * Math.Abs(Integral(lab1.RowInterval(arr)[1][i], lab1.x(arr), lab1.S(arr)) - Integral(lab1.RowInterval(arr)[0][j], lab1.x(arr), lab1.S(arr)));
                Sum += Math.Round((Math.Pow((y - npk), 2) / npk), 3);
                i++; //переход на следующий интервал
                k++;
            }
            a3[0] = Sum;
            a3[1] = MathNet.Numerics.Distributions.ChiSquared.InvCDF(k-3, ver);
            return a3;
        }

        /// <summary>
        /// Метод подсчета Критерия Романовского
        /// </summary>
        /// <param name="arr"><Выборка/param>
        /// <returns></returns>
        public static double Roman(double[] arr)
        {
            int i = 0, k = 0;
            while (i < lab1.RowInterval(arr)[0].Length)
            {
                //int j = i;
                double y = lab1.RowInterval(arr)[2][i];
                while (y < 5)
                {
                    i++;
                    if (i == lab1.RowInterval(arr)[0].Length)
                        break;
                    else
                        y += lab1.RowInterval(arr)[2][i];
                    if (i == lab1.RowInterval(arr)[0].Length - 1)
                        break;
                }
                i++;
                k++;
            }
            return Math.Round((Math.Abs(Pirs(arr, 0.05)[0] - (k - 3)) / Math.Sqrt(2 * (k - 3))), 3);
        }

        /// <summary>
        /// Метод подсчета Приближенного критерия
        /// </summary>
        /// <param name="arr">Выборка</param>
        /// <returns></returns>
        public static double Prib(double[] arr)
        {
            double SA = 6 * (arr.Count() - 1);
            double SA1 = (arr.Count() + 1) * (arr.Count() + 3);
            SA = Math.Round(Math.Sqrt(Math.Round(SA / SA1, 5)), 4);
            double SE = Math.Round(Math.Sqrt((24 * arr.Count() * (arr.Count() - 2) * (arr.Count() - 3)) / (Math.Pow((arr.Count() - 1), 2) * (arr.Count() + 3) * (arr.Count() + 5))), 4);
            //Console.WriteLine("SA=" + SA + "\nSE=" + SE);
            return Math.Round(Math.Pow(Math.Abs(lab1.As(arr)), 2) / Math.Pow(SA, 2) + Math.Pow((lab1.Ex(arr)), 2) / Math.Pow(SE, 2), 5);
        }

        /// <summary>
        /// Метод подсчета Критерия Ястремского
        /// </summary>
        /// <param name="arr">Выборка</param>
        /// <returns></returns>
        public static double Yast(double[] arr)
        {
            double Sum = 0;
            int i = 0, k = 0;
            while (i < lab1.RowInterval(arr)[0].Length)
            {
                int j = i; //начало интервала
                double y = lab1.RowInterval(arr)[2][i]; //значение в интервале
                while (y < 5)
                {
                    if (i == lab1.RowInterval(arr)[0].Length - 1)
                        break;
                    i++; //переход на следующий интервал для сложения
                    y += lab1.RowInterval(arr)[2][i]; //сложение значения интервала
                    if (i == lab1.RowInterval(arr)[0].Length - 1)
                        break;
                }

                double npk = arr.Count() * Math.Abs(Integral(lab1.RowInterval(arr)[1][i], lab1.x(arr), lab1.S(arr)) - Integral(lab1.RowInterval(arr)[0][j], lab1.x(arr), lab1.S(arr)));
                Sum += Math.Round((Math.Pow((y - npk), 2) / npk * (1 - Math.Abs(Integral(lab1.RowInterval(arr)[1][i], lab1.x(arr), lab1.S(arr)) - Integral(lab1.RowInterval(arr)[0][j], lab1.x(arr), lab1.S(arr))))), 3);
                i++; //переход на следующий интервал
                k++;
            }
            return Math.Round(Sum / Math.Sqrt(2 * k + 4 * 0.6), 4);
        }

        /// <summary>
        /// Метод подсчета Критерия Колмогорова
        /// </summary>
        /// <param name="arr">Выборка</param>
        /// <returns></returns>
        public static double Kolm(double[] arr)
        {
            const double Pi = 3.14159;
            int h = Convert.ToInt32(lab1.R(arr) / lab1.k(arr));
            double[] ni = new double[lab1.RowDiscret(arr)[1].Length];
            double[] W = new double[lab1.RowDiscret(arr)[1].Length];
            for (int i = 0; i < lab1.RowDiscret(arr)[1].Length-1; i++)
            {
                double xiX = lab1.RowDiscret(arr)[0][i] - lab1.x(arr);
                double Ui = xiX / lab1.S(arr);
                double FUi = (1 / Math.Sqrt(2 * Pi) * Math.Exp(-(Math.Pow(Ui, 2) / 2)));
                double Yi = ((arr.Count() * h) / lab1.S(arr)) * FUi;
                ni[i] = Math.Round(Yi);
            }
            W[0] = Math.Abs(lab1.RowDiscret(arr)[1][0] - ni[0]);
            double[] wi = { lab1.RowDiscret(arr)[1][0], ni[0] };
            for (int i = 1; i < lab1.RowDiscret(arr)[1].Length; i++)
            {
                wi[0] += lab1.RowDiscret(arr)[1][i];
                wi[1] += ni[i];
                W[i] = Math.Abs((wi[0]) - (wi[1]));
            }
            double l = Math.Round(W.Max() / Math.Sqrt(arr.Count()), 2);
            double K = 0;
            for (int p = -1000; p < 1000; p++)
                K = K + Math.Pow((-1), p) * Math.Exp(-2 * p * p * l * l);
            return Math.Round(K, 4);
        }
    }
}
