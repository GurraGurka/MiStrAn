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

        public Structure(List<Node> _nodes, List<ShellElement> _elements, List<BC> _bcs)
        {
            nodes = _nodes;
            elements = _elements;
            bcs = _bcs;
        }


    }
}
