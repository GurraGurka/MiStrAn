using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SF = MiStrAnEngine.StaticFunctions;

namespace MiStrAnEngine
{
    [Serializable]
    public class Structure 
    {
        protected List<Node> nodes;
        protected List<ShellElement> elements;
        List<Support> supports;
       // List<Load> loads;
        public SparseMatrix K;
        public Vector f;
        public Matrix bc;

        public Vector a;
        public Vector r;
        public List<Vector3D> PrincipalStresses;
        public List<Matrix> stresses;
        public List<double> PrincipalAngles;



        public Structure(List<Node> _nodes, List<ShellElement> _elements) : this()
        {
            nodes = _nodes;
            elements = _elements;
        }

        public Structure()
        {
            InitializeLists();
        }

        public int NumberOfElements { get { return elements.Count; } }

        private void InitializeLists()
        {
            nodes = new List<Node>();
            elements = new List<ShellElement>();
            supports = new List<Support>();
          //  loads = new List<Load>();
        }

        public void AssembleKfbc()
        {

            int nDofs = nodes.Count * 6;
            K = new SparseMatrix(nDofs, nDofs);
            f = new Vector(nDofs);


            for (int i=0; i<elements.Count; i++)
            {
                int[] dofs = elements[i].GetElementDofs();
                Matrix Ke;
                Vector fe;
                elements[i].GenerateKefe(out Ke, out fe);

                K.AddStiffnessContribution(Ke, dofs);
                f[dofs] = f[dofs] + fe;

            }


            // Add point loads to force vector.
            foreach (Node node in nodes)
            {
                foreach (Load pL in node.Loads)
                {
                    int[] dofs = new int[]{ node.dofX, node.dofY, node.dofZ, };
                    f[dofs] = f[dofs] + pL.LoadVec.ToVector();
                }
            }


            List<int> fixedDofs = new List<int>();

            foreach (Support s in supports)
            {
                if (s.X) fixedDofs.Add(s.node.dofX);
                if (s.Y) fixedDofs.Add(s.node.dofY);
                if (s.Z) fixedDofs.Add(s.node.dofZ);
                if (s.RX) fixedDofs.Add(s.node.dofXX);
                if (s.RY) fixedDofs.Add(s.node.dofYY);
                if (s.RZ) fixedDofs.Add(s.node.dofZZ);
            }

            bc = Matrix.ZeroMatrix(fixedDofs.Count, 2);

            for (int i = 0; i < fixedDofs.Count; i++)
                bc[i, 0] = fixedDofs[i];
        }

        public bool Analyze(bool useExactMethod = false)
        {
            AssembleKfbc();


            return StaticFunctions.solveq(K, f, bc, out a, out r, useExactMethod);


        }


        //Calculate principal tresses for all the elements
        public void CalcStresses()
        {
            //Folllowing plants in MATlab, each element 15 dofs

            //the 15 active dofs used in each element (with 18 dofs in total)
            int[] activeDofs = new int[] { 0, 1, 2, 3, 4, 6, 7, 8, 9, 10, 12, 13, 14, 15, 16 };
            PrincipalStresses = new List<Vector3D>();
            stresses = new List<Matrix>();
            PrincipalAngles = new List<double>();

            for (int i = 0; i < elements.Count; i++)
            {
                //Get all the 18 element dofs from the global
                int[] dofsFull = elements[i].GetElementDofs();

                //Take the deformations from the element dofs
                Matrix aTrans = new Matrix(18, 1);
                for (int j = 0; j < dofsFull.Count(); j++)
                    aTrans[j] = a[dofsFull[j]];

                //Transform the local coordinates to global
                aTrans = elements[i].Te.Invert() * aTrans; //tror inte invert behövs

                //Take the 15 active dofs that should be used from the transformed defrormations 
                Matrix ed = new Matrix(1, 15);
                for (int j = 0; j < activeDofs.Count(); j++)
                    ed[0, j] = aTrans[activeDofs[j]];


                //Stresses (D*B*a)
                Matrix ss = elements[i].DBe * ed[0, SF.intSrs(0, 14)].Transpose();
                stresses.Add(ss);

                double theta = 0.5 * Math.Atan(2 * ss[2] / (ss[0] - ss[1]));
                PrincipalAngles.Add(theta);

                //Von mises
                //   double vM = Math.Sqrt(Math.Pow(ss[i,0],2)+ Math.Pow(ss[i,1], 2)-ss[i,0]*ss[i,1]+3*Math.Pow(ss[i,2],2));
                //  vMstresses.Add(vM);

                //Principle stresses
                double p1 = (ss[0] + ss[1]) / 2.0 + Math.Sqrt(Math.Pow((ss[0] - ss[1]) / 2, 2) + Math.Pow(ss[2], 2));
                double p2 = (ss[0] + ss[1]) / 2.0 - Math.Sqrt(Math.Pow((ss[0] - ss[1]) / 2, 2) + Math.Pow(ss[2], 2));
                PrincipalStresses.Add(new Vector3D(p1, p2, 0));
            }

        }

    

