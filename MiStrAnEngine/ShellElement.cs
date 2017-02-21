using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SF = MiStrAnEngine.StaticFunctions;

namespace MiStrAnEngine
{
    public class ShellElement
    {
        public List<Node> nodes;
        public int Id;
        public double thickness;
        public Matrix eq; // [eqx eqy eqz]
        public Matrix D;
        public Matrix q;


        public ShellElement(List<Node> _nodes, int _id)
        {
            nodes = _nodes;
            Id = _id;
        }


       

        public bool GenerateKefe(out Matrix Ke, out Vector fe)
        {
            Ke = new Matrix(18, 18);
            fe = new Vector(18);

            Matrix B, N, gp, gw, xe, T; //,q;

            int ng = 4; // Number of gauss points

            GenerateGaussPoints(ng, out gp, out gw);
           
            GetLocalNodeCoordinates(out xe, out T);

            int[] activeDofs = new int[] { 0, 1, 2, 3, 4, 6, 7, 8, 9, 10, 12, 13, 14, 15, 16 };
            int[] passiveDofs = new int[] { 5, 11, 17 };

            double b1 = xe[1, 1] - xe[2, 1];
            double b2 = xe[2, 1] - xe[0, 1];
            double c1 = xe[2, 0] - xe[1, 0];
            double c2 = xe[0, 0] - xe[2, 0];
            double elementArea = 0.5 * (b1 * c2 - b2 * c1);

            for (int i = 0; i < ng; i++)
            {
                GetB_N(gp.GetRow(i), xe,out B, out N);
                Matrix DKe = gw[i]* B.Transpose() * D * B;
                Ke[activeDofs, activeDofs] = Ke[activeDofs, activeDofs] + elementArea*DKe; 
                Matrix DMe= gw[i] * N.Transpose() * q;
                fe[activeDofs] = fe[activeDofs] + elementArea*DMe.ToVector();
            }
        
            Ke[passiveDofs, passiveDofs] = Matrix.Ones(3, 3); //Ke.Max() * Matrix.Ones(3, 3);

            Ke = T * Ke * T.Transpose();

            return true;
        }

        public void SetSteelSection()
        {
            double E = 210e9;
            double v = 0.3;
            double G = E / (2.0 * (1 + v));
            double density = 0;//7800*1000; //[kg/m^3]
            Matrix d = new Matrix(6, 6);
            Matrix qLoc = new Matrix(6, 1);

            double[] angle = new double[1] { 0}; // double[] angle = new double[] { 0};
            Materials.eqModulus(E, E, G, v, angle, thickness, density, out d, out qLoc);
            this.D = d;
            this.q = qLoc;

        }

        private static void GenerateGaussPoints(int n, out Matrix gp, out Matrix gw)
        {

            if (n == 1)
            {
                gp = new Matrix(new double[,] { { 1.0 / 3.0, 1.0 / 3.0, 1.0 / 3.0 } });
                gw = new Matrix(1, 1);
                gw[0] = 1;
            }

            else if (n == 3)
            {
                gp = new Matrix(new double[,] { { 1.0 / 2.0, 1.0 / 2.0, 0 }, { 1.0 / 2.0, 0, 1.0 / 2.0 }, { 0, 1.0 / 2.0, 1.0 / 2.0 } });
                gw = new Matrix(new double[,] { { 1.0 / 3.0, 1.0 / 3.0, 1.0 / 3.0 } });
            }
            else if (n == 4)
            {
                gp = new Matrix(new double[,] {
                    { 1.0 / 3.0, 1.0 / 3.0, 1.0 / 3.0 },
                    { 0.6, 0.2, 0.2 },
                    { 0.2, 0.6, 0.2 },
                    { 0.2, 0.2, 0.6 } });

                gw = new Matrix(new double[,] { { -27.0 / 48.0, 25.0 / 48.0, 25.0 / 48.0, 25.0 / 48.0 } });
            }
            else if (n == 7)
            {
                double alpha1 = 0.0597158717;
                double beta1 = 0.4701420641;
                double alpha2 = 0.7974269853;
                double beta2 = 0.1012865073;
                gp = new Matrix(new double[,] {
                    { 1.0 / 3.0, 1.0 / 3.0, 1.0 / 3.0 },
                    { alpha1, beta1, beta1 },
                    { beta1, alpha1, beta1 },
                    { beta1, beta1, alpha1 },
                    { alpha2, beta2, beta2 },
                    { beta2, alpha2, beta2 },
                    { beta2, beta2, alpha2 } });
                gw = new Matrix(new double[,] { { 0.2250000000, 0.1323941527, 0.1323941527,
                        0.1323941527, 0.1259391805, 0.1259391805, 0.1259391805 } });
            }

            else
                throw new MException("Number of gauss points is out of range");

        }

