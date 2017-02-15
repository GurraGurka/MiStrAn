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
        }

        public int nnz { get { return values.Count; } }

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
                    values[index] = value;
                else
                {
                    coords.Add(new int[] { iRow, iCol });
                    values.Add(value);
                }
            }
        }

        //private SparseMatrix Multiply(SparseMatrix A, SparseMatrix B)
        //{
        //    if (A.cols != B.rows) throw new MException("Wrong dimensions of matrix!");

        //    SparseMatrix C = new SparseMatrix(A.rows, B.cols);

        //    for (int i = 0; i < B.cols; i++)
        //    {
        //        int[] colsInA
        //    }


        //}


    }
}
