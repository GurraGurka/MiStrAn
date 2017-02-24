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

        //public StructureType(List<Node> _nodes, List<ShellElement> _elements, List<Support> _bcs, List<PointLoad> _loads, List<DistributedLoad> _distLoads)
        //    : base(_nodes, _elements, _bcs, _loads, _distLoads)
        //{

        //}

        public StructureType(List<Node> _nodes, List<ShellElement> _elements) : base(_nodes, _elements)
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
            return base.ToString();
        }

        public bool Write(GH_IWriter writer)
        {
            throw new NotImplementedException();
        }



        public static StructureType CreateFromMesh(Rhino.Geometry.Mesh mesh)
        {
            List<Node> nodes = new List<Node>(mesh.Vertices.Count);

            for (int i = 0; i < mesh.Vertices.Count; i++)
            {
                Node node = new Node(mesh.Vertices[i].X, mesh.Vertices[i].Y, mesh.Vertices[i].Z, i);
                nodes.Add(node);
            }

            List<ShellElement> elements = new List<ShellElement>(mesh.Faces.Count);

            for (int i = 0; i < mesh.Faces.Count; i++)
            {
                int A = mesh.Faces[i].A;
                int B = mesh.Faces[i].B;
                int C = mesh.Faces[i].C;

                List<Node> elementNodes = new List<Node> { nodes[A], nodes[B], nodes[C] };
                ShellElement element = new ShellElement(elementNodes, i);
                elements.Add(element);
            }
            

            return new StructureType(nodes, elements);
        }
    }
}
