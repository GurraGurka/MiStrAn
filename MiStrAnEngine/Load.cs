using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiStrAnEngine
{
    public class Load
    {
        public Node node;

        // Field variables
        public Vector LoadVec;


        //Constructor
        public Load(Node _node, Vector loadVec)
        {
            node = _node;
            LoadVec = loadVec;
        }

    }
}
