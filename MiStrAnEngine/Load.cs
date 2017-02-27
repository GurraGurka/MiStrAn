using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiStrAnEngine
{
    public class Load
    {
        public Vector3D LoadVec;
        public Vector3D Pos;
        public int ParentIndex; 
        public TypeOfLoad Type;
        public bool ApplyToAllElements = false;

        public Load(Vector3D pos, Vector3D loadVec, TypeOfLoad type)
        {
            LoadVec = loadVec;
            Pos = pos;
            Type = type;

            if (type == TypeOfLoad.GravityLoad) // This load is handeled by the element
                LoadVec = Vector3D.ZeroVector;
        }

        public override string ToString()
        {
            return "MiStrAn Load, type: " + Type.ToString();
        }
    }

    public enum TypeOfLoad
    {
        PointLoad,
        DistributedLoad,
        GravityLoad
    }
}
