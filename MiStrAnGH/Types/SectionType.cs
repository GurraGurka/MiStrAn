using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GH_IO.Serialization;
using Grasshopper.Kernel.Types;
using MiStrAnEngine;

namespace MiStrAnGH
{
    public class SectionType : Section, IGH_Goo
    {
        public SectionType() : base()
        { }

        public SectionType(List<double> _thickness, List<double> _angles, List<double> _Exs,List<double> _Eys, List<double> _Gxys, List<double> _vs, List<int> _faceIndexes,double _density, double _totalThickness)
            : base(_thickness, _angles, _Exs, _Eys,_Gxys, _vs, _faceIndexes, _density, _totalThickness)
        {

        }

        public bool IsValid
        {
            get
            {
                return true;
            }
        }

        public string IsValidWhyNot
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string TypeDescription
        {
            get
            {
                return "A MiStrAn Section";
            }
        }

        public string TypeName
        {
            get
            {
                return "MiStrAn Section";
            }
        }

        public bool CastFrom(object source)
        {
            throw new NotImplementedException();
        }

        public bool CastTo<T>(out T target)
        {
            throw new NotImplementedException();
        }

        public IGH_Goo Duplicate()
        {
            throw new NotImplementedException();
        }

        public IGH_GooProxy EmitProxy()
        {
            throw new NotImplementedException();
        }

        public bool Read(GH_IReader reader)
        {
            throw new NotImplementedException();
        }

        public object ScriptVariable()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return "Section";
        }

        public bool Write(GH_IWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}
