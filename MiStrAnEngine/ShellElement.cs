using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiStrAnEngine
{
    public class ShellElement
    {
        public List<Node> nodes;
        public int Id;

        public Matrix D;

        public ShellElement(List<Node> _nodes, int _id)
        {
            nodes = _nodes;
            Id = _id;
        }

        public bool GenerateKefe(out Matrix Ke, out Matrix fe)
        {
            Ke = Matrix.ZeroMatrix(20, 20);
            fe = Matrix.ZeroMatrix(20, 1);

            Matrix dofPlan = new Matrix(new double[,] { { 1, 2, 6, 7, 11, 12, 16, 17 } });

            return true;
        }

    }
}