        public void AddSupports(List<Support> supports)
        {
            foreach (Support s in supports)
                this.AddSupport(s);
        }

        public void AddSupport(Support support)
        {
            Node node = GetOrAddNode(support.node.Pos);
            support.node = node;
            supports.Add(support);
        }

        public Node GetOrAddNode(Vector3D pos)
        {
            double tol = 0.0001;

            for (int i = 0; i < nodes.Count; i++)
            {
                double dist = (nodes[i].Pos - pos).Length;
                if (dist < tol)
                    return nodes[i];
            }

            Node newNode = new Node(pos.X, pos.Y, pos.Z, nodes.Count);
            nodes.Add(newNode);

            return newNode;
        }

        public Node GetNode(Vector3D pos)
        {
            double tol = 0.0001;

            for (int i = 0; i < nodes.Count; i++)
            {
                double dist = (nodes[i].Pos - pos).Length;
                if (dist < tol)
                    return nodes[i];
            }

            throw new Exception("Could not find node by position");
        }

        public ShellElement GetElementByCentroid(Vector3D pos)
        {
            double tol = 0.0001;

            for (int i = 0; i < elements.Count; i++)
            {
                double dist = (elements[i].Centroid - pos).Length;
                if (dist < tol)
                    return elements[i];
            }

            throw new Exception("Could not find element by position");

        }

        public void AddLoad(Load load)
        {
            if (load.Type == TypeOfLoad.PointLoad)
            {
                Node node = GetNode(load.Pos);
                node.Loads.Add(load);
            }

            else if (load.Type == TypeOfLoad.DistributedLoad)
            {

                ShellElement element = GetElementByCentroid(load.Pos);
                element.Loads.Add(load);
            }

            else if (load.Type == TypeOfLoad.GravityLoad)
            {
                if (!load.ApplyToAllElements)
                {
                    ShellElement element = GetElementByCentroid(load.Pos);
                    element.Loads.Add(load);
                }
                else
                {
                    foreach (ShellElement elem in elements)
                    {
                        elem.Loads.Add(load);
                    }

                }
            }
        }

        //Denna bör nog fixas till lite sen 
        public void SetSections(List<Section> sections)
        {
            //Set a section for each shell
            foreach(Section s in sections)
            {
                if(!s.applyToAll)
                {
                    //DEtta är för testing just nu!!
                  //  foreach (Vector3D vec in s.faceIndexes)
                 //   {
                        ShellElement element = GetElementByCentroid(s.faceCenterPt);
                        element.Section = s;
                        element.GenerateD();
                 //   }
                }

                else
                {
                    //Detta blir ju då lite konstigt om man inte fyller i några värden och har 2 sektioner
                    //men det ska man inte göra, kanske sätta någon varning
                    foreach (ShellElement element in elements)
                    {
                        element.Section = s;
                        element.GenerateD();
                    }

                }



            }
        }

        public Vector3D[] GetElementCentroids()
        {
            Vector3D[] centroids = new Vector3D[elements.Count];

            for (int i = 0; i < elements.Count; i++)
            {
                Vector3D cen = elements[i].Centroid;
                centroids[i] = cen;
            }

            return centroids;
        }

        public void GetElementCoordinateSystems(out Vector3D[] e1, out Vector3D[] e2, out Vector3D[] e3)
        {
            int n = elements.Count;
            e1 = new Vector3D[n];
            e2 = new Vector3D[n];
            e3 = new Vector3D[n];

            for (int i = 0; i < n; i++)
            {
                Vector3D e1_, e2_, e3_;

                elements[i].GetLocalCoordinateSystem(out e1_, out e2_, out e3_);
                e1[i] = e1_;
                e2[i] = e2_;
                e3[i] = e3_;
            }
        }

        

        public override string ToString()
        {
            return "MiStrAn Strucuture: Nodes: " + nodes.Count + ", Elements: " + elements.Count + ", Supports: " + supports.Count;
        }
    }
}

