﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace MiStrAnEngine
{

    // Sparse matrix
    [Serializable]
    public class SparseMatrix
    {
        public alglib.sparsematrix mat;
        public int rows, cols;

        public SparseMatrix(int _rows,int _cols)
        {
            alglib.sparsecreate(_rows, _cols, out mat);
            rows = _rows;
            cols = _cols;
        }

        public SparseMatrix(SparseMatrix copy) : this(copy.rows,copy.cols)
        {
            alglib.sparsecopy(copy.mat, out mat);
        }

        public int nnz { get { return mat.innerobj.vals.Length; } }

        public double this[int iRow, int iCol]      // Access this matrix as a 2D array
        {
            get
            {
                return alglib.sparseget(mat, iRow, iCol);
            }
            set
            {
                alglib.sparseset(mat, iRow, iCol, value);
            }
        }

        public SparseMatrix this[int[] iRows, int[] iCols]      // Access this matrix as a 2D array
        {
            get
            {
                SparseMatrix subMat = new SparseMatrix(iRows.Length, iCols.Length);

                for (int i = 0; i < iRows.Length; i++)
                {
                    for (int j = 0; j < iCols.Length; j++)
                    {
                        subMat[i, j] = this[iRows[i], iCols[j]];
                    }
                }

                return subMat;

            }
        }

        public SparseMatrix ExtractLargeSubMatrix(int[] _rows, int[] _cols)
        {

            SparseMatrix A = this;
            SparseMatrix ret = new SparseMatrix(_rows.Length, _cols.Length);
            A.ConvertToCRS();

            bool[] rowsBool = new bool[rows];
            bool[] colsBool = new bool[cols];
            int[] remapRows =  Enumerable.Repeat(-1,rows).ToArray();
            int[] remapCols = Enumerable.Repeat(-1, cols).ToArray();

            for (int i = 0; i < _rows.Length; i++)
            {
                rowsBool[_rows[i]] = true;
                remapRows[_rows[i]] = i;
            }

            for (int i = 0; i < _cols.Length; i++)
            {
                colsBool[_cols[i]] = true;
                remapCols[_cols[i]] = i;
            }

            int t0 = 0, t1 = 0;
            int r, k;
            double v;

            for (int i = 0; i < A.nnz; i++)
            {
                alglib.sparseenumerate(A.mat, ref t0, ref t1, out r, out k, out v);
                if (rowsBool[r] && colsBool[k])
                    ret[remapRows[r], remapCols[k]] = v;
            }
            

            return ret;

        }

        public SparseMatrix Transpose()
        {
            SparseMatrix M = new SparseMatrix(this.cols, this.rows);

            int t0 = 0, t1 = 0;

            for (int i = 0; i < nnz; i++)
            {
                int r, c;
                double value;
                alglib.sparseenumerate(mat, ref t0, ref t1, out r, out c, out value);
                M[c, r] = value;
            }

            return M;

        }

        public Matrix ToMatrix()
        {
            Matrix M = new Matrix(rows, cols);
            ConvertToCRS();

            int t0 = 0, t1 = 0;

            for (int i = 0; i < nnz; i++)
            {
                int r, c;
                double value;
                alglib.sparseenumerate(mat, ref t0, ref t1, out r, out c, out value);
                M[r, c] = value;
            }

            return M;
        }

        private static SparseMatrix Addition(SparseMatrix A, SparseMatrix B)
        {
            if(A.cols != B.cols || A.rows != B.rows) throw new MException("Wrong dimensions of matrix!");

            SparseMatrix C = new SparseMatrix(A.rows, A.cols);

            int t0 = 0, t1 = 0;

            for (int i = 0; i < A.nnz; i++)
            {
                int r, c;
                double value;
                alglib.sparseenumerate(A.mat, ref t0, ref t1, out r, out c, out value);
                alglib.sparseadd(B.mat, r, c, value);
            }

            return C;

        }

        private static SparseMatrix ScalarMultiplication(double k, SparseMatrix A)
        {
            SparseMatrix B = new SparseMatrix(A.rows, A.cols);
            int t0 = 0, t1 = 0;
            for (int i = 0; i < A.nnz; i++)
            {
                int r, c;
                double value;
                alglib.sparseenumerate(A.mat, ref t0, ref t1, out r, out c, out value);
                B[r, c] = k * value;
            }

            return B;
        }

        public void ConvertToCRS(bool overRide = false)
        {
            if(alglib.sparsegetmatrixtype(mat) != 1 || overRide)
                alglib.sparseconverttocrs(mat);
        }

        private static SparseMatrix MatrixMultiplication(SparseMatrix A, SparseMatrix B)
        {

            if (A.cols != B.rows) throw new MException("Wrong dimensions of matrix!");

            SparseMatrix C = new SparseMatrix(A.rows, B.cols);

            A.ConvertToCRS();
            B.ConvertToCRS();

            for (int i = 0; i < A.rows; i++)
            {
                int nzcnt = 0;
                double[] rowA = new double[nzcnt];
                double[] colB;            
                int[] colidx = new int[nzcnt];

                alglib.sparsegetcompressedrow(A.mat, i, ref colidx, ref rowA, out nzcnt);

                for (int j = 0; j < B.cols; j++)
                {

                    colB = new double[colidx.Length];
                    for (int k = 0; k < colidx.Length; k++)
                    {
                        colB[k] = B[colidx[k], j];
                    }
                    double val = scalarProduct(rowA, colB);

                    if (val != 0)
                        C[i, j] = val;
                }       
            }

            alglib.sparseconverttocrs(C.mat);
            return C;


        }

        private static Vector VectorMultiplication(SparseMatrix A, Vector v)
        {
            if (A.cols != v.Length) throw new MException("Vector has wrong length");
            Vector ret = new Vector(v.Length);
            A.ConvertToCRS();

            alglib.sparsemv(A.mat, v.values, ref ret.values);
            
            return ret;

        }

        // Forward substitution
        private static Vector SubsForth(SparseMatrix A, Vector v)
        {
            A.ConvertToCRS();
            Vector X = new Vector(v.Length);

            for (int i = 0; i < A.rows; i++)
            {
                int nzcnt;
                double[] vals;
                int[] colidx;
                A.GetRowEntries(i, out nzcnt, out vals, out colidx);

                double sum = 0;
                for (int k = 0; k < nzcnt-1; k++)
                    sum += vals[k] * X[colidx[k]];

                X[i] = (v[i] - sum) / A[i, i];
            }

            return X;
        }

        // Back substitution
        private static Vector SubsBack(SparseMatrix A, Vector v)
        {

            A.ConvertToCRS();
            Vector X = new Vector(v.Length);


            for (int i = A.rows - 1; i >= 0; i--)
            {
                int nzcnt;
                double[] vals;
                int[] colidx;
                A.GetRowEntries(i, out nzcnt, out vals, out colidx);

                double sum = 0;
                for (int k = 1; k < nzcnt; k++)
                    sum += vals[k] * X[colidx[k]];

                X[i] = (v[i] - sum) / A[i, i];
            }

            return X;

        }


        public SparseMatrix GetPreconditioningMatrixCholesky()
        {
            SparseMatrix iL = MakeIncompleteCholesky();


            SparseMatrix Minverse = new SparseMatrix(rows, cols);

            for (int i = 0; i < rows; i++)
            {
                Vector Ei = new Vector(rows);
                Ei[i] = 1;
                Vector col = SolveWith_LL(iL, Ei);
                Minverse.SetCol(col, i);
            }

            Minverse.ConvertToCRS();

            return Minverse;
        }

        public SparseMatrix GetPreconditioningMatrixJacobi()
        {
            SparseMatrix Minv = new SparseMatrix(rows, cols);

            for (int i = 0; i < rows; i++)
            {
                Minv[i, i] = 1/this[i, i];
            }
            Minv.ConvertToCRS();
            return Minv;
        }

        private Vector SolveWith_LL(SparseMatrix L, Vector b)
        {

            if (rows != cols) throw new MException("The matrix is not square!");
            if (rows != b.Length) throw new MException("Wrong number of results in solution vector!");

            Vector Y = SubsForth(L, b);
            Vector X = SubsBack(L.Transpose(), Y);

            return X;
        }

        public Vector SolveWith_LL(Vector b)
        {

            if (rows != cols) throw new MException("The matrix is not square!");
            if (rows != b.Length) throw new MException("Wrong number of results in solution vector!");

            SparseMatrix L = MakeCholesky();

            Vector Y = SubsForth(L, b);
            Vector X = SubsBack(L.Transpose(), Y);

            return X;
        }


        // Går att förbättra genom att använda getrowentries
        public SparseMatrix MakeIncompleteCholesky()
        {
            SparseMatrix A = this;
            SparseMatrix iL = new SparseMatrix(A.rows, A.cols);
            A.ConvertToCRS();


            int nnz = A.nnz;
            int t0 = 0; int t1 = 0;


            for (int i = 0; i < nnz; i++)
            {
                int r, c;
                double value;

                alglib.sparseenumerate(A.mat, ref t0, ref t1, out r, out c, out value);

                if (r < c)
                    continue;
                if (r == c)
                {
                    double sum = 0;
                    for (int k = 0; k < c; k++)
                    {
                        sum += iL[r,k] * iL[r, k];
                    }
                    double setVal = Math.Sqrt(value - sum);
                    iL[r, c] = setVal;
                }
                else
                {
                    double sum = 0;
                    for (int k = 0; k < c; k++)
                    {
                        sum += iL[r, k] * iL[c, k];
                    }
                    double setVal = (1 / iL[c,c]) * (value - sum);
                    iL[r, c] = setVal;
                }

            }

            return iL;
        }

        public void SetCol(Vector col,int j)
        {
            for (int i = 0; i < col.Length; i++)
            {
                if (col[i] != 0)
                    this[i, j] = col[i];
            }

        }

        public SparseMatrix MakeCholesky()
        {
            SparseMatrix A = this;

            int n = A.cols;

            if(A.rows > 2600)
            {
                Matrix M = A.ToMatrix();
                M.MakeLL_ALGLIB();

                return M.L.ToSparse();
            }

            SparseMatrix ret = new SparseMatrix(n,n);
            for (int r = 0; r < n; r++)
                for (int c = 0; c <= r; c++)
                {
                    if (c == r)
                    {
                        double sum = 0;
                        for (int j = 0; j < c; j++)
                        {
                            sum += ret[c, j] * ret[c, j];
                        }
                        ret[c, c] = Math.Sqrt(A[c, c] - sum);
                    }
                    else
                    {
                        double sum = 0;
                        for (int j = 0; j < c; j++)
                            sum += ret[r, j] * ret[c, j];
                        ret[r, c] = 1.0 / ret[c, c] * (A[r, c] - sum);
                    }
                }

            return ret;
        }

        public Vector SolveWith_CG(Vector b)
        {
            SparseMatrix A = this;
            A.ConvertToCRS();

            Vector x = new Vector(A.rows);

            Vector r = b - A * x;
            Vector old_r;

            Vector p = new Vector(r);

            int maxIterations = 1000;

            double tol = 1e-3;

            for (int i = 0; i < maxIterations; i++)
            {
                double alpha_k = (r*r)/ (p * (A * p));
                x = x + alpha_k * p;

                old_r = new Vector(r);
                r = r - alpha_k * (A * p);


                if (r.Norm < tol)
                    break;
                double beta_k = (r * r) / (old_r * old_r);
                p = r + beta_k * p;
            }


            return x;
        }

        public Vector SolveWith_Preconditioned_CG(Vector b)
        {
            SparseMatrix A = this;
            A.ConvertToCRS();

            Vector x = new Vector(A.rows);
            SparseMatrix Minv = A.GetPreconditioningMatrixJacobi();

            Vector r = new Vector(b);
            Vector z = Minv * r;
            Vector old_r, old_z;

            Vector p = new Vector(z);

            int maxIterations = b.Length; // size of matrix
            double tol = 1e-3;
            bool converged = false;
            double bNorm = b.Norm;

            for (int i = 0; i < maxIterations; i++)
            {
                Vector Ap = A * p;

                double alpha_k = (r*z) / (p * Ap);

                x = x + alpha_k * p;

                old_r = new Vector(r);
                r = r - alpha_k * (Ap);

                double relNorm = r.Norm / bNorm;
                if (relNorm < tol)
                {
                    converged = true;
                    break;
                }

                old_z = new Vector(z);

                z = Minv * r;
                double beta_k = (z * r) / (old_z * old_r);
                p = z + beta_k * p;
            }

            if (!converged)
                MessageBox.Show("Warning! Solver did not converge");
            return x;
        }

        public Vector SolveAlglibCG(Vector b)
        {
            ConvertToCRS();
            alglib.lincgstate s;
            alglib.lincgreport rep;
            double[] x;
            alglib.lincgcreate(b.Length, out s);
            alglib.lincgsolvesparse(s, mat, true, b.values);
            alglib.lincgresults(s, out x, out rep);

            return new Vector(x);
        }

        public Vector SolvePARDISO(Vector b)
        {
            SparseMatrix K = this;

            K.ConvertToCRS();
            /* Matrix data. */
            int n = b.Length;

            int[] ia/*[9]*/ = new int[K.mat.innerobj.ridx.Length];
            int[] ja/*[18]*/ = new int[K.mat.innerobj.idx.Length];
            double[] a = new double[K.mat.innerobj.vals.Length];
            double[] b_ = new double[n];
            K.mat.innerobj.ridx.CopyTo(ia, 0);
            K.mat.innerobj.idx.CopyTo(ja, 0);
            K.mat.innerobj.vals.CopyTo(a, 0);
            b.values.CopyTo(b_, 0);
            double[] x = new double[n];
            int mtype = 11; /* Real symmetric matrix */
                            /* RHS and solution vectors. */

            int nrhs = 1; /* Number of right hand sides. */
                          /* Internal solver memory pointer pt, */
                          /* 32-bit: int pt[64]; 64-bit: long int pt[64] */
                          /* or void *pt[64] should be OK on both architectures */
                          /* void *pt[64]; */
            IntPtr[] pt = new IntPtr[64];
            /* Pardiso control parameters. */
            int[] iparm = new int[64];
            int maxfct, mnum, phase, error, msglvl;
            /* Auxiliary variables. */
            int i;
            double[] ddum = new double[1]; /* Double dummy */
            int[] idum = new int[1]; /* Integer dummy. */
                                     /* ----------------------------------------------------------------- */
                                     /* .. Setup Pardiso control parameters. */
                                     /* ----------------------------------------------------------------- */
            for (i = 0; i < 64; i++)
            {
                iparm[i] = 0;
            }
            iparm[0] = 1; /* No solver default */
            iparm[1] = 2; /* Fill-in reordering from METIS */
                          /* Numbers of processors, value of OMP_NUM_THREADS */
            iparm[2] = 1;
            iparm[3] = 0; /* No iterative-direct algorithm */
            iparm[4] = 0; /* No user fill-in reducing permutation */
            iparm[5] = 0; /* Write solution into x */
            iparm[6] = 0; /* Not in use */
            iparm[7] = 10; /* Max numbers of iterative refinement steps */
            iparm[8] = 0; /* Not in use */
            iparm[9] = 13; /* Perturb the pivot elements with 1E-13 */
            iparm[10] = 1; /* Use nonsymmetric permutation and scaling MPS */
            iparm[11] = 0; /* Not in use */
            iparm[12] = 0; /* Maximum weighted matching algorithm is switched-off
                            * (default for symmetric). Try iparm[12] = 1 in case of
                            *  inappropriate accuracy */
            iparm[13] = 0; /* Output: Number of perturbed pivots */
            iparm[14] = 0; /* Not in use */
            iparm[15] = 0; /* Not in use */
            iparm[16] = 0; /* Not in use */
            iparm[17] = -1; /* Output: Number of nonzeros in the factor LU */
            iparm[18] = -1; /* Output: Mflops for LU factorization */
            iparm[19] = 0; /* Output: Numbers of CG Iterations */
            iparm[34] = 1;
            maxfct = 1; /* Maximum number of numerical factorizations. */
            mnum = 1; /* Which factorization to use. */
            msglvl = 0; /* Print statistical information in file */
            error = 0; /* Initialize error flag */
                       /* ----------------------------------------------------------------- */
                       /* .. Initialize the internal solver memory pointer. This is only */
                       /* necessary for the FIRST call of the PARDISO solver. */
                       /* ----------------------------------------------------------------- */
            for (i = 0; i < 64; i++)
            {
                pt[i] = IntPtr.Zero;
            }
            /* ----------------------------------------------------------------- */
            /* .. Reordering and Symbolic Factorization. This step also allocates */
            /* all memory that is necessary for the factorization. */
            /* ----------------------------------------------------------------- */
            phase = 11;
            MKL.pardiso(pt, ref maxfct, ref mnum, ref mtype, ref phase,
                ref n, a, ia, ja, idum, ref nrhs,
                iparm, ref msglvl, ddum, ddum, ref error);
            if (error != 0)
            {
                MessageBox.Show("\nERROR during symbolic factorization: " + error);
                return new Vector(n);
            }

            /* ----------------------------------------------------------------- */
            /* .. Numerical factorization. */
            /* ----------------------------------------------------------------- */
            phase = 22;
            MKL.pardiso(pt, ref maxfct, ref mnum, ref mtype, ref phase,
                ref n, a, ia, ja, idum, ref nrhs,
                iparm, ref msglvl, ddum, ddum, ref error);
            if (error != 0)
            {
                MessageBox.Show("\nERROR during numerical factorization: " + error);
                return new Vector(n);
            }

            /* ----------------------------------------------------------------- */
            /* .. Back substitution and iterative refinement. */
            /* ----------------------------------------------------------------- */
            phase = 33;
            iparm[7] = 5; /* Max numbers of iterative refinement steps. */
                          /* Set right hand side to one. */

            MKL.pardiso(pt, ref maxfct, ref mnum, ref mtype, ref phase,
                ref n, a, ia, ja, idum, ref nrhs,
                iparm, ref msglvl, b_, x, ref error);
            if (error != 0)
            {
                MessageBox.Show("\nERROR during solution: " + error);
                return new Vector(n);
            }

            /* ----------------------------------------------------------------- */
            /* .. Termination and release of memory. */
            /* ----------------------------------------------------------------- */
            phase = -1; /* Release internal memory. */
            MKL.pardiso(pt, ref maxfct, ref mnum, ref mtype, ref phase,
                ref n, ddum, ia, ja, idum, ref nrhs,
                iparm, ref msglvl, ddum, ddum, ref error);

            return new Vector(x);

        }




        /// <summary>
        ///  https://software.intel.com/en-us/node/521750?language=es
        ///  See link for info
        ///  ASSUMES SYMMETRIC MATRIX
        /// The routine compute all the eigenvalues and eigenvectors for generalized eigenvalue problems, Ax = λBx, within a given search interval.
        /// </summary>
        /// <param name="A">Matrix A</param>
        /// <param name="B">Matrix B</param>
        /// <param name="emin">start of search interval</param>
        /// <param name="emax">end of search interval</param>
        /// <param name="m0">number of expected eigenvalues in interval</param>
        /// <param name="eigenVectors"></param>
        /// <param name="eigenValues"></param>
        public static void GeneralizedEigen(SparseMatrix A, SparseMatrix B, double emin, double emax, int m0, out Vector[] eigenVectors, out double[] eigenValues, out double[] residuals, out string infoString)
        {
            if (A.rows != A.cols || B.rows != B.cols || A.rows != B.rows)
                throw new MException("Eigenvalue; Matrix size mismatch or not square");

            A.ConvertToCRS();
            B.ConvertToCRS();



            // INPUT VALUES
            char uplo = 'F'; //Store full matrices
            int n = A.rows;
            double[] a = new double[A.mat.innerobj.vals.Length];
            int[] ia/*[9]*/ = new int[A.mat.innerobj.ridx.Length];
            int[] ja/*[18]*/ = new int[A.mat.innerobj.idx.Length];
            A.mat.innerobj.ridx.CopyTo(ia, 0);
            A.mat.innerobj.idx.CopyTo(ja, 0);
            A.mat.innerobj.vals.CopyTo(a, 0);
            double[] b = new double[B.mat.innerobj.vals.Length];
            int[] ib/*[9]*/ = new int[B.mat.innerobj.ridx.Length];
            int[] jb/*[18]*/ = new int[B.mat.innerobj.idx.Length];
            B.mat.innerobj.ridx.CopyTo(ib, 0);
            B.mat.innerobj.idx.CopyTo(jb, 0);
            B.mat.innerobj.vals.CopyTo(b, 0);

            int[] fpm = new int[128];
            fpm[0] = 0; //do not print runtime status
            fpm[1] = 4; //number of contour points.. ???
            fpm[2] = 4; //Error trace double precision stopping criteria ε (ε = 10-fpm[2]) .
            fpm[3] = 5;  // Maximum number of Extended Eigensolver refinement loops allowed. If no convergence is reached within fpm[3] refinement loops, Extended Eigensolver routines return info=2.
            fpm[4] = 0; //Solver generates initial subspace
            fpm[5] = 0; //Stopping test..?
            fpm[6] = 5; // Error trace single precision stopping criteria (10 - fpm[6]).
            fpm[13] = 0;
            fpm[26] = 0; // Check input matrices

            //OUTPUT VALUES
            double[] x = new double[n * m0];
            double epsout = 0;
            int loops = 0;
            double[] e = new double[m0];
            int m = 0;
            double[] res = new double[m0];
            int info = 1;

            // Convert to one-based THIS COULD CERTAINLY BE OPTIMIZED
            for (int i = 0; i < ia.Length; i++)
                ia[i] = ia[i] + 1;
            for (int i = 0; i < ja.Length; i++)
                ja[i] = ja[i] + 1;
            for (int i = 0; i < ib.Length; i++)
                ib[i] = ib[i] + 1;
            for (int i = 0; i < jb.Length; i++)
                jb[i] = jb[i] + 1;


            MKL.GeneralizedEigenSolver(ref uplo, ref n, a, ia, ja, b, ib, jb, fpm, ref epsout, ref loops, ref emin, ref emax, ref m0, e, x, ref m, res, ref info);

            eigenValues = e;
            eigenVectors = new Vector[x.Count()/n];

            for (int i = 0; i < x.Count() / n; i++)
            {
                Vector v = new Vector(n);
                for (int j = 0; j < n; j++)
                {
                    v[j] = x[(i * n) + j];
                }
                eigenVectors[i] = v;
            }

            residuals = res;

            switch (info)
            {
                case 2:
                    infoString = "No convergence";
                    break;

                case 1:
                    infoString = "No eigenvalue found in the search interval.In some extreme cases the return value info=1 may indicate that the Extended Eigensolver routine has failed to find the eigenvalues in the search interval. This situation could arise if a very large search interval is used to locate a small and isolated cluster of eigenvalues (i.e. the dimension of the search interval is many orders of magnitude larger than the number of contour points. It is then either recommended to increase the number of contour points fpm[1] or simply rescale more appropriately the search interval. ";
                    break;
                case 0:
                    infoString = "Successful";
                    break;
                default:
                    infoString = "Undocumented error. Do not trust results";
                    break;
            }



        }

        public static void GeneralizedEigenTesting(SparseMatrix A, SparseMatrix B, double emin, double emax, int m0, out Vector[] eigenVectors, out double[] eigenValues)
        {
            if (A.rows != A.cols || B.rows != B.cols || A.rows != B.rows)
                throw new MException("Eigenvalue; Matrix size mismatch or not square");

            A.ConvertToCRS();
            B.ConvertToCRS();



            // INPUT VALUES
            char uplo = 'F'; //Store full matrices
            int n = A.rows;
            double[] a = new double[A.mat.innerobj.vals.Length];
            int[] ia/*[9]*/ = new int[A.mat.innerobj.ridx.Length];
            int[] ja/*[18]*/ = new int[A.mat.innerobj.idx.Length];
            A.mat.innerobj.ridx.CopyTo(ia, 0);
            A.mat.innerobj.idx.CopyTo(ja, 0);
            A.mat.innerobj.vals.CopyTo(a, 0);
            double[] b = new double[B.mat.innerobj.vals.Length];
            int[] ib/*[9]*/ = new int[B.mat.innerobj.ridx.Length];
            int[] jb/*[18]*/ = new int[B.mat.innerobj.idx.Length];
            B.mat.innerobj.ridx.CopyTo(ib, 0);
            B.mat.innerobj.idx.CopyTo(jb, 0);
            B.mat.innerobj.vals.CopyTo(b, 0);

            int[] fpm = new int[128];
            fpm[0] = 0; //do not print runtime status
            fpm[1] = 32; //number of contour points.. ???
            fpm[2] = 4; //Error trace double precision stopping criteria ε (ε = 10-fpm[2]) .
            fpm[3] = 5;  // Maximum number of Extended Eigensolver refinement loops allowed. If no convergence is reached within fpm[3] refinement loops, Extended Eigensolver routines return info=2.
            fpm[4] = 0; //Solver generates initial subspace
            fpm[5] = 0; //Stopping test..?
            fpm[6] = 5; // Error trace single precision stopping criteria (10 - fpm[6]).
            fpm[13] = 0;
            fpm[26] = 0; // Check input matrices

            //OUTPUT VALUES
            double[] x = new double[n * m0];
            double epsout = 0;
            int loops = 0;
            double[] e = new double[m0];
            int m = 0;
            double[] res = new double[m0];
            int info = 1;

            // Convert to one-based THIS COULD CERTAINLY BE OPTIMIZED
            for (int i = 0; i < ia.Length; i++)
                ia[i] = ia[i] + 1;
            for (int i = 0; i < ja.Length; i++)
                ja[i] = ja[i] + 1;
            for (int i = 0; i < ib.Length; i++)
                ib[i] = ib[i] + 1;
            for (int i = 0; i < jb.Length; i++)
                jb[i] = jb[i] + 1;


            double k = a.Max();
            double[] aScaled = scale(1.0/k, a);
            emin = 0;
            MKL.GeneralizedEigenSolver(ref uplo, ref n, aScaled, ia, ja, b, ib, jb, fpm, ref epsout, ref loops, ref emin, ref emax, ref m0, e, x, ref m, res, ref info);

            eigenValues = scale(k,e);
            eigenVectors = new Vector[x.Count() / n];

            for (int i = 0; i < x.Count() / n; i++)
            {
                Vector v = new Vector(n);
                for (int j = 0; j < n; j++)
                {
                    v[j] = x[(i * n) + j];
                }
                eigenVectors[i] = v;
            }

            Vector res1 = A * eigenVectors[0] - B * eigenValues[0] * eigenVectors[0];

        }


        public SparseMatrix Inverse()
        {
            SparseMatrix Inv = new SparseMatrix(rows, cols);

            for (int i = 0; i < rows; i++)
            {
                Vector b = new Vector(rows);
                b[i] = 1;

                Vector InvCol = SolvePARDISO(b);
                Inv.SetCol(InvCol, i);
            }

            return Inv;

        }

        private static double[] scale(double a, double[] b)
        {
            double[] ret = new double[b.Length];
            for (int i = 0; i < b.Length; i++)
            {
                ret[i] = a * b[i];
            }


            return ret;
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

        public void GetRowEntries(int i, out int nzcnt, out double[] rowValues, out int[] colidx)
        {
            nzcnt = 0;
            rowValues = new double[nzcnt];
            colidx = new int[nzcnt];

            alglib.sparsegetcompressedrow(mat, i, ref colidx, ref rowValues, out nzcnt);
        }

        public void CleanLowerTriangle()
        {
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    this[i, j] = 0;
                }
            }

        }

        public void AddStiffnessContribution(Matrix A, int[] dofs)
        {

            for (int i = 0; i < dofs.Length; i++)
            {
                for (int j = 0; j < dofs.Length; j++)
                {
                     alglib.sparseadd(this.mat, dofs[i], dofs[j], A[i, j]);
                }
            }
            
        }


        //   O P E R A T O R S

        public static SparseMatrix operator -(SparseMatrix m)
        { return SparseMatrix.ScalarMultiplication(-1, m); }

        public static SparseMatrix operator +(SparseMatrix m1, SparseMatrix m2)
        { return SparseMatrix.Addition(m1, m2); }

        public static SparseMatrix operator -(SparseMatrix m1, SparseMatrix m2)
        { return SparseMatrix.Addition(m1, -m2); }

        public static SparseMatrix operator *(SparseMatrix m1, SparseMatrix m2)
        { return SparseMatrix.MatrixMultiplication(m1, m2); }

        public static Vector operator *(SparseMatrix A, Vector v)
        { return SparseMatrix.VectorMultiplication(A, v); }

        public static SparseMatrix operator *(double n, SparseMatrix m)
        { return SparseMatrix.ScalarMultiplication(n, m); }

        public static SparseMatrix operator *(SparseMatrix m, double n)
        { return SparseMatrix.ScalarMultiplication(n, m); }
    }

    

   
}
