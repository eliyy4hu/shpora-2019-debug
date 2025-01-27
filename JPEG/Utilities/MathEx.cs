﻿using System;
using System.Linq;

namespace JPEG.Utilities
{
    public static class MathEx
    {

        public static double Sum(int from, int to, Func<int, double> function)
        {

            var res = 0.0;
            for (var i = from; i < to; i++)
            {
                res += function(i);
            }


            return res;
        }

        public static double SumByTwoVariables(int from1, int to1, int from2, int to2, Func<int, int, double> function)
            => Sum(from1, to1, x => Sum(from2, to2, y => function(x, y)));

        public static double LoopByTwoVariables(int from1, int to1, int from2, int to2, Action<int, int> function)
            => Sum(from1, to1, x => Sum(from2, to2, y =>
            {
                function(x, y);
                return 0;
            }));
    }
}