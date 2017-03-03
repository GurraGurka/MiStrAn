using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GH_IO.Serialization;
using Grasshopper.Kernel.Types;
using MiStrAnEngine;

namespace MiStrAnGH
{
    [Serializable]
    public class LoadType : Load, IGH_Goo
    {
        public LoadType(Vector3D pos, Vector3D loadVec, TypeOfLoad type) : base(pos, loadVec, type)
        { }

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
                return "A MiStrAn Load";
            }
        }

        public string TypeName
        {
            get
            {
                return "MiStrAn Load";
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
            return base.ToString();
        }

        public bool Write(GH_IWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}
