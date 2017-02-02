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


        // Direct copy of CALFEM's solveq
        // FÄRDIG OCH TESTAD 2017-02-02
        public static bool solveq(Matrix K, Matrix f, Matrix bc, out Matrix d, out Matrix Q)
        {

            int nd = K.cols;
            int[] fdof = intSrs(0, nd - 1);
            int[] pdof = new int[bc.rows];

            for (int i = 0; i < bc.rows; i++)
                pdof[i] = Convert.ToInt32(bc[i, 1]);

            fdof = fdof.Except(pdof).ToArray();

            d = Matrix.ZeroMatrix(nd, 1);
            Q = Matrix.ZeroMatrix(nd, 1);

            Matrix dp = bc.GetCol(1);

            Matrix s = K[fdof, fdof].SolveWith(f[fdof, 0] - K[fdof, pdof] * dp);

            d[pdof, 0] = dp;
            d[fdof, 0] = s;

            Q = (K * d) - f;

            return true;
    }




    }
}
