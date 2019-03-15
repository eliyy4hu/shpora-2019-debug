using System;
using System.Collections.Generic;
using JPEG.Utilities;

namespace JPEG
{
    public class DCT
    {
        private static double[,] BasisFunctionCache;

        public static double sqrt2 = 1 / Math.Sqrt(2);

        public static void BuildCache(int size)
        {
            BasisFunctionCache = new double[size,size];
            for (var i = 0; i < size; i++)
            {
                for (var j = 0; j < size; j++)
                {
                    BasisFunctionCache[i, j] = Math.Cos(((2d * i + 1d) * j * Math.PI) / (2 * size));
                }
            }
        }

        public static double[,] DCT2D(double[,] input)
        {
            var height = input.GetLength(0);
            var width = input.GetLength(1);

            var beta = Beta(height, width);
            var coeffs = new double[width, height];

            MathEx.LoopByTwoVariables(
                0, width,
                0, height,
                (u, v) =>
                {
                    var sum = MathEx
                        .SumByTwoVariables(
                            0, width,
                            0, height,
                            (x, y) => BasisFunction(input[x, y], u, v, x, y, height, width));

                    coeffs[u, v] = sum * beta * Alpha(u) * Alpha(v);
                });

            return coeffs;
        }

        public static void IDCT2D(double[,] coeffs, double[,] output)
        {

            var beta = Beta(coeffs.GetLength(0), coeffs.GetLength(1));
            for (var x = 0; x < coeffs.GetLength(1); x++)
            {
                for (var y = 0; y < coeffs.GetLength(0); y++)
                {
                    var sum = MathEx
                        .SumByTwoVariables(
                            0, coeffs.GetLength(1),
                            0, coeffs.GetLength(0),
                            (u, v) =>
                                BasisFunction(coeffs[u, v], u, v, x, y, coeffs.GetLength(0), coeffs.GetLength(1)) *
                                Alpha(u) * Alpha(v));

                    output[x, y] = sum * beta;
                }
            }
        }

        public static double BasisFunction(double a, double u, double v, double x, double y, int height, int width)
        {
            //var b = Math.Cos(((2d * x + 1d) * u * Math.PI) / (2 * width));
            //var c = Math.Cos(((2d * y + 1d) * v * Math.PI) / (2 * height));
            var b = BasisFunctionCache[(int) x, (int) u];
            var c = BasisFunctionCache[(int) y, (int) v];

            return a * b * c;
        }

        private static double Alpha(int u)
        {
            if (u == 0)
                return sqrt2;
            return 1;
        }

        private static double Beta(int height, int width)
        {
            return 1d / width + 1d / height;
        }
    }
}