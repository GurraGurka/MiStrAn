using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiStrAnEngine
{
    public class PointLoad
    {
        public Node node;

        // Field variables
        public Vector3D LoadVec;


        //Constructor
        public PointLoad(Node _node, Vector3D _loadVec)
        {
            node = _node;
            LoadVec = _loadVec;
        }

    }
}
