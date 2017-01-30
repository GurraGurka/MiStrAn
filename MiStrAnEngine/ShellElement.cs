using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiStrAnEngine
{
    public class ShellElement
    {
        public List<Node> nodes;
        public int Id;
        public double t;

        public Matrix D;

        public ShellElement(List<Node> _nodes, int _id)
        {
            nodes = _nodes;
            Id = _id;
        }

        public bool GenerateKefe(out Matrix Ke, out Matrix fe)
        {
            Ke = Matrix.ZeroMatrix(20, 20);
            fe = Matrix.ZeroMatrix(20, 1);

            Matrix dofPlan = new Matrix(new double[,] { { 1, 2, 6, 7, 11, 12, 16, 17 } });
            Matrix dofPlate = new Matrix(new double[,] { { 3, 4, 5, 8, 9, 10, 13, 14, 15, 18, 19, 20 } });

            return true;
        }


        // Direct copy of CALFEM's planre
        private void planre(out Matrix Ke, out Matrix fe)
        {

            double Lx = nodes[2].x - nodes[0].x; double Ly = nodes[2].y - nodes[0].y;

            Matrix _D = (Math.Pow(t,3) / 12) * D;

            double A1 = Ly / Math.Pow(Lx,3); double A2 = Lx / Math.Pow(Ly, 3); double A3 = 1 / (Lx * Ly);
            double A4 = Ly / Math.Pow(Lx, 2); double A5 = Lx / Math.Pow(Ly, 2); double A6 = 1 / Lx;
            double A7 = 1 / Ly; double A8 = Ly / Lx; double A9 = Lx / Ly;

            double C1 = 4 * A1 * D[1, 1] + 4 * A2 * D[2, 2] + 2 * A3 * D[1, 2] + 5.6 * A3 * D[3, 3];


            double C2 = -4 * A1 * D[1, 1] + 2 * A2 * D[2, 2] - 2 * A3 * D[1, 2] - 5.6 * A3 * D[3, 3];
            double C3 = 2 * A1 * D[1, 1] - 4 * A2 * D[2, 2] - 2 * A3 * D[1, 2] - 5.6 * A3 * D[3, 3];
            double C4 = -2 * A1 * D[1, 1] - 2 * A2 * D[2, 2] + 2 * A3 * D[1, 2] + 5.6 * A3 * D[3, 3];
            double C5 = 2 * A5 * D[2, 2] + A6 * D[1, 2] + 0.4 * A6 * D[3, 3];
            double C6 = 2 * A4 * D[1, 1] + A7 * D[1, 2] + 0.4 * A7 * D[3, 3];

            double C7 = 2 * A5 * D[2, 2] + 0.4 * A6 * D[3, 3];
            double C8 = 2 * A4 * D[1, 1] + 0.4 * A7 * D[3, 3];
            double C9 = A5 * D[2, 2] - A6 * D[1, 2] - 0.4 * A6 * D[3, 3];
            double C10 = A4 * D[1, 1] - A7 * D[1, 2] - 0.4 * A7 * D[3, 3];
            double C11 = A5 * D[2, 2] - 0.4 * A6 * D[3, 3];
            double C12 = A4 * D[1, 1] - 0.4 * A7 * D[3, 3];

            double C13 = 4 / 3 * A9 * D[2, 2] + 8 / 15 * A8 * D[3, 3];
            double C14 = 4 / 3 * A8 * D[1, 1] + 8 / 15 * A9 * D[3, 3];
            double C15 = 2 / 3 * A9 * D[2, 2] - 8 / 15 * A8 * D[3, 3];
            double C16 = 2 / 3 * A8 * D[1, 1] - 8 / 15 * A9 * D[3, 3];
            double C17 = 2 / 3 * A9 * D[2, 2] - 2 / 15 * A8 * D[3, 3];
            double C18 = 2 / 3 * A8 * D[1, 1] - 2 / 15 * A9 * D[3, 3];
            double C19 = 1 / 3 * A9 * D[2, 2] + 2 / 15 * A8 * D[3, 3];
            double C20 = 1 / 3 * A8 * D[1, 1] + 2 / 15 * A9 * D[3, 3];
            double C21 = D[1, 2];

            Matrix Keq = Matrix.ZeroMatrix(12, 12);
            //            Keq(1, 1:12) =[C1 C5 - C6 C2 C9 - C8 C4 C11 - C12 C3 C7 - C10];
            //            Keq(2, 2:12) =[C13 - C21 C9 C15 0 - C11 C19 0 - C7 C17 0];
            //            Keq(3, 3:12) =[C14 C8 0 C18 C12 0 C20 - C10 0 C16];
            //            Keq(4, 4:12) =[C1 C5 C6 C3 C7 C10 C4 C11 C12];
            //            Keq(5, 5:12) =[C13 C21 - C7 C17 0 - C11 C19 0];
            //            Keq(6, 6:12) =[C14 C10 0 C16 - C12 0 C20];
            //            Keq(7, 7:12) =[C1 - C5 C6 C2 - C9 C8];
            //            Keq(8, 8:12) =[C13 - C21 - C9 C15 0];
            //            Keq(9, 9:12) =[C14 - C8 0 C18];
            //            Keq(10, 10:12) =[C1 - C5 - C6];
            //            Keq(11, 11:12) =[C13 C21];
            //            Keq(12, 12) =[C14];
            //            Keq = Keq'+Keq-diag(diag(Keq));
            //              %
            //if nargin == 5
            //R1 = eq * Lx * Ly / 4;
            //            R2 = eq * Lx * Ly ^ 2 / 24;
            //            R3 = eq * Ly * Lx ^ 2 / 24;
            //%
            //feq(:, 1) =[R1 R2 - R3 R1 R2 R3 R1 - R2 R3 R1 - R2 - R3]';
            //fe = feq;
            //            end
            //            Ke = Keq;

            Ke = Matrix.ZeroMatrix(1, 1);
            fe = Matrix.ZeroMatrix(1, 1);

        }

    }
}
