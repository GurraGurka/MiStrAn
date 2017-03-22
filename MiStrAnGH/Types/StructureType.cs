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
    [Serializable]
    public class StructureType : Structure, IGH_Goo
    {
        public List<Line> XAxisLines;
        public List<Line> YAxisLines;
        public List<Line> ZAxisLines;
        public List<Line> MaterialAxisLines;
        public BoundingBox boundingBox;
        public List<Line> PrincipalStressLinesX;
        public List<Line> PrincipalStressLinesY;
        private Mesh mesh;



        public StructureType() : base()
        { }

        public StructureType(List<Node> _nodes, List<ShellElement> _elements) : base(_nodes, _elements)
        { }

        public Mesh M
        {
            get
            {
                if (mesh == null)
                    mesh = GenerateMesh();
                return mesh;
            }

            set
            {
                mesh = value;
            }
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
            
            StructureType S = new StructureType(nodes, elements);
            S.mesh = mesh;
            return S;
        }

        public Mesh GenerateMesh()
        {
            Mesh mesh = new Mesh();

            foreach (Node n in nodes)
                mesh.Vertices.Add(n.x, n.y, n.z);

            foreach (ShellElement e in elements)
                mesh.Faces.AddFace(e.Nodes[0].Id, e.Nodes[1].Id, e.Nodes[2].Id);

            return mesh;
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

        public Mesh GenerateStressMesh(List<double> a, List<Vector3D> prinicpalStress, double lim0, double lim1)
        {
            Mesh mesh = new Mesh();
            

            foreach (Node node in nodes)
                mesh.Vertices.Add(node.x, node.y, node.z); 

            Grasshopper.GUI.Gradient.GH_Gradient grad = StaticFunctions.GetStandardGradient(lim0, lim1);

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
                if (max == 0) normVal = 0;

                System.Drawing.Color color = grad.ColourAt(normVal);
                mesh.VertexColors.Add(color.R, color.G, color.B);
            }

            return mesh;


        }

        public Mesh GenerateEigenMode(Vector eigenvec, double t)
        {
            List<double> fullEigenVec = new List<double>();
            Mesh mesh = new Mesh();

            int[] pdof = new int[this.bc.rows];

            for (int i = 0; i < this.bc.rows; i++)
                pdof[i] = Convert.ToInt32(this.bc[i, 0]);

            //Temp
            int j = 0;
            for (int i = 0; i < K.cols; i++)
            {
                if (pdof.Contains(i))
                    fullEigenVec.Add(0);
                else
                {
                    fullEigenVec.Add(eigenvec[j]);
                    j++;
                }
                   
            }

            foreach (Node node in nodes)
            {
                Vector3D dispVec = new Vector3D(fullEigenVec[node.dofX], fullEigenVec[node.dofY], fullEigenVec[node.dofZ]);
                dispVec = dispVec * t;
                mesh.Vertices.Add(node.x + dispVec.X, node.y + dispVec.Y, node.z + dispVec.Z);
            }

            

            foreach (ShellElement elem in this.elements)
            {
                MeshFace face = new MeshFace(elem.Nodes[0].Id, elem.Nodes[1].Id, elem.Nodes[2].Id);
                mesh.Faces.AddFace(face);
            }

            return mesh;
        }

        public void GenerateElementAxisLines()
        {
            XAxisLines = new List<Line>();
            YAxisLines = new List<Line>();
            ZAxisLines = new List<Line>();
            MaterialAxisLines = new List<Line>();

            Vector3D[] centroids = GetElementCentroids();
            Vector3D[] _e1, _e2, _e3;

            GetElementCoordinateSystems(out _e1, out _e2, out _e3);

            for (int i = 0; i < NumberOfElements; i++)
            {
                Point3d C = centroids[i].ToRhinoPoint3d();
                Point3d node2 = elements[i].Nodes[1].Pos.ToRhinoPoint3d();
                Point3d node3 = elements[i].Nodes[2].Pos.ToRhinoPoint3d();
                Vector3d e1 = _e1[i].ToRhinoVector3d();
                Vector3d e2 = _e2[i].ToRhinoVector3d();
                Vector3d e3 = _e3[i].ToRhinoVector3d();


                Line B = new Line(node2, node3);
                double x, b;

                Line X = new Line(C, C + e1);

                Rhino.Geometry.Intersect.Intersection.LineLine(X, B, out x, out b, 0.001, false);
                X = new Line(C, B.PointAt(b));

                Line Y = new Line(C, C + e2 * X.Length);
                Line Z = new Line(C, C + e3 * X.Length);

                XAxisLines.Add(X);
                YAxisLines.Add(Y);
                ZAxisLines.Add(Z);

                double alpha = elements[i].MaterialOrientationAngle * 2 * Math.PI / 360;

                Transform T = Transform.Rotation(alpha, e3, C);

                Line M = new Line(X.From, X.To);
                M.Transform(T);
                MaterialAxisLines.Add(M);

            }

            GenerateBoundingboxFromAxisLines();


        }

        private void GenerateBoundingboxFromAxisLines()
        {
            List<Point3d> pts = new List<Point3d>();

            for (int i = 0; i < NumberOfElements; i++)
            {
                pts.Add(XAxisLines[i].From);
                pts.Add(XAxisLines[i].To);
                pts.Add(YAxisLines[i].From);
                pts.Add(YAxisLines[i].To);
                pts.Add(ZAxisLines[i].From);
                pts.Add(ZAxisLines[i].To);
            }

            boundingBox = new BoundingBox(pts);
        }

        public void GeneratePrincipalStressLines()
        {
            PrincipalStressLinesX = new List<Line>();
            PrincipalStressLinesY = new List<Line>();


            for (int i = 0; i < NumberOfElements; i++)
            {
                Vector3D e1, e2, e3;
                ShellElement ele = elements[i];

                ele.GetLocalCoordinateSystem(out e1, out e2, out e3);

                double L = ele.GetPerimeterLength();
                double scaler = 0.1 * L;

                Point3d C = ele.Centroid.ToRhinoPoint3d();

                Transform T = Transform.Rotation(PrincipalAngles[i], e3.ToRhinoVector3d(), C);

                //Vector3d p1 = e1.ToRhinoVector3d();
                //Vector3d p2 = e2.ToRhinoVector3d();
                //p1.Transform(T);
                //p2.Transform(T);

                Line X = new Line(C - scaler * e1.ToRhinoVector3d(), C + scaler * e1.ToRhinoVector3d());
                Line Y = new Line(C - scaler * e2.ToRhinoVector3d(), C + scaler * e2.ToRhinoVector3d());

                X.Transform(T);
                Y.Transform(T);

                PrincipalStressLinesX.Add(X);
                PrincipalStressLinesY.Add(Y);

            }


        }

        public void AlignMaterialAxisWithCurves(Curve[] dirCurves)
        {
            for (int i = 0; i < elements.Count; i++)
            {
                ShellElement elem = elements[i];
                Point3d centroid = elem.Centroid.ToRhinoPoint3d();
                double t;
                Curve dirCurve = FindClosestCurve(dirCurves, centroid, out t);

                Vector3d dirVec = dirCurve.TangentAt(t);

                Vector3D e1_, e2_, e3_,C_;
                elem.GetLocalCoordinateSystem(out e1_, out e2_, out e3_);
                Vector3d e1 = e1_.ToRhinoVector3d(), e2 = e2_.ToRhinoVector3d(), e3 = e3_.ToRhinoVector3d();
                Plane P = new Plane(centroid, e3);

                double alpha = Vector3d.VectorAngle(e1, dirVec, P);

                if (Double.IsNaN(alpha) || Double.IsInfinity(alpha) || Math.Abs(alpha) > Math.PI * 2)
                    SetDefaultMaterialOrientationAngle(i);

                else
                    elem.MaterialOrientationAngle = (alpha / (2 * Math.PI)) * 360;

                if (i == 187)
                    elem = elem; // test

            }

        }

        private static Curve FindClosestCurve(Curve[] crvs, Point3d pt, out double t)
        {
            double minDist = 1e10; //large distance
            int index = -1;
            t = 0;

            for (int i = 0; i < crvs.Length; i++)
            {
                double t_;
                crvs[i].ClosestPoint(pt, out t_);

                double d = pt.DistanceTo(crvs[i].PointAt(t_));
                if (d < minDist)
                {
                    index = i;
                    minDist = d;
                    t = t_;
                }

            }

            return crvs[index];
        }

        public void SetDefaultMaterialOrientationAngles()
        {
            for (int i = 0; i < NumberOfElements; i++)
                SetDefaultMaterialOrientationAngle(i);
            
        }

        public void SetDefaultMaterialOrientationAngle(int elementIndex)
        {
            Vector3D e1, e2, e3, C;

            ShellElement ele = elements[elementIndex];
            ele.GetLocalCoordinateSystem(out e1, out e2, out e3);
            C = ele.Centroid;

            Plane P = new Plane(C.ToRhinoPoint3d(), e3.ToRhinoVector3d());
            double alpha = Vector3d.VectorAngle(e1.ToRhinoVector3d(), Vector3d.XAxis, P);

            if (Double.IsNaN(alpha) || Double.IsInfinity(alpha) || Math.Abs(alpha) > Math.PI * 2)
                alpha = Vector3d.VectorAngle(e1.ToRhinoVector3d(), Vector3d.YAxis, P);

            ele.MaterialOrientationAngle = (alpha / (2 * Math.PI)) * 360;

        }
    }
}
