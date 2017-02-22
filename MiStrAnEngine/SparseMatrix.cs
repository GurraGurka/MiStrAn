using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MiStrAnEngine
{

    // Sparse symmetric matrix
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
            SparseMatrix B = new SparseMatrix(A);

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

        public Vector SolveDnAnalytics_PCG(Vector b)
        {
            dnAnalytics.LinearAlgebra.Matrix A = this.ToDnAnalytics();
            dnAnalytics.LinearAlgebra.Vector v = b.ToDnAnalytics();

            dnAnalytics.LinearAlgebra.Solvers.IPreConditioner p = new dnAnalytics.LinearAlgebra.Solvers.Preconditioners.Diagonal();
      
            dnAnalytics.LinearAlgebra.Solvers.IIterator I = dnAnalytics.LinearAlgebra.Solvers.Iterator.CreateDefault();

            dnAnalytics.LinearAlgebra.Solvers.Iterative.MlkBiCgStab solver = new dnAnalytics.LinearAlgebra.Solvers.Iterative.MlkBiCgStab(p, I);


            dnAnalytics.LinearAlgebra.Vector X = solver.Solve(A, v);



            dnAnalytics.LinearAlgebra.Solvers.ICalculationStatus status = I.Status;
            MessageBox.Show(status.ToString());
            

            return new Vector(X.ToArray());


            


        }

        public dnAnalytics.LinearAlgebra.Matrix ToDnAnalytics()
        {
            dnAnalytics.LinearAlgebra.Matrix M = new dnAnalytics.LinearAlgebra.SparseMatrix(rows);

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    M[i, j] = this[i, j];
                }
            }

            return M;
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
                r = r - alpha_k * (A * p);

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
