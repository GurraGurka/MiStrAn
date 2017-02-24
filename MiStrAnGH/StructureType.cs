using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GH_IO.Serialization;
using Grasshopper.Kernel.Types;
using MiStrAnEngine;

namespace MiStrAnGH
{
    public class StructureType : Structure, IGH_Goo
    {
        public StructureType() : base()
        { }

        public StructureType(List<Node> _nodes, List<ShellElement> _elements, List<BC> _bcs, List<Load> _loads, List<DistributedLoad> _distLoads)
            : base(_nodes, _elements, _bcs, _loads, _distLoads)
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
                return "A MiStrAn Structure";
            }
        }

        public string TypeName
        {
            get
            {
                return "MiStrAn Structure";
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
            return "testing";
        }

        public bool Write(GH_IWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}
