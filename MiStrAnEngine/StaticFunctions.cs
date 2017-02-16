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

        public static Matrix SolveWith_CG_alglib(alglib.sparsematrix A, double[] b)
        {

            alglib.sparseconverttocrs(A);

            double[] x = new double[b.Length];

            double[] r = new double[b.Length];
            b.CopyTo(r, 0);

            double[] old_r = new double[b.Length];

            double[] p = new double[b.Length];
            r.CopyTo(p, 0);

            int maxIterations = 10000;
            int resolvedIteration = 0;

            double tol = 1*b.Length;

            for (int i = 0; i < maxIterations; i++)
            {

                double[] Ap = new double[b.Length];
                alglib.sparsemv(A, p, ref Ap);

                double alpha_k = scalarProduct(r, r) / scalarProduct(p, Ap);



                x = vectorAddition(x, scalarMultiply(alpha_k, p));

                r.CopyTo(old_r, 0);
                
                r = vectorSubtraction(r,scalarMultiply(alpha_k,Ap));

                double rNorm = r.Sum(z => Math.Sqrt(z * z));
                if (rNorm < tol)
                {
                    resolvedIteration = i;
                    break;
                    
                }

                double beta_k = scalarProduct(r,r) / scalarProduct(old_r, old_r);

                p = vectorAddition(r, scalarMultiply(beta_k, p));

            }


            return new Matrix(x);

        }



        private static double scalarProduct(double[] a, double[] b)
        {
            double c = 0;

            for (int i = 0; i < a.Length; i++)
            {
                c += a[i] * b[i];
            }

            return c;
        }

        private static double[] scalarMultiply(double a, double[] b)
        {
            double[] c = new double[b.Length];

            for (int i = 0; i < b.Length; i++)
            {
                c[i] = a * b[i];
            }

            return c;
        }

        private static double[] vectorAddition(double[] a, double[] b)
        {
            double[] c = new double[b.Length];

            for (int i = 0; i < b.Length; i++)
            {
                c[i] = a[i] + b[i];
            }

            return c;
        }

        private static double[] vectorSubtraction(double[] a, double[] b)
        {
            double[] c = vectorAddition(scalarMultiply(-1, b),a);

            return c;
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

            Matrix s;

            if(!useExactMethod)
                s= SolveWith_CG_alglib(kff.ToAlglibSparse(), b.VectorToDoubleArray());
            else
                s = kff.SolveWith_LL(b);


            d[pdof, 0] = dp;
            d[fdof, 0] = s;

            Q = (K * d) - f;

            return true;
    }




    }
}
