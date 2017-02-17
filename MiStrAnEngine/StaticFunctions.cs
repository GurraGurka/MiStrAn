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


        


        // Direct copy of CALFEM's solveq
        // FÄRDIG OCH TESTAD 2017-02-02 .hmmm...
        public static bool solveq(Matrix K, Matrix f, Matrix bc, out Matrix d, out Matrix Q, bool useExactMethod = false)
        {


            int nd = K.cols;
            int[] fdof = intSrs(0, nd - 1);
            int[] pdof = new int[bc.rows];

            for (int i = 0; i < bc.rows; i++)
                pdof[i] = Convert.ToInt32(bc[i, 0]);

            fdof = fdof.Except(pdof).ToArray();

            d = Matrix.ZeroMatrix(nd, 1);
            Q = Matrix.ZeroMatrix(nd, 1);

            Matrix dp = bc.GetCol(1);

            //       Matrix s = K[fdof, fdof].SolveWith(f[fdof, 0] - K[fdof, pdof] * dp);
            //    Matrix s = K[fdof, fdof].SolveWith_LDL(f[fdof, 0] - K[fdof, pdof] * dp);
            //Matrix s = K[fdof, fdof].SolveWith_LL(f[fdof, 0] - K[fdof, pdof] * dp);

            Matrix kff = K[fdof, fdof];
            Matrix b = f[fdof, 0] - K[fdof, pdof] * dp;

            SparseMatrix Kff_s = kff.ToSparse();

            Matrix s;

            if (!useExactMethod)
                  s = Kff_s.SolveWith_Preconditioned_CG(b.ToVector()).ToMatrix();
            else
                s = kff.SolveWith_LL(b);


            d[pdof, 0] = dp;
            d[fdof, 0] = s;

            Q = (K * d) - f;

            return true;
    }




    }
}
