using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace MiStrAnEngine
{
    public static class StaticFunctions
    {

        // Generate a series of integers
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

        // IF the parameter contains too few elements (or only one defined) it is filled up for every ply
        public static List<double> checkPlyListLength(List<double> param, int listLength)
        {
            if (param.Count != listLength)
            {

                for (int i = param.Count; i < listLength; i++)
                {
                    param.Add(param[0]);
                }
            }
            return param;
        }





        // Direct copy of CALFEM's solveq
        // FÄRDIG OCH TESTAD 2017-02-02 .hmmm...
        public static bool solveq(SparseMatrix K, Vector f, Matrix bc, out Vector d, out Vector Q, bool useExactMethod = false)
        {
            int nd = K.cols;
            int[] fdof = intSrs(0, nd - 1);
            int[] pdof = new int[bc.rows];

            for (int i = 0; i < bc.rows; i++)
                pdof[i] = Convert.ToInt32(bc[i, 0]);

            fdof = fdof.Except(pdof).ToArray();

            d = new Vector(nd);
            Q = new Vector(nd);

            Vector dp = bc.GetCol(1).ToVector();


            SparseMatrix Kff = K[fdof, fdof];
            Vector b = f[fdof] - K[fdof, pdof] * dp;

            Vector s;

            Kff.ConvertToCRS();

            if (!useExactMethod)
                 s = Kff.SolveWith_Preconditioned_CG(b);
            else
            {
                Matrix s_ = Kff.ToMatrix().SolveWith_LL(b.ToMatrix());
                s = s_.ToVector();
            }
                


            d[pdof] = dp;
            d[fdof] = s;

            Q = (K * d) - f;

            return true;
    }




    }
}
