using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiStrAnEngine
{
    [Serializable]
    public class Section
    {

        // Field variables
        public List<double> thickness;
        public List<double> angles;
        public List<double> Exs;
        public List<double> Eys;
        public List<double> Gxys;
        public List<double> vs;
        public Vector3D faceCenterPt;
        public int ParentIndex = -1;
        public List<double> densitys;
        public double totalThickness;
        public bool applyToAll = false;


        //Constructor
        public Section(List<double> _thickness, List<double> _angles, List<double> _Exs, List<double> _Eys, List<double> _Gxys, List<double> _vs, Vector3D _faceCenterPt, List<double> _densitys, double _totalThickness )
        {
            thickness = _thickness;
            angles = _angles;
            Exs = _Exs;
            Eys = _Eys;
            Gxys = _Gxys;
            vs = _vs;
            faceCenterPt = _faceCenterPt;
            densitys = _densitys;
            totalThickness = _totalThickness;
        }

        public Section()
        { }

    }
}
