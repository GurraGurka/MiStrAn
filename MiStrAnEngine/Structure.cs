using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SF = MiStrAnEngine.StaticFunctions;

namespace MiStrAnEngine
{
    public class Structure 
    {
        List<Node> nodes;
        List<ShellElement> elements;
        List<Support> supports;
        List<Load> loads;
        public SparseMatrix K;
        public Vector f;
        public Matrix bc;
        public Matrix DB;
        public Matrix T;

        //public Structure(List<Node> _nodes, List<ShellElement> _elements, List<Support> _bcs, List<PointLoad> _loads, List<DistributedLoad> _distLoads)
        //{
        //    nodes = _nodes;
        //    elements = _elements;
        //    supports = _bcs;
        //    loads = _loads;
        //    distLoads = _distLoads;
        //}

        public Structure(List<Node> _nodes, List<ShellElement> _elements) : this()
        {
            nodes = _nodes;
            elements = _elements;
        }

        public Structure()
        {
            InitializeLists();
        }

        private void InitializeLists()
        {
            nodes = new List<Node>();
            elements = new List<ShellElement>();
            supports = new List<Support>();
            loads = new List<Load>();
        }

        public void AssembleKfbc()
        {

            int nDofs = nodes.Count * 6;
            K = new SparseMatrix(nDofs, nDofs);
            f = new Vector(nDofs);
            DB = Matrix.ZeroMatrix(elements.Count * 6,15);
            T = Matrix.ZeroMatrix(elements.Count * 18, 18);


            //Maybe make B calcs a seperate function


            for (int i=0; i<elements.Count; i++)
            {
                int[] dofs = elements[i].GetElementDofs();
                Matrix Ke, q,DBe, Te;
                Vector fe;
                elements[i].GenerateKefe(out Ke, out fe, out DBe, out Te);
                DB[SF.intSrs(i * 6, 5 * (i + 1) + i), SF.intSrs(0, 14)] = DBe;
                T[SF.intSrs(i * 18, 17 * (i + 1) + i), SF.intSrs(0, 17)] = Te;
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

        public bool Analyze(out Vector a, out Vector r, bool useExactMethod = false)
        {
            AssembleKfbc();


            return StaticFunctions.solveq(K, f, bc, out a, out r, useExactMethod);


        }

        public void SetSteelSections(double t)
        {
            foreach (ShellElement el in elements)
            {
                el.thickness = t;
                el.SetSteelSection();
            }


        }


        public void CalcStresses(List<double> a, out List<Vector3D> principalStresses)
        {
            //Folllowing plants in MATlab
            //each element 15 dofs
            int[] activeDofs = new int[] { 0, 1, 2, 3, 4, 6, 7, 8, 9, 10, 12, 13, 14, 15, 16 };
            List<double> vMstresses = new List<double>();
            Matrix ed = new Matrix(elements.Count, 15);
            Matrix edDebug = new Matrix(elements.Count, 15);
            principalStresses = new List<Vector3D>();

            for (int i =0; i<elements.Count; i++)
            {
                int[] dofsFull = elements[i].GetElementDofs();

                //Transform a with local T matrix
                Matrix aTrans = new Matrix(18, 1);
                Matrix aDEBUG = new Matrix(18, 1);
                for (int j = 0; j < dofsFull.Count(); j++)
                    aTrans[j] = a[dofsFull[j]];

                 aDEBUG = aTrans;
                int tttette = 17 * (i + 1) + i;
                Matrix tTest = T[SF.intSrs(i * 18, 17 * (i + 1) + i), SF.intSrs(0, 17)];
                
                aTrans = T[SF.intSrs(i * 18, 17 * (i + 1) + i), SF.intSrs(0, 17)].Invert()* aTrans;
                
                 
                int[] dofs = new int[activeDofs.Count()];
                //Fixa hehe
                for (int j = 0; j < activeDofs.Count(); j++)
                    dofs[j] = dofsFull[activeDofs[j]];

                //Detta kan förbättras
                for (int j =0; j< activeDofs.Count();j++)
                {
                    ed[i, j] = aTrans[activeDofs[j]];
                    edDebug[i, j] = aDEBUG[activeDofs[j]];
                }
                    

                //fixa lite
                Matrix test = DB[SF.intSrs(i * 6, 5 * (i + 1) + i), SF.intSrs(0, 14)];
                Matrix ss = DB[SF.intSrs(i * 6, 5 * (i + 1) + i), SF.intSrs(0, 14)] * ed[i, SF.intSrs(0, 14)].Transpose();
                //ss[i, SF.intSrs(0, 5)] = ssTranspose.Transpose();

                //Von mises
             //   double vM = Math.Sqrt(Math.Pow(ss[i,0],2)+ Math.Pow(ss[i,1], 2)-ss[i,0]*ss[i,1]+3*Math.Pow(ss[i,2],2));
              //  vMstresses.Add(vM);

                //Principle stresses
                double p1 = (ss[0] + ss[1]) / 2.0 + Math.Sqrt(Math.Pow((ss[0]-ss[1])/2,2)+Math.Pow(ss[2],2));
                double p2 = (ss[0] + ss[1]) / 2.0 - Math.Sqrt(Math.Pow((ss[0] - ss[1]) / 2, 2) + Math.Pow(ss[2], 2));
                principalStresses.Add(new Vector3D(p1, p2, 0));


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
            if(load.Type == TypeOfLoad.PointLoad)
            {
                Node node = GetNode(load.Pos);
                node.Loads.Add(load);
            }

            else if(load.Type == TypeOfLoad.DistributedLoad)
            {
                ShellElement element = GetElementByCentroid(load.Pos);
                element.Loads.Add(load);
            }
        }

        public override string ToString()
        {
            return "MiStrAn Strucuture: Nodes: " + nodes.Count + ", Elements: " + elements.Count + ", Supports: " + supports.Count;
        }
    }
}
