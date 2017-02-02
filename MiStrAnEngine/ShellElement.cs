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
        public double t;
        public Matrix eq; // [eqx eqy eqz]
        public Matrix D;

        public ShellElement(List<Node> _nodes, int _id)
        {
            nodes = _nodes;
            Id = _id;
        }

        public bool GenerateKefe(out Matrix Ke, out Matrix fe)
        {
            planre(out Ke, out fe);

            Matrix dofPlan = new Matrix(new double[,] { { 1, 2, 6, 7, 11, 12, 16, 17 } });
            Matrix dofPlate = new Matrix(new double[,] { { 3, 4, 5, 8, 9, 10, 13, 14, 15, 18, 19, 20 } });

            return true;
        }


        // Direct copy of CALFEM's platre
        // FÄRDIG OCH TESTAD 2017-02-01 18:06
        private void platre(out Matrix Ke, out Matrix fe)
        {

            double Lx = nodes[2].x - nodes[0].x; double Ly = nodes[2].y - nodes[0].y;

            Matrix _D = (Math.Pow(t,3) / 12) * D;

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

        // Direct copy of CALFEMS planre
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

                Ke = Ke + a * b * t * Matrix.Transpose(B) * D * B;
            }

            fe = new Matrix(new double[,] { {bx}, {by}, {bx}, { by }, { bx }, { by }, { bx }, { by } });
            fe = a * b * t * fe;
        }

    }
}
