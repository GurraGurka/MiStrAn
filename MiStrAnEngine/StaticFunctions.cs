using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiStrAnEngine
{
    public static class StaticFunctions
    {
        internal static Matrix GenerateK(List<Node> nodes, List<ShellElement> elements)
        {
            return new Matrix(1, 1);
        }

        // Generate a series of integerns
        public static int[] intSrs(int from, int to)
        {
            int n = to-from + 1;
            int[] series = new int[n];

            int s = from;

            for (int i = 0; i < n; i++)
            {
                series[i] = s;
                s++;
            }

            return series;
        }


    }
}
