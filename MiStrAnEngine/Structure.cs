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

        public Matrix K;
        public Matrix f;

        public Structure(List<Node> _nodes, List<ShellElement> _elements, List<BC> _bcs)
        {
            nodes = _nodes;
            elements = _elements;
            bcs = _bcs;
        }

        public Structure(List<Node> _nodes, List<ShellElement> _elements)
        {
            nodes = _nodes;
            elements = _elements;
        }

        public bool AssembleKf()
        {
            int nDofs = nodes.Count * 6;
            int nElem = elements.Count;

            Matrix K = Matrix.ZeroMatrix(nDofs, nDofs);
            Matrix f = Matrix.ZeroMatrix(nDofs, 1);

            foreach (ShellElement elem in elements)
            {
                int[] elemDofs = elem.GetElementDofs();
                Matrix Ke, fe;
                elem.GenerateKefe(out Ke, out fe);

                Ke[elemDofs, elemDofs] = Ke[elemDofs, elemDofs] + Ke;
                fe[elemDofs, 0] = fe[elemDofs, 0] + fe;
            }

            return true;
        }
    }
}
