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
        public SparseMatrix K;
        public Vector f;
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
            K = new SparseMatrix(nDofs, nDofs);
            f = new Vector(nDofs);

            foreach (ShellElement elem in elements)
            {
                int[] dofs = elem.GetElementDofs();
                Matrix Ke;
                Vector fe;
                elem.GenerateKefe(out Ke, out fe);

                K.AddStiffnessContribution(Ke, dofs);
                f[dofs] = f[dofs] + fe;
            }

            // #TODO make it possible to have rotational loads
            foreach (Load load in loads)
            {
                int[] loadDofs = new int[] {load.node.dofX, load.node.dofY, load.node.dofZ };

                f[loadDofs] = f[loadDofs] + load.LoadVec.ToVector();
            }

            // #TODO make it possible to have other bc's than fully fixed
            bc = Matrix.ZeroMatrix(bcs.Count * 6, 2);

            for (int i = 0; i < bcs.Count; i++)
            {
                bc[(i + 1) * 6 - 6, 0] = bcs[i].node.dofX;
                bc[(i + 1) * 6 - 6, 1] = 0;

                bc[(i + 1) * 6 - 5, 0] = bcs[i].node.dofY;
                bc[(i + 1) * 6 - 5, 1] = 0;

                bc[(i + 1) * 6 - 4, 0] = bcs[i].node.dofZ;
                bc[(i + 1) * 6 - 4, 1] = 0;

                bc[(i + 1) * 6 - 3, 0] = bcs[i].node.dofXX;
                bc[(i + 1) * 6 - 3, 1] = 0;

                bc[(i + 1) * 6 - 2, 0] = bcs[i].node.dofYY;
                bc[(i + 1) * 6 - 2, 1] = 0;

                bc[(i + 1) * 6 - 1, 0] = bcs[i].node.dofZZ;
                bc[(i + 1) * 6 - 1, 1] = 0;
            }
        }

        public bool Analyze(out Vector a, out Vector r, bool useExactMethod = false)
        {
            AssembleKfbc();


            return StaticFunctions.solveq(K, f, bc, out a, out r, useExactMethod);

            
        }
    }
}
