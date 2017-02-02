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

        public void AssembleKf()
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
          
        }
    }
}
