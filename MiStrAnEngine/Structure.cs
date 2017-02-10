using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiStrAnEngine
{
    public class Structure 
    {
        List<Node> nodes;
        List<ShellElement> elements;
        List<BC> bcs;
        List<Load> loads;
        public Matrix K;
        public Matrix f;
        public Matrix bc;

        public Structure(List<Node> _nodes, List<ShellElement> _elements, List<BC> _bcs, List<Load> _loads)
        {
            nodes = _nodes;
            elements = _elements;
            bcs = _bcs;
            loads = _loads;
        }

        public Structure(List<Node> _nodes, List<ShellElement> _elements)
        {
            nodes = _nodes;
            elements = _elements;
        }

        public void AssembleKfbc()
        {
            int nDofs = nodes.Count * 6;
            K = Matrix.ZeroMatrix(nDofs, nDofs);
            f = Matrix.ZeroMatrix(nDofs, 1);

            foreach (ShellElement elem in elements)
            {
                int[] dofs = elem.GetElementDofs();
                Matrix Ke, fe;
                elem.GenerateKefe(out Ke, out fe);

                K[dofs, dofs] = K[dofs, dofs] + Ke;
                f[dofs, 0] = f[dofs, 0] + fe;
            }

            // #TODO make it possible to have rotational loads
            foreach (Load load in loads)
            {
                int[] loadDofs = new int[] {load.node.dofX, load.node.dofY, load.node.dofZ };

                f[loadDofs, 0] = f[loadDofs, 0] + load.LoadVec.ToMatrix();
            }

            // #TODO make it possible to have other bc's than fully fixed
            bc = Matrix.ZeroMatrix(bcs.Count * 5, 2);

            for (int i = 0; i < bcs.Count; i++)
            {
                bc[(i + 1) * 5 - 5, 0] = bcs[i].node.dofX;
                bc[(i + 1) * 5 - 5, 1] = 0;

                bc[(i + 1) * 5 - 4, 0] = bcs[i].node.dofY;
                bc[(i + 1) * 5 - 4, 1] = 0;

                bc[(i + 1) * 5 - 3, 0] = bcs[i].node.dofZ;
                bc[(i + 1) * 5 - 3, 1] = 0;

                bc[(i + 1) * 5 - 2, 0] = bcs[i].node.dofXX;
                bc[(i + 1) * 5 - 2, 1] = 0;

                bc[(i + 1) * 5 - 1, 0] = bcs[i].node.dofYY;
                bc[(i + 1) * 5 - 1, 1] = 0;
            }
        }

        public bool Analyze(out Matrix a, out Matrix r)
        {
            AssembleKfbc();

           return StaticFunctions.solveq(K, f, bc, out a, out r);

        }
    }
}
