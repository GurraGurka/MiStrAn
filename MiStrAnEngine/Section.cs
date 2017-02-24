using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiStrAnEngine
{
    public class Section
    {
        public int shellIndex;

        // Field variables
        public List<double> thickness;
        public List<double> angles;
        public List<double> Exs;
        public List<double> Eys;
        public List<double> vs;
        public List<int> faceIndexes;


        //Constructor
        public Section(List<double> _thickness, List<double> _angles, List<double> _Exs, List<double> _Eys, List<double> _vs, List<int> _faceIndexes)
        {
            thickness = _thickness;
            angles = _angles;
            Exs = _Exs;
            Eys = _Eys;
            vs = _vs;
            faceIndexes = _faceIndexes;
        }

    }
}
