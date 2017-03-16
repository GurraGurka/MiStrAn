using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiStrAnEngine
{
    [Serializable]
    public class Support
    {
        public Node node;
        public int NodeIndex = -1;
        public bool X;
        public bool Y;
        public bool Z;
        public bool RX;
        public bool RY;
        public bool RZ;



        public Support(Node _node)
        {
            node = _node;   
        }

        public override string ToString()
        {
            string s = "Support: ";
            if (X) s += "X, ";
            if (Y) s += "Y, ";
            if (Z) s += "Z, ";
            if (RX) s += "RX, ";
            if (RY) s += "RY, ";
            if (RZ) s += "RZ, ";

            return s;
        }

    }
}
