using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOI
{
    public class Lab5
    {
        /// <summary>
        /// Средние значения
        /// </summary>
        /// <param name="y"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        public static double[] Sr(double[] y, double[,] x)
        {
            double[] Sum = new double[(2 + x.GetLength(0) * 3)];
            int t = 2;
            Sum[0] = y.Sum(); //Сумма всех Y

            for (int i = 0; i < y.Count(); i++)
            {
                Sum[1] += y[i] * y[i]; //Сумма всех Y^2
            }

            for (int i = 0; i < x.GetLength(0); i++)
            {
                for (int j = 0; j < x.GetLength(1); j++)
                {
                    Sum[t] += x[i, j]; //Сумма всех Х
                    Sum[t + x.GetLength(0)] += x[i, j] * x[i, j]; //Сумма всех Х^2
                    Sum[t + x.GetLength(0) * 2] += y[j] * x[i, j]; //Сумма всех Y*X
                }
                t++;
            }

            t = Sum.Count();
            for (int i = 0; i < x.GetLength(0) - 1; i++)
            {
                for (int j = i + 1; j < x.GetLength(0); j++)
                {
                    Array.Resize(ref Sum, Sum.Count() + 1);
                    for (int k = 0; k < x.GetLength(1); k++)
                    {
                        Sum[t] += x[i, k] * x[j, k]; //Сумма всех X1*X2 ....
                    }
                    t++;
                }
            }

            double[] Result = new double[Sum.Count()];
            for (int i = 0; i < Sum.Count(); i++)
            {
                Result[i] = Sum[i] / y.Count();
            }

            return Result;
        }

        public static double[] S(double[] y, double[,] x)
        {
            double[] Result = new double[(1 + x.GetLength(0))];

            //Массив sr
            int nSr = Sr(y, x).Count();     
            double[] Sr1 = new double[nSr];
            Array.Copy(Sr(y, x), Sr1, nSr); 

            Result[0] = Math.Round(Sr1[1] - Math.Pow(Sr1[0], 2), 2); //Sy2
            Result[0] = Math.Round(Math.Sqrt(Result[0]), 6); //Sy
            for (int i = 0; i < x.GetLength(0); i++)
            {
                Result[i + 1] = Math.Round(Sr1[i + 2 + x.GetLength(0)] - Math.Pow(Sr1[i + 2], 2), 6); //Sx2
                Result[i + 1] = Math.Round(Math.Sqrt(Result[i + 1]), 6); //Sx
            }
            return Result;
        }

        /// <summary>
        /// Межфакторные парные коэффициенты
        /// </summary>
        /// <param name="y"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        public static double[] r(double[] y, double[,] x)
        {
            //Массив sr
            int nSr = Sr(y, x).Count();
            double[] Sr1 = new double[nSr];
            Array.Copy(Sr(y, x), Sr1, nSr);

            int N = nSr - (2 + x.GetLength(0) * 2); //Размер выходного массива
            int start = 2 + x.GetLength(0) * 2; //Стартовая точка (yx1) в массиве средних значений Sr
            double[] Result = new double[N];

            for (int i = 0; i < x.GetLength(0); i++) //ryxi
            {
                Result[i] = Sr1[start + i] - Sr1[0] * Sr1[i + 2];
                Result[i] = Result[i] / (S(y, x)[0] * S(y, x)[i + 1]);
            }

            start += x.GetLength(0); // (x1x2)
            int j = x.GetLength(0); //Cтартовая точка для Result (rx1x2)
            int t = 1; //Индекс Хi в массиве Sr
            while (j < N)
            {
                int k = 1; //Счетчик для количества использования Xi
                while (k <= x.GetLength(0) - t) //rxix(i+1)
                {
                    Result[j] = Sr1[start] - Sr1[1 + t] * Sr1[1 + t + k];
                    Result[j] = Result[j] / (S(y, x)[t] * S(y, x)[t + k]);
                    k++;
                    j++;
                    start++;
                }
                t++;

            }
            return Result;
        }

        /// <summary>
        ///Исключение фактора Х по парному коэффициенту
        /// </summary>
        /// <param name="y"></param>
        /// <param name="x"></param>
        /// <param name="xi"></param>
        /// <returns></returns>
        private static int notXi(double[] y, double[,] x, int xi)
        {
            int nx = x.GetLength(0);
            int j = 0, t = 0, yxi_1 = -1, yxi_2 = -1;

            for (int i = r(y, x).Count() - 1; i >= nx; i--)
            {
                if ((xi <= i) && (xi >= i - j))
                {
                    yxi_1 = nx - j - 2;
                    yxi_2 = nx - (i - xi) - 1;
                    break;
                }
                else if (t >= j)
                {
                    j++;
                    t = 0;
                }
                else t++;
            }

            if (r(y, x)[yxi_1] < r(y, x)[yxi_2])
                return yxi_1;
            else
                return yxi_2;
        }

        /// <summary>
        /// Индексы исключенных факторов
        /// </summary>
        /// <param name="y"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        public static int[] notX(double[] y, double[,] x)
        {
            int[] Result = new int[0];
            int t = 0;

            for (int i = x.GetLength(0); i < r(y, x).Count(); i++)
            {
                if (r(y, x)[i] > 0.8)
                {
                    Array.Resize(ref Result, Result.Count() + 1);
                    Result[t] = notXi(y, x, i);
                    t++;
                }
            }
            return Result;
        }

        /// <summary>
        /// Нахождение коэффициентов уравнения регрессии
        /// </summary>
        /// <param name="y"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        public static double[] ab(double[] y, double[,] x)
        {
            //Заполнение матрицы коэффициентов системы 
            int nx = x.GetLength(0); //количество факторов Х
            string[,] xixj = new string[nx, nx]; //матрица с включенными факторами
            for (int i = 0; i < nx; i++)
                for (int j = 0; j < nx; j++)
                    xixj[i, j] = "not"; //дефолтное заполнение (not - исключение)

            //Массив sr
            int nSr = Sr(y, x).Count();
            double[] Sr1 = new double[nSr];
            Array.Copy(Sr(y, x), Sr1, nSr);

            int k = 2 + 3 * nx; //стартовая точка для средних значений x1x2 и т.д. в массиме Sr
            for (int i = 0; i < nx - 1; i++)
                for (int j = i + 1; j < nx; j++)
                {
                    xixj[i, j] = (Sr1[k] * y.Count()).ToString(); //заполнение всеми значениями факторов Х
                    k++;
                }

            if (notX(y, x).Count() != 0) //проверка на пустоту исключений (есть ли исключенные факторы)
            {
                for (int i = 0; i < notX(y, x).Count(); i++)
                {
                    for (int j = 0; j < nx; j++)
                    {
                        xixj[notX(y, x)[i], j] = "not"; //исключение факторов из матрицы xixj
                        xixj[j, notX(y, x)[i]] = "not";
                    }
                }
            }

            int nA = nx - notX(y, x).Count() + 1; //размерность матрицы коэффициентов СЛАУ (количество уравнений)
            double[,] A = new double[nA, nA]; //матрица коэффициентов СЛАУ
            A[0, 0] = y.Count(); //n
            int j1 = 1; //счетчик с 1го элемента в матрице
            for (int i = 2; i < nx + 2; i++)
                if (Array.IndexOf(notX(y, x), (i - 2)) < 0)
                {
                    A[0, j1] = y.Count() * Sr1[i]; //заполнение первой строки (уравнения) SrXi*n
                    A[j1, 0] = A[0, j1]; //заполнение первого столбца матрицы (по аналогии с первой строкой)  SrXi*n
                    A[j1, j1] = y.Count() * Sr1[i + nx]; //диагональ матрицы SrXi^2*n
                    j1++;
                }

            j1 = 1; //счетчки для хода по строкам (столбцам)
            int j2 = 2; //счетчки для хода по столбцам (строкам)
            bool save = false; //индикатор записи в матрицу
            for (int i = 0; i < nx; i++)
            {
                if (save) //проверка записи
                {
                    j2 = i + 2;
                    j1++;
                    save = false;
                }
                for (int t = 1; t < nx; t++)
                {
                    if (xixj[i, t] != "not") //проверка включения фактора
                    {
                        A[j1, j2] = Convert.ToDouble(xixj[i, t]); //запись в матрицу значений Sr(Xi*Xj)*n
                        A[j2, j1] = A[j1, j2];
                        j2++;
                        save = true;
                    }
                }
            }

            //Решение СЛАУ методом Гауса

            double s = 0;
            double[] ab = new double[nA]; // массив решений (искомые коэффициенты уравнения регрессии)
            double[] B = new double[nA]; // массив свободных коэффициентов системы

            for (int i = 0; i < nA; i++)
                ab[i] = 0;

            B[0] = Sr1[0] * y.Count();
            j1 = 1;
            for (int i = 2 + 2 * nx; i < 3 * nx + 2; i++)
                if (Array.IndexOf(notX(y, x), (i - (2 + 2 * nx))) < 0)
                {
                    B[j1] = y.Count() * Sr1[i]; //заполнение свободных коэффициентов SrYXi*n
                    j1++;
                }

            // Само решение СЛАУ
            for (int k1 = 0; k1 < nA - 1; k1++)
            {
                for (int i = k1 + 1; i < nA; i++)
                {
                    for (int j = k1 + 1; j < nA; j++)
                    {
                        A[i, j] = A[i, j] - A[k1, j] * (A[i, k1] / A[k1, k1]);
                    }
                    B[i] = B[i] - B[k1] * A[i, k1] / A[k1, k1];
                }
            }

            for (int k1 = nA - 1; k1 >= 0; k1--)
            {
                s = 0;
                for (int j = k1 + 1; j < nA; j++)
                    s = s + A[k1, j] * ab[j];
                ab[k1] = Math.Round((B[k1] - s) / A[k1, k1], 7);
            }
            return ab;
        }

        /// <summary>
        /// Значения уравнений Y(x)
        /// </summary>
        /// <param name="y"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        private static double[] Y1(double[] y, double[,] x)
        {
            int nx = x.GetLength(0) - notX(y, x).Count() + 1;
            double[] Result = new double[y.Count()];
            double[] ab1 = new double[nx];
            Array.Copy(ab(y, x), ab1, nx);

            for (int i = 0; i < y.Count(); i++)
            {
                int k = 1;
                Result[i] = 0;
                for (int j = 0; j < x.GetLength(0); j++)
                {
                    if (Array.IndexOf(notX(y, x), j) < 0)
                    {
                        Result[i] += ab1[k] * x[j, i];
                        k++;
                    }
                }
                Result[i] += ab1[0];
            }
            return Result;
        }

        /// <summary>
        /// Коэффициент корреляции
        /// </summary>
        /// <param name="y"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        public static double R_(double[] y, double[,] x) 
        {
            double S2_ = 0;
            double S2Y = 0;
            double[] Y = new double[y.Count()];
            Array.Copy(Y1(y, x), Y, y.Count());
            int nx = x.GetLength(0) - notX(y, x).Count();

            //Массив sr
            int nSr = Sr(y, x).Count();
            double[] Sr1 = new double[nSr];
            Array.Copy(Sr(y, x), Sr1, nSr);

            for (int i = 0; i < y.Count(); i++)
            {
                S2_ += Math.Pow((y[i] - Y[i]), 2);
                S2Y += Math.Pow((y[i] - Sr1[0]), 2);
            }

            S2_ = S2_ / (y.Count() - nx - 1);
            S2Y = S2Y / (y.Count() - 1);
            return Math.Round(Math.Sqrt(1 - S2_ / S2Y), 7);
        }

        public static double R(double[] y, double[,] x)
        {
            int nx = x.GetLength(0) - notX(y, x).Count();
            return Math.Round(Math.Sqrt(1 - (1 - Math.Pow(R_(y, x), 2)) * (y.Count() - nx - 1) / (y.Count() - 1)), 7);
        }

        /// <summary>
        /// Значимость коэффициента корреляции
        /// </summary>
        /// <param name="y"></param>
        /// <param name="x"></param>
        /// <param name="l"></param>
        /// <returns></returns>
        public static string R_Znach(double[] y, double[,] x, double l)
        {
            double SR = 1 / Math.Sqrt(y.Count() - 1);
            double Tvib = Math.Round(R_(y, x) / SR, 6);
            double Tkrit = lab3.TNorm(l, y.Count() - 3);
            if (Tvib > Tkrit)
                return "значим   Tvib=" + Tvib + " Tkrit=" + Tkrit;
            else
                return "незначим Tvib=" + Tvib + " Tkrit=" + Tkrit;
        }

        /// <summary>
        /// Адекватность уравнения тремя способами
        /// </summary>
        /// <param name="y"></param>
        /// <param name="x"></param>
        /// <param name="l"></param>
        /// <returns></returns>
        public static string Adec(double[] y, double[,] x, double l)
        {
            double R2 = Math.Round(Math.Pow(R(y, x), 2), 4); //Коэффициент детерминации
            int p = x.GetLength(0) - notX(y, x).Count(); //Количество включенных факторов Х
            string Deter = String.Format("Отобранные факторные признаки влияют на результативность в пределах {0}%", R2 * 100);
            string Fisher = "Уравнение регрессии ";
            string Aproc = "Средняя ошибка аппроксимации Е=";
            double F = (R2 * (y.Count() - p - 1)) / ((1 - R2) * p); //F наблюдаемое

            if (F > lab3.Fkrit(l, p, y.Count() - p - 1))
                Fisher += "согласуется с данными.";
            else
                Fisher += "не согласуется с данными.";

            double[] Y = new double[y.Count()]; //значения уравнений регрессии
            Array.Copy(Y1(y, x), Y, y.Count());
            double E = 0; //ошибка апроксимации

            for (int i = 0; i < y.Count(); i++)
                E += Math.Abs(y[i] - Y[i]) / y[i];

            E = Math.Round(E / y.Count() * 100, 4);
            Aproc += E + "%";

            return String.Format("{0}\n{1}\n{2}\nФишер: {3}\nFн: {4} ", Deter, Fisher, Aproc, lab3.Fkrit(l, p, y.Count() - p - 1), F);
        }

        /// <summary>
        /// Коэффициенты эластичности
        /// </summary>
        /// <param name="y"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        public static double[] K(double[] y, double[,] x)
        {
            int nx = x.GetLength(0) - notX(y, x).Count(); //Количество включенных факторов Х
            double[] Result = new double[nx];
            double[] AB = new double[nx + 1]; //коэффициенты уравнения регрессии
            Array.Copy(ab(y, x), AB, nx + 1);

            //Массив sr
            int nSr = Sr(y, x).Count();
            double[] Sr1 = new double[nSr];
            Array.Copy(Sr(y, x), Sr1, nSr);

            int j = 1;
            for (int i = 2; i < x.GetLength(0) + 2; i++)
            {
                if (Array.IndexOf(notX(y, x), (i - 2)) < 0)
                {
                    Result[j - 1] = Math.Round(AB[j] * Sr1[i] / Sr1[0], 5);
                    j++;
                }
            }

            return Result;
        }
    }
}
