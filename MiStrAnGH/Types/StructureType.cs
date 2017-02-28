using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GH_IO.Serialization;
using Grasshopper.Kernel.Types;
using MiStrAnEngine;
using Rhino.Geometry;

namespace MiStrAnGH
{
    public class StructureType : Structure, IGH_Goo
    {
        public StructureType() : base()
        { }

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



        public static StructureType CreateFromMesh(Mesh mesh)
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

        public Mesh GenereateDeformedMesh(List<double> a, double t)
        {
            Mesh mesh = new Mesh();
            List<Vector3D> dispVecs = new List<Vector3D>();

            foreach (Node node in nodes)
            {
                Vector3D dispVec = new Vector3D(a[node.dofX], a[node.dofY], a[node.dofZ]);
                dispVec = dispVec * t;
                dispVecs.Add(dispVec);
                mesh.Vertices.Add(node.x + dispVec.X, node.y + dispVec.Y, node.z + dispVec.Z);
            }

            double max = dispVecs.Max(x => x.Length);

            Grasshopper.GUI.Gradient.GH_Gradient grad = StaticFunctions.GetStandardGradient();

            foreach (Vector3D vec in dispVecs)
            {
                double normVal = vec.Length / max;
                System.Drawing.Color color = grad.ColourAt(normVal);
                mesh.VertexColors.Add(color.R, color.G, color.B);
            }

            foreach (ShellElement elem in this.elements)
            {
                MeshFace face = new MeshFace(elem.Nodes[0].Id, elem.Nodes[1].Id, elem.Nodes[2].Id);
                mesh.Faces.AddFace(face);
            }

            return mesh;


        }

        public Mesh GenerateStressMesh(List<double> a, List<Vector3D> prinicpalStress)
        {
            Mesh mesh = new Mesh();
            List<double> vonMises = new List<double>();

            foreach (Node node in nodes)
                mesh.Vertices.Add(node.x, node.y, node.z);

            for (int i = 0; i < prinicpalStress.Count; i++)
                vonMises.Add(Math.Sqrt(prinicpalStress[i].X * prinicpalStress[i].X - prinicpalStress[i].X * prinicpalStress[i].Y + prinicpalStress[i].Y * prinicpalStress[i].Y));

            Grasshopper.GUI.Gradient.GH_Gradient grad = StaticFunctions.GetStandardGradient();

            double max = vonMises.Max();

            foreach (ShellElement elem in this.elements)
            {
                MeshFace face = new MeshFace(elem.Nodes[0].Id, elem.Nodes[1].Id, elem.Nodes[2].Id);
                mesh.Faces.AddFace(face);
            }

            for (int i = 0; i < nodes.Count; i++)
            {
                int index = mesh.TopologyVertices.TopologyVertexIndex(i);
                int[] connectedFaces = mesh.TopologyVertices.ConnectedFaces(index);

                double sum = 0;
                for (int j = 0; j < connectedFaces.Length; j++)
                {
                    sum += vonMises[connectedFaces[j]];
                }

                double mean = sum / connectedFaces.Length;

                double normVal = mean / max;

                System.Drawing.Color color = grad.ColourAt(normVal);
                mesh.VertexColors.Add(color.R, color.G, color.B);
            }

            return mesh;


        }

       


    }
}