        public void ShellTesting()
        {
            Vector3D x1 = nodes[0].Pos;
            Vector3D x2 = nodes[1].Pos;
            Vector3D x3 = nodes[2].Pos;

            Vector3D gr = -x1 + x2;
            Vector3D gs = -x1 + x3;

            Matrix B1 = new Matrix(3,6);

            B1[0, 0] = -gr.X;
            B1[0, 1] = -gr.Y;
            B1[0, 2] = gr.X;
            B1[0, 3] = gr.Y;

            B1[1, 0] = -gs.X;
            B1[1, 1] = -gs.Y;
            B1[1, 4] = gs.X;
            B1[1, 5] = gs.Y;

            B1[2, 0] = -gs.X - gr.X;
            B1[2, 1] = -gs.Y - gr.Y;
            B1[2, 2] = gs.X;
            B1[2, 3] = gs.Y;
            B1[2, 4] = gr.X;
            B1[2, 5] = gr.Y;


            double ex1 = x1.X;
            double ex2 = x2.X;
            double ex3 = x3.X;

            double ey1 = x1.Y;
            double ey2 = x2.Y;
            double ey3 = x3.Y;

            Matrix A = new Matrix(new double[,] { { gr.X, gr.Y }, { gs.X, gs.Y } });
            Matrix g_r = A.SolveWith(new Matrix(new double[,] { { 0 }, { 1 } }));
            Matrix g_s = A.SolveWith(new Matrix(new double[,] { { 1 }, { 0 } }));

            Vector3D Lr = new Vector3D();
            Lr.X = gs.Y; Lr.Y = -gs.X;
            Vector3D Ls = new Vector3D(Lr.Y, -Lr.X, 0);
            Ls = -Ls;
            //Matrix T = new Matrix(new double[,] {
            //    { Vector.DotProduct(Lr, gr), Vector.DotProduct(Lr, gs),  },
            //    { Vector.DotProduct(Ls, gr), Vector.DotProduct(Ls, gs),  },

            //});

            Matrix T = new Matrix(2, 2);
            T.SetCol(g_s, 0);
            T.SetCol(g_r, 1);

            Matrix Tg = new Matrix(6, 6);
            int[] rng1 = SF.intSrs(0, 1);
            int[] rng2 = SF.intSrs(2, 3);
            int[] rng3 = SF.intSrs(4, 5);

            Tg[rng1, rng1] = T;
            Tg[rng2, rng2] = T;
            Tg[rng3, rng3] = T;

            B1 = B1 * Tg;

            Matrix C = new Matrix(new double[,] { 
                { 1, ex1, ey1, 0, 0, 0 }, 
                { 0, 0, 0, 1, ex1, ey1 }, 
                { 1, ex2, ey2, 0, 0, 0 }, 
                { 0, 0, 0, 1, ex2, ey2 }, 
                { 1, ex3, ey3, 0, 0, 0 }, 
                { 0, 0, 0, 1, ex3, ey3 } });

            Matrix B2 = new Matrix(3, 6);
            B2[0, 1] = 1;
            B2[1, 5] = 1;
            B2[2, 2] = 1;
            B2[2, 4] = 1;

            B2 = B2 * C.Invert();





        }

