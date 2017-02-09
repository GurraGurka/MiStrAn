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


        public ShellElement(List<Node> _nodes, int _id)
        {
            nodes = _nodes;
            Id = _id;
        }

       

        public bool GenerateKefe(out Matrix Ke, out Matrix fe)
        {
            Ke = new Matrix(18, 18);
            fe = new Matrix(18, 1);

            Matrix B, gp, gw;
            double xe1, xe2, xe3, ye1, ye2, ye3;
            
            int nnDof = 6; // Degrees of freedom per node
            int en = 3; //number of nodes per element
            int ng = 4; // Number of gauss points

            GenerateGaussPoints(ng, out gp, out gw);
           
            GetLocalNodeCoordinates(out xe1, out xe2, out xe3, out ye1, out ye2, out ye3);
            Matrix xe = new Matrix(new double[,] { { xe1, ye1, 0 }, { xe2, ye2, 0 }, { xe2, ye2, 0 } });

            int[] activeDofs = new int[] { 0, 1, 2, 3, 4, 6, 7, 8, 9, 10, 12, 13, 14, 15, 16 };
            int[] passiveDofs = new int[] { 5, 11, 17 };

            for (int i = 0; i < ng; i++)
            {
                B = GetB(gp.GetRow(i), xe);

                Ke[activeDofs, activeDofs] = Ke[activeDofs, activeDofs] + B.Transpose() * D * B;
            }



            return true;
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
            Vector x1 = nodes[0].Pos;
            Vector x2 = nodes[1].Pos;
            Vector x3 = nodes[2].Pos;

            Vector gr = -x1 + x2;
            Vector gs = -x1 + x3;

            Matrix B1 = new Matrix(3,6);

            B1[0, 0] = -gr.X;
            B1[0, 1] = -gr.Y;
            B1[0, 2] = gr.X;
            B1[0, 3] = gr.Y;

            B1[1, 0] = -gs.X;
            B1[1, 1] = -gs.Y;
            B1[1, 4] = gs.X;
            B1[1, 5] = gs.Y;

            double ex1 = x1.X;
            double ex2 = x2.X;
            double ex3 = x3.X;

            double ey1 = x1.Y;
            double ey2 = x2.Y;
            double ey3 = x3.Y;

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
           {nodes[0].dofX, nodes[0].dofY, nodes[0].dofZ, nodes[0].dofXX, nodes[0].dofYY,
            nodes[1].dofX, nodes[1].dofY, nodes[1].dofZ, nodes[1].dofXX, nodes[1].dofYY,
            nodes[2].dofX, nodes[2].dofY, nodes[2].dofZ, nodes[2].dofXX, nodes[2].dofYY,
            nodes[3].dofX, nodes[3].dofY, nodes[3].dofZ, nodes[3].dofXX, nodes[3].dofYY};

            return dofs;
        }

        public void GetLocalNodeCoordinates(out double xe1, out double xe2, out double xe3,
            out double ye1, out double ye2, out double ye3)
        {
            Vector centroid = (nodes[0].Pos + nodes[1].Pos + nodes[2].Pos) / 3;

            Vector e1 = centroid - nodes[0].Pos;
            e1.Normalize();

            Vector e3 = Vector.CrossProduct(nodes[1].Pos - nodes[0].Pos, nodes[2].Pos - nodes[0].Pos);
            e3.Normalize();
            Vector e2 = Vector.CrossProduct(e3, e1);

            // Relative positions
            Vector relPos1 = nodes[0].Pos - centroid;
            Vector relPos2 = nodes[1].Pos - centroid;
            Vector relPos3 = nodes[2].Pos - centroid;

            Matrix T = new Matrix(new double[,] {
                { Vector.DotProduct(e1, Vector.e1), Vector.DotProduct(e1, Vector.e2), Vector.DotProduct(e1, Vector.e3) },
                { Vector.DotProduct(e2, Vector.e1), Vector.DotProduct(e2, Vector.e2), Vector.DotProduct(e2, Vector.e3) },
                { Vector.DotProduct(e3, Vector.e1), Vector.DotProduct(e3, Vector.e2), Vector.DotProduct(e3, Vector.e3) }
            });

            Vector localPos1 = (T * relPos1.ToMatrix()).ToVector();
            Vector localPos2 = (T * relPos2.ToMatrix()).ToVector();
            Vector localPos3 = (T * relPos3.ToMatrix()).ToVector();

            xe1 = localPos1.X;
            xe2 = localPos2.X;
            xe3 = localPos3.X;

            ye1 = localPos1.Y;
            ye2 = localPos2.Y;
            ye3 = localPos3.Y;
        }

        public Matrix GetL()
        {
            //double x1 = nodes[0].x;
            //double x1 = nodes[0].x;
            //double x1 = nodes[0].x;

            //double x1 = nodes[0].x;
            //double x1 = nodes[0].x;
            //double x1 = nodes[0].x;



            return Matrix.ZeroMatrix(1, 1);
        }


        // Direct copy of CALFEM's platre
        // FÄRDIG OCH TESTAD 2017-02-01 18:06
        private void platre(out Matrix Ke, out Matrix fe)
        {

            double Lx = nodes[2].x - nodes[0].x; double Ly = nodes[2].y - nodes[0].y;

            Matrix _D = (Math.Pow(thickness,3) / 12) * D;

            double A1 = Ly / Math.Pow(Lx,3); double A2 = Lx / Math.Pow(Ly, 3); double A3 = 1 / (Lx * Ly);
            double A4 = Ly / Math.Pow(Lx, 2); double A5 = Lx / Math.Pow(Ly, 2); double A6 = 1 / Lx;
            double A7 = 1 / Ly; double A8 = Ly / Lx; double A9 = Lx / Ly;

            double C1 = 4 * A1 * _D[0, 0] + 4 * A2 * _D[1, 1] + 2 * A3 * _D[0, 1] + 5.6 * A3 * _D[2, 2];


            double C2 = -4 * A1 * _D[0, 0] + 2 * A2 * _D[1, 1] - 2 * A3 * _D[0, 1] - 5.6 * A3 * _D[2, 2];
            double C3 = 2 * A1 * _D[0, 0] - 4 * A2 * _D[1, 1] - 2 * A3 * _D[0, 1] - 5.6 * A3 * _D[2, 2];
            double C4 = -2 * A1 * _D[0, 0] - 2 * A2 * _D[1, 1] + 2 * A3 * _D[0, 1] + 5.6 * A3 * _D[2, 2];
            double C5 = 2 * A5 * _D[1, 1] + A6 * _D[0, 1] + 0.4 * A6 * _D[2, 2];
            double C6 = 2 * A4 * _D[0, 0] + A7 * _D[0, 1] + 0.4 * A7 * _D[2, 2];

            double C7 = 2 * A5 * _D[1, 1] + 0.4 * A6 * _D[2, 2];
            double C8 = 2 * A4 * _D[0, 0] + 0.4 * A7 * _D[2, 2];
            double C9 = A5 * _D[1,1] - A6 * _D[0,1] - 0.4 * A6 * _D[2,2];
            double C10 = A4 * _D[0,0] - A7 * _D[0,1] - 0.4 * A7 * _D[2,2];
            double C11 = A5 * _D[1,1] - 0.4 * A6 * _D[2,2];
            double C12 = A4 * _D[0,0] - 0.4 * A7 * _D[2,2];

            double C13 = 4.0 / 3.0 * A9 * _D[1,1] + 8.0 / 15.0 * A8 * _D[2,2];
            double C14 = 4.0 / 3.0 * A8 * _D[0,0] + 8.0 / 15.0 * A9 * _D[2,2];
            double C15 = 2.0 / 3.0 * A9 * _D[1,1] - 8.0 / 15.0 * A8 * _D[2,2];
            double C16 = 2.0 / 3.0 * A8 * _D[0,0] - 8.0 / 15.0 * A9 * _D[2,2];
            double C17 = 2.0 / 3.0 * A9 * _D[1,1] - 2.0 / 15.0 * A8 * _D[2,2];
            double C18 = 2.0 / 3.0 * A8 * _D[0,0] - 2.0 / 15.0 * A9 * _D[2,2];
            double C19 = 1.0 / 3.0 * A9 * _D[1,1] + 2.0 / 15.0 * A8 * _D[2,2];
            double C20 = 1.0 / 3.0 * A8 * _D[0,0] + 2.0 / 15.0 * A9 * _D[2,2];
            double C21 = _D[0,1];

            Matrix Keq = Matrix.ZeroMatrix(12, 12);
            Keq[0, SF.intSrs(0, 11)] = new Matrix(new double[,] { { C1, C5, -C6, C2, C9, -C8, C4, C11, -C12, C3, C7, -C10 } });
            Keq[1, SF.intSrs(1, 11)] = new Matrix(new double[,] { { C13, -C21, C9, C15, 0, -C11, C19, 0, -C7, C17, 0 } });
            Keq[2, SF.intSrs(2, 11)] = new Matrix(new double[,] { { C14, C8, 0, C18, C12, 0, C20, -C10, 0, C16 } });
            Keq[3, SF.intSrs(3, 11)] = new Matrix(new double[,] { { C1, C5, C6, C3, C7, C10, C4, C11, C12 } });
            Keq[4, SF.intSrs(4, 11)] = new Matrix(new double[,] { { C13, C21, -C7, C17, 0, -C11, C19, 0 } });
            Keq[5, SF.intSrs(5, 11)] = new Matrix(new double[,] { { C14, C10, 0, C16, -C12, 0, C20, } });
            Keq[6, SF.intSrs(6, 11)] = new Matrix(new double[,] { { C1, -C5, C6, C2, -C9, C8 } });
            Keq[7, SF.intSrs(7, 11)] = new Matrix(new double[,] { { C13, -C21, -C9, C15, 0 } });
            Keq[8, SF.intSrs(8, 11)] = new Matrix(new double[,] { { C14, -C8, 0, C18, } });
            Keq[9, SF.intSrs(9, 11)] = new Matrix(new double[,] { { C1, -C5, -C6 } });
            Keq[10, SF.intSrs(10, 11)] = new Matrix(new double[,] { { C13, C21 } });
            Keq[11, 11] = C14;
            Keq = Matrix.Transpose(Keq) + Keq - Matrix.Diag(Matrix.Diag(Keq)); // remove double entries on diagonal


            double R1 =  (Lx * Ly / 4)*eq[0,2];
            double R2 = eq[0, 2] * Lx * Math.Pow(Ly, 2) / 24;
            double R3 = eq[0, 2] * Ly * Math.Pow(Lx , 2) / 24;

            Matrix feq = new Matrix(new double[,] { { R1, R2, -R3, R1, R2, R3, R1, -R2, R3, R1, -R2, -R3 } });

            fe = Matrix.Transpose(feq);
            Ke = Keq;
        }

        // Direct copy of CALFEM's planre
        // FÄRDIG OCH TESTAD 2017-02-02 11:30
        private void planre(out Matrix Ke, out Matrix fe)
        {
           
            //GÖR EN KONTROLL SÅ ATT DET VERKLIGEN BARA ÄR 4 (3 I FRAMTIDEN) NODER I DENNA LISTA

            //Det är konstigt att bara punkt 1 och 3 är med, men alla punkter används sen i Kirchoff och det baseras på rektangulära element
            // svar: Detta element förutsätter "element edges parallell to axis" - Gustav

            double a = (nodes[2].x - nodes[0].x) / 2;
            double b = (nodes[2].y - nodes[0].y) / 2;

            double bx = eq[0, 0];
            double by = eq[0, 1];

            Matrix xgp = new Matrix(new double[,] { { -1, 1, 1, -1 } });
            xgp = (1 / Math.Sqrt(3)) * xgp;

            Matrix ygp = new Matrix(new double[,] { { -1, -1, 1, 1 } });
            ygp = (1 / Math.Sqrt(3)) * ygp;



            Ke = Matrix.ZeroMatrix(8, 8); //8x8 matrix


            for (int i = 0; i < 4; i++)
            {
                double x = a * xgp[0, i];
                double y = b * ygp[0, i];
                Matrix B = new Matrix(new double[,] { { -(b - y), 0, b - y, 0, b + y, 0, -(b + y), 0 }, { 0, -(a - x), 0, -(a + x), 0, a + x, 0, a - x }, { -(a - x), -(b - y), -(a + x), b - y, a + x, b + y, a - x, -(b + y) } });
                B = 1 / (4 * a * b) * B;

                Ke = Ke + a * b * thickness * Matrix.Transpose(B) * D * B;
            }

            fe = new Matrix(new double[,] { {bx}, {by}, {bx}, { by }, { bx }, { by }, { bx }, { by } });
            fe = a * b * thickness * fe;
        }

        //Temp public for testing
        public Matrix GetB(Matrix L,Matrix xe)
        {
            //xe is the transformed coordinates

            double L1 = L[0,0];// first element
            double L2 = L[0,1]; //second
            double L3 = L[0,2]; //third

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

            double mu1 = (Math.Pow(l3, 2) - Math.Pow(l2,2)) / Math.Pow(l1, 2);
            double mu2 = (Math.Pow(l1, 2) - Math.Pow(l3,2)) / Math.Pow(l2, 2);
            double mu3 = (Math.Pow(l2, 2) - Math.Pow(l1,2)) / Math.Pow(l3, 2);

            double dN1dx=b1/(2*delta);
            double dN1dy=c1/(2*delta);
            double dN2dx = b2 / (2 * delta);
            double dN2dy = c2 / (2 * delta);
            double dN3dx = b3 / (2 * delta);
            double dN3dy = c3 / (2 * delta);

            return new Matrix(1, 1);
        }

    }
}
