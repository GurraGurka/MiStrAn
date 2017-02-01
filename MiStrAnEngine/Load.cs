using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiStrAnEngine
{
    public class Load
    {
        Node node;

        // Field variables
        public double loadAmplitude;
        public double loadDirection;


        //Constructor
        public Load(Node _node,double _loadAmp, double _loadDir)
        {
            node = _node;
            loadAmplitude = _loadAmp;
            loadDirection = _loadDir;
        }

    }
}
