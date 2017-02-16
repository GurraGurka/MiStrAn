using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiStrAnEngine
{

    // Sparse symmetric matrix
    public class SparseMatrix
    {
        public List<int[]> coords;
        public List<double> values;
        public int rows, cols;

        public SparseMatrix(int _rows,int _cols)
        {
            coords = new List<int[]>();
            values = new List<double>();

            rows = _rows;
            cols = _cols;
        }

        public SparseMatrix(SparseMatrix copy) : this(copy.rows,copy.cols)
        {
            for (int i = 0; i < copy.nnz; i++)
            {
                values.Add(copy.values[i]);
                coords.Add(copy.coords[i]);
            }


        }

        public int nnz { get { return values.Count; } }

        // see https://en.wikipedia.org/wiki/Conjugate_gradient_method
        public Matrix SolveWith_CG(SparseMatrix b)
        {
            SparseMatrix A = this;

            SparseMatrix x = new SparseMatrix(b.rows, b.cols);

            SparseMatrix r = b - A * x;
            SparseMatrix old_r;

            SparseMatrix p = new SparseMatrix(r);

            int maxIterations = 100;

            double tol = 1e-3;

            for (int i = 0; i < maxIterations; i++)
            {
                double alpha_k = (r.Transpose() * r)[0, 0] / (p.Transpose() * A * p)[0, 0];
                x = x + alpha_k * p;

                old_r = new SparseMatrix(r);
                r = r - alpha_k * A * p;

                if (r.Norm < tol)
                    break;
                double beta_k = (r.Transpose() * r)[0, 0] / (old_r.Transpose() * old_r)[0, 0];
                p = r + beta_k * p;
            }


            return x.ToMatrix();
        }

        public double Norm
        { get { return values.Sum(x => Math.Pow(x, 2)); } }



        public double this[int iRow, int iCol]      // Access this matrix as a 2D array
        {
            get
            {
                int index = coords.FindIndex(x => x[0] == iRow && x[1] == iCol);
                return index != -1 ? values[index] : 0;
            }
            set
            {

                int index = coords.FindIndex(x => x[0] == iRow && x[1] == iCol);

                if (index != -1)
                {
                    if (value != 0)
                    {
                        values[index] = value;
                    }
                    else
                    {
                        values.RemoveAt(index);
                        coords.RemoveAt(index);
                    }
                }
                else
                {
                    if (value != 0)
                    {
                        coords.Add(new int[] { iRow, iCol });
                        values.Add(value);
                    }
                }
            }
        }

        public SparseMatrix Transpose()
        {
            SparseMatrix M = new SparseMatrix(this.cols, this.rows);

            for (int i = 0; i < nnz; i++)
            {
                M.values.Add(this.values[i]);
                M.coords.Add(new int[] { this.coords[i][1], this.coords[i][0] });
            }

            return M;

        }

        public Matrix ToMatrix()
        {
            Matrix M = new Matrix(rows, cols);

            for (int i = 0; i < nnz; i++)
            {
                M[coords[i][0], coords[i][1]] = values[i];
            }
            return M;
        }

        public static SparseMatrix Addition(SparseMatrix A, SparseMatrix B)
        {
            if(A.cols != B.cols || A.rows != B.rows) throw new MException("Wrong dimensions of matrix!");

            SparseMatrix C = new SparseMatrix(A.rows, A.cols);

            List<int[]> coords = A.coords.Union(B.coords, new CoordEqualityComparer()).ToList();


            for (int i = 0; i < coords.Count; i++)
            {
                C[coords[i][0], coords[i][1]] = A[coords[i][0], coords[i][1]] + B[coords[i][0], coords[i][1]];
            }


            return C;

        }

        public static SparseMatrix ScalarMultiplication(double k, SparseMatrix A)
        {
            SparseMatrix B = new SparseMatrix(A.rows, A.cols);

            for (int i = 0; i < A.nnz; i++)
            {
                B.coords.Add(A.coords[i]);
                B.values.Add(A.values[i] * k);
            }

            return B;
        }

        public static SparseMatrix MatrixMultiplication(SparseMatrix A, SparseMatrix B)
        {
            if (A.cols != B.rows) throw new MException("Wrong dimensions of matrix!");

            SparseMatrix C = new SparseMatrix(A.rows, B.cols);

            for (int i = 0; i < A.rows; i++)
            {
                int[] colsInA = A.NonZeroColsInRow(i);

                for (int j = 0; j < B.cols; j++)
                {
                    int[] rowsInB = B.NonZeroRowsInColumn(j);
                    int[] intersect = colsInA.Intersect(rowsInB).ToArray();

                    C[i, j] = intersect.Sum(x => B[x, j] * A[i, x]);
                }       
            }

            return C;


        }



        private int[] NonZeroRowsInColumn(int col)
        {
            List<int[]> found = coords.FindAll(x => x[1] == col);

            int[] rows = found.ConvertAll(x => x[0]).ToArray();
            return rows;         
        }

        private int[] NonZeroColsInRow(int row)
        {
            List<int[]> found = coords.FindAll(x => x[0] == row);

            int[] cols = found.ConvertAll(x => x[1]).ToArray();
            return cols;
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

        public static SparseMatrix operator *(double n, SparseMatrix m)
        { return SparseMatrix.ScalarMultiplication(n, m); }

        public static SparseMatrix operator *(SparseMatrix m, double n)
        { return SparseMatrix.ScalarMultiplication(n, m); }
    }

    public class CoordEqualityComparer : IEqualityComparer<int[]>
    {
        public bool Equals(int[] coord1, int[] coord2)
        {
            if (coord1[0] == coord2[0] && coord1[1] == coord2[1])
                return true;
            else
                return false;
        }

        public int GetHashCode(int[] coord)
        {
            int hCode = (int)(coord[0]*3.4-coord[1]*5500);
            return hCode.GetHashCode();
        }
    }

   
}
