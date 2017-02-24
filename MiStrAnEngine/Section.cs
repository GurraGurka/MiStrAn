using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiStrAnEngine
{
    public class Section
    {

        // Field variables
        public List<double> thickness;
        public List<double> angles;
        public List<double> Exs;
        public List<double> Eys;
        public List<double> Gxys;
        public List<double> vs;
        public List<int> faceIndexes;
        public double density;
        public double totalThickness;


        //Constructor
        public Section(List<double> _thickness, List<double> _angles, List<double> _Exs, List<double> _Eys, List<double> _Gxys, List<double> _vs, List<int> _faceIndexes, double _density, double _totalThickness )
        {
            thickness = _thickness;
            angles = _angles;
            Exs = _Exs;
            Eys = _Eys;
            Gxys = _Gxys;
            vs = _vs;
            faceIndexes = _faceIndexes;
            density = _density;
            totalThickness = _totalThickness;
        }

        public Section()
        { }

    }
}
