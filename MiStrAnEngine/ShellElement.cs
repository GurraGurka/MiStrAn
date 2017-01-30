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

    }
}