        public int[] GetElementDofs()
        {
            int[] dofs = new int[]
           {nodes[0].dofX, nodes[0].dofY, nodes[0].dofZ, nodes[0].dofXX, nodes[0].dofYY, nodes[0].dofZZ,
            nodes[1].dofX, nodes[1].dofY, nodes[1].dofZ, nodes[1].dofXX, nodes[1].dofYY, nodes[1].dofZZ,
            nodes[2].dofX, nodes[2].dofY, nodes[2].dofZ, nodes[2].dofXX, nodes[2].dofYY, nodes[2].dofZZ };

            return dofs;
        }


        // See slides p.44
        public void GetLocalNodeCoordinates(out Matrix xel, out Matrix Tg)
        {
            Vector3D v1 = nodes[1].Pos - nodes[0].Pos;
            Vector3D _v2 = nodes[2].Pos - nodes[0].Pos;
            Vector3D v3 = Vector3D.CrossProduct(v1, _v2);
            Vector3D v2 = Vector3D.CrossProduct(v3, v1);

            Vector3D e1 = v1.Normalize(false);
            Vector3D e2 = v2.Normalize(false);
            Vector3D e3 = v3.Normalize(false);

            Matrix xeg = new Matrix(3, 3);
            xeg.SetRow(v1.ToMatrix().Transpose(), 1);
            xeg.SetRow(_v2.ToMatrix().Transpose(), 2);

            Matrix T = new Matrix(3, 3);
            T.SetCol(e1.ToMatrix(), 0);
            T.SetCol(e2.ToMatrix(), 1);
            T.SetCol(e3.ToMatrix(), 2);


            xel = xeg * T;

            Tg = new Matrix(18, 18);
            int[] pos1 = SF.intSrs(0, 2);
            int[] pos2 = SF.intSrs(3, 5);
            int[] pos3 = SF.intSrs(6, 8);
            int[] pos4 = SF.intSrs(9, 11);
            int[] pos5 = SF.intSrs(12, 14);
            int[] pos6 = SF.intSrs(15, 17);
            Tg[pos1, pos1] = T;
            Tg[pos2, pos2] = T;
            Tg[pos3, pos3] = T;
            Tg[pos4, pos4] = T;
            Tg[pos5, pos5] = T;
            Tg[pos6, pos6] = T;


        }


