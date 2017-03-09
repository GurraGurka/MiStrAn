using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Diagnostics;
using SF = MiStrAnEngine.StaticFunctions;


namespace MiStrAnEngine
{
    [Serializable]
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

        //Not used
        public static double InverseIterationMethod(SparseMatrix K, SparseMatrix M, Matrix bc, int nbIterations)
        {
            //This is from Dynamics assignment4

            //Reduce M and K matrices
            SparseMatrix reducedK;
            SparseMatrix reducedM;

            int[] activeDofs = Enumerable.Range(0, K.cols-1).ToArray();

            for (int i = 0; i < bc.rows; i++)
            {
                int numToRemove = Convert.ToInt32(bc[i,0]);
                activeDofs = activeDofs.Where(val => val != numToRemove).ToArray();

            }

            reducedK = K[activeDofs, activeDofs];
            reducedM = M[activeDofs, activeDofs];


            //Guess vector 
            Matrix fi0norm = Matrix.Ones(K.cols, 1);
            fi0norm = (1 / Math.Sqrt(Convert.ToDouble(reducedK.cols)))*fi0norm;

            SparseMatrix fiNorm = new SparseMatrix(reducedK.cols, nbIterations);
            int[] numbers = new int[1] { 0 };
            fiNorm.SetCol(new Vector(reducedK.cols), 0);

            Vector lambdas = new Vector(nbIterations);
            

            for (int i = 1; i < nbIterations; i++)
            {
                SparseMatrix v = reducedK.Inverse() * reducedM * fiNorm[intSrs(0, reducedK.cols - 1), intSrs(i - 1, i - 1)];
                Vector vec = v.ToMatrix().ToVector();

              //  double TempFreq = 0;

                //Take the lamnda value
                //for(int j =0; j<m.rows; j++)
                //     TempFreq += Math.Pow(m[j],2);

                //   TempFreq = Math.Sqrt(TempFreq);
                //lambdas[i] = 1.0 / TempFreq;
                lambdas[i] = 1.0 / vec.Length;

                //Get the new guess vector
                vec =(1.0/vec.Length)*vec;
                fiNorm.SetCol(vec,i);
            }

            return Math.Sqrt(lambdas[lambdas.Length - 1]) / (2.0 * Math.PI);
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


            SparseMatrix Kff = K.ExtractLargeSubMatrix(fdof, fdof);
            SparseMatrix Kfp = K.ExtractLargeSubMatrix(fdof, pdof);

            Vector b = f[fdof] - Kfp * dp;

            Vector s;

            if (!useExactMethod)
                // s = Kff.SolveWith_Preconditioned_CG(b);
                //s = Kff.SolveMathNET_PCG(b);
                // s = Kff.SolveAlglibCG(b);
               s = Kff.SolvePARDISO(b);
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

        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Ansi)]
        public static extern IntPtr LoadLibrary([MarshalAs(UnmanagedType.LPStr)]string lpFileName);


    }




}