        //Temp public for testing
        public void GetB_N(Matrix L, Matrix xe, out Matrix B, out Matrix N)
        {
            //xe is the transformed coordinates

            double L1 = L[0];// first element
            double L2 = L[1]; //second
            double L3 = L[2]; //third

            double b1 = xe[1, 1] - xe[2, 1];
            double b2 = xe[2, 1] - xe[0, 1];
            double b3 = xe[0, 1] - xe[1, 1];
            double c1 = xe[2, 0] - xe[1, 0];
            double c2 = xe[0, 0] - xe[2, 0];
            double c3 = xe[1, 0] - xe[0, 0];
            double delta = 0.5 * (b1 * c2 - b2 * c1);

            //Lengths of edges (z will be 0 in these cases)
            double l1 = Math.Sqrt(Math.Pow(xe[0, 0] - xe[1, 0], 2) + Math.Pow(xe[0, 1] - xe[1, 1], 2));
            double l2 = Math.Sqrt(Math.Pow(xe[1, 0] - xe[2, 0], 2) + Math.Pow(xe[1, 1] - xe[2, 1], 2));
            double l3 = Math.Sqrt(Math.Pow(xe[2, 0] - xe[0, 0], 2) + Math.Pow(xe[2, 1] - xe[0, 1], 2));

            

            double mu3 = (Math.Pow(l3, 2) - Math.Pow(l2, 2)) / Math.Pow(l1, 2);
            double mu1 = (Math.Pow(l1, 2) - Math.Pow(l3, 2)) / Math.Pow(l2, 2);
            double mu2 = (Math.Pow(l2, 2) - Math.Pow(l1, 2)) / Math.Pow(l3, 2);

            Matrix A = new Matrix(new double[,] { { b1, b2, b3 }, { c1, c2, c3 } });
            A = (1 / (2 * delta)) * A;

            //First parts of N matrix
            double N1 = L1;
            double N2 = L2;
            double N3 = L3;


            double dN1dx = b1 / (2 * delta);
            double dN1dy = c1 / (2 * delta);
            double dN2dx = b2 / (2 * delta);
            double dN2dy = c2 / (2 * delta);
            double dN3dx = b3 / (2 * delta);
            double dN3dy = c3 / (2 * delta);


            Matrix P = new Matrix(new double[,] { { L1 }, {L2}, {L3 }, {L1*L2 }, { L2*L3}, {L3*L1 },
                                                {Math.Pow(L1,2)*L2+0.5*L1*L2*L3*(3*(1-mu3)*L1-(1+3*mu3)*L2+(1+3*mu3)*L3)},
                                                {Math.Pow(L2,2)*L3+0.5*L1*L2*L3*(3*(1-mu1)*L2-(1+3*mu1)*L3+(1+3*mu1)*L1)},
                                                {Math.Pow(L3,2)*L1+0.5*L1*L2*L3*(3*(1-mu2)*L3-(1+3*mu2)*L1+(1+3*mu2)*L2)}});

            //For the N-matrix  %Danskarnas bok volym 2 p.133 eq 4.64
            double NN11 = P[0] - P[3] + P[5] + 2 * (P[6] - P[8]);
            double NN12 = -b2 * (P[8] - P[5]) - b3 * P[6];
            double NN13 = -c2 * (P[8] - P[5]) - c3 * P[6];
            double NN21 = P[1] - P[4] + P[3] + 2 * (P[7] - P[6]);
            double NN22 = -b3 * (P[6] - P[3]) - b1 * P[7];
            double NN23 = -c3 * (P[6] - P[3]) - c1 * P[7];
            double NN31 = P[2] - P[5] + P[4] + 2 * (P[8] - P[7]);
            double NN32 = -b1 * (P[7] - P[4]) - b2 * P[8];
            double NN33 = -c1 * (P[7] - P[4]) - c2 * P[8];

            N = new Matrix(new double[,] { { N1, 0, 0, 0, 0, N2, 0, 0, 0, 0, N3, 0, 0, 0, 0 },
                                                { 0,N1,0,0,0,0,N2,0,0,0,0,N3,0,0,0},
                                                { 0,0,NN11,NN12,NN13,0,0,NN21,NN22,NN23,0,0,NN31,NN32,NN33} });

            //each ddP is a column vector
            double[] ddP1 = new double[] {0,0,0,0,0,0,2*L2+L2*L3*3*(1-mu3),L2*L3*(1+3*mu1),-L2*L3*(1+3*mu2) };
            double[] ddP2 = new double[] { 0, 0, 0, 0, 0, 0, -L1*L3*(1+3*mu3),2*L3+L1*L3*3*(1-mu1),L1*L3*(1+3*mu2) };
            double[] ddP3 = new double[] { 0, 0, 0, 0, 0, 0,L1*L2*(1+3*mu3),-L1*L2*(1+3*mu1),2*L1+L1*L2*3*(1-mu2) };
            double[] ddP4 = new double[] { 0, 0, 0, 1, 0, 0, 2*L1+L1*L3*3*(1-mu3)-L2*L3*(1+3*mu3)+0.5*Math.Pow(L3,2)*(1+3*mu3),L2*L3*3*(1-mu1)-0.5*Math.Pow(L3,2)*(1+3*mu1)+L1*L3*(1+3*mu1),0.5*Math.Pow(L3,2)*3*(1-mu2)-L1*L3*(1+3*mu2)+L2*L3*(1+3*mu2) };
            double[] ddP5 = new double[] { 0, 0, 0, 0, 0, 1,L1*L2*3*(1-mu3)-0.5*Math.Pow(L2,2)*(1+3*mu3)+L2*L3*(1+3*mu3),0.5*Math.Pow(L2,2)*3*(1-mu1)-L2*L3*(1+3*mu1)+L1*L2*(1+3*mu1),2*L3+L2*L3*3*(1-mu2)-L1*L2*(1+3*mu2)+0.5*Math.Pow(L2,2)*(1+3*mu2)  };
            double[] ddP6 = new double[] { 0, 0, 0, 0, 1, 0,0.5*Math.Pow(L1,2)*3*(1-mu3)-L1*L2*(1+3*mu3)+L1*L3*(1+3*mu3),2*L2+L1*L2*3*(1-mu1)-L1*L3*(1+3*mu1)+0.5*Math.Pow(L1,2)*(1+3*mu1),L1*L3*3*(1-mu2)-0.5*Math.Pow(L1,2)*(1+3*mu2)+L1*L2*(1+3*mu2)  };

            //Snygga till sen
            List<double[]> ddPs = new List<double[]>();
            ddPs.Add(ddP1); ddPs.Add(ddP2); ddPs.Add(ddP3); ddPs.Add(ddP4); ddPs.Add(ddP5); ddPs.Add(ddP6);

            
            //JAG SKA GÖRA EN FUNKTION AV DETTA SEN MEN JAG LÅTER DET VARA FÖR TILLFÄLLET

            //For the B matrix
            double[] N11 = new double[6];
            for(int i =0; i<ddPs.Count;i++)
            {
                double[] ddP = ddPs[i];
                double dd = ddP[0] - ddP[3] + ddP[5] + 2 *( ddP[6] - ddP[8]);
                N11[i] = dd;
            }

            Matrix ddN11 = new Matrix(new double[,] { { N11[0], N11[3], N11[4] }, { N11[3], N11[1], N11[5] }, { N11[4], N11[5], N11[2] } });

            double[] N12 = new double[6];
            for (int i = 0; i < ddPs.Count; i++)
            {
                double[] ddP = ddPs[i];
                double dd = -b2 * (ddP[8] - ddP[5]) - b3 * ddP[6];
                N12[i] = dd;
            }

            Matrix ddN12 = new Matrix(new double[,] { { N12[0], N12[3], N12[4] }, { N12[3], N12[1], N12[5] }, { N12[4], N12[5], N12[2] } });

            double[] N13 = new double[6];
            for (int i = 0; i < ddPs.Count; i++)
            {
                double[] ddP = ddPs[i];
                double dd = -c2 *( ddP[8] - ddP[5]) - c3 * ddP[6];
                N13[i] = dd;
            }

            Matrix ddN13 = new Matrix(new double[,] { { N13[0], N13[3], N13[4] }, { N13[3], N13[1], N13[5] }, { N13[4], N13[5], N13[2] } });

            double[] N21 = new double[6];
            for (int i = 0; i < ddPs.Count; i++)
            {
                double[] ddP = ddPs[i];
                double dd =ddP[1] - ddP[4]+ ddP[3]+2*(ddP[7]-ddP[6]);
                N21[i] = dd;
            }

            Matrix ddN21 = new Matrix(new double[,] { { N21[0], N21[3], N21[4] }, { N21[3], N21[1], N21[5] }, { N21[4], N21[5], N21[2] } });

            double[] N22 = new double[6];
            for (int i = 0; i < ddPs.Count; i++)
            {
                double[] ddP = ddPs[i];
                double dd = -b3 *( ddP[6] - ddP[3]) - b1 * ddP[7];
                N22[i] = dd;
            }

            Matrix ddN22 = new Matrix(new double[,] { { N22[0], N22[3], N22[4] }, { N22[3], N22[1], N22[5] }, { N22[4], N22[5], N22[2] } });

            double[] N23 = new double[6];
            for (int i = 0; i < ddPs.Count; i++)
            {
                double[] ddP = ddPs[i];
                double dd = -c3 *( ddP[6] - ddP[3]) - c1 * ddP[7];
                N23[i] = dd;
            }

            Matrix ddN23 = new Matrix(new double[,] { { N23[0], N23[3], N23[4] }, { N23[3], N23[1], N23[5] }, { N23[4], N23[5], N23[2] } });

            double[] N31 = new double[6];
            for (int i = 0; i < ddPs.Count; i++)
            {
                double[] ddP = ddPs[i];
                double dd = ddP[2] - ddP[5] + ddP[4]+2*(ddP[8]-ddP[7]);
                N31[i] = dd;
            }

            Matrix ddN31 = new Matrix(new double[,] { { N31[0], N31[3], N31[4] }, { N31[3], N31[1], N31[5] }, { N31[4], N31[5], N31[2] } });

            double[] N32 = new double[6];
            for (int i = 0; i < ddPs.Count; i++)
            {
                double[] ddP = ddPs[i];
                double dd = -b1*(ddP[7] - ddP[4]) -b2*ddP[8];
                N32[i] = dd;
            }

            Matrix ddN32 = new Matrix(new double[,] { { N32[0], N32[3], N32[4] }, { N32[3], N32[1], N32[5] }, { N32[4], N32[5], N32[2] } });

            double[] N33 = new double[6];
            for (int i = 0; i < ddPs.Count; i++)
            {
                double[] ddP = ddPs[i];
                double dd = -c1 *( ddP[7] - ddP[4]) - c2 * ddP[8];
                N33[i] = dd;
            }

            Matrix ddN33 = new Matrix(new double[,] { { N33[0], N33[3], N33[4] }, { N33[3], N33[1], N33[5] }, { N33[4], N33[5], N33[2] } });

            ddN11 = A * ddN11 * A.Transpose();
            ddN12 = A * ddN12 * A.Transpose();
            ddN13 = A * ddN13 * A.Transpose();
            ddN21 = A * ddN21 * A.Transpose();
            ddN22 = A * ddN22 * A.Transpose();
            ddN23 = A * ddN23 * A.Transpose();
            ddN31 = A * ddN31 * A.Transpose();
            ddN32 = A * ddN32 * A.Transpose();
            ddN33 = A * ddN33 * A.Transpose();

            B = new Matrix(new double[,] { { dN1dx,0,0,0,0,dN2dx,0,0,0,0,dN3dx,0,0,0,0},
                                                  {0,dN1dy,0,0,0,0,dN2dy,0,0,0,0,dN3dy,0,0,0 },
                                                  {dN1dy,dN1dx,0,0,0,dN2dy,dN2dx,0,0,0,dN3dy,dN3dx,0,0,0 },
                                                  {0,0,ddN11[0,0],ddN12[0,0],ddN13[0,0],0,0,ddN21[0,0],ddN22[0,0],ddN23[0,0],0,0,ddN31[0,0],ddN32[0,0],ddN33[0,0] },
                                                  {0,0,ddN11[1,1],ddN12[1,1],ddN13[1,1],0,0,ddN21[1,1],ddN22[1,1],ddN23[1,1],0,0,ddN31[1,1],ddN32[1,1],ddN33[1,1] },
                                                  {0,0,2*ddN11[0,1],2*ddN12[0,1],2*ddN13[0,1],0,0,2*ddN21[0,1],2*ddN22[0,1],2*ddN23[0,1],0,0,2*ddN31[0,1],2*ddN32[0,1],2*ddN33[0,1] } });

           
        }


    }
}
