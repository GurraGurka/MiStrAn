using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiStrAnEngine
{
    public class solveq
    {
        //Main script, gets the indata from Grasshopper and a mistran-structure

            //also need material indata from Grasshopper in future
        public static Matrix solveqMat(Structure struc)
        {
            //TEMP MATERIAL DATA             //THIS SHOULD BE AN INPUT FROM GRASSHOPPER
            double thickness = 0.1;
            double irb = 2;
            double irs = 1;

            double E = 210 * Math.Pow(10, 9);
            double v = 0.3; //Maybe make this an input

            double bx = 0; //Body mass in y-dir
            double by = 0; //Body mass in x-dir
            double qz = 1000; //BODY LOAD IN Z-DIR


            //Constitutive matrix CHANGE EQS IF D IS NOT PLANE STRESS
            Matrix D = new Matrix(new double[,] { { 1, v, 0 }, { v, 1, 0 }, { 0, 0, (1 - v) / 2 } });
            D = E / (1 - Math.Pow(v, 2)) * D;

            Matrix G = new Matrix(new double[,] { { 1, 0 }, { 0, 1 } });
            G = E / (2 + 2 * v) * G;


            //DETTA SKA TYP IN I GENERATE K


            ////Global K- and F matrix
            //int nbDofs = 0; //HÄMTA FRÅN EDOF
            //Matrix K = Matrix.ZeroMatrix(nbDofs, nbDofs);

            //List<ShellElement> shells = struc.Shells;
            //double[] ep = new double[3] { thickness, irb, irs };

            //foreach (ShellElement shell in shells)
            //{
            //    Matrix KeFe = shell2re(shell, ep, D, G, qz);
            //    //LÄGG IN KeFe i K och f mha Edof               
            //}

            //Matrix m = new Matrix(3, 3);
            ////Här ska K och f skickas tillbaka
            return new Matrix(1, 1);
        }

        

        //Disregard a case with no bc
        //SOLVEQ, SKA KOLLA VILKA MATRISFUNKTIONER VI HAR. KAN MAN TA RANGE MED DETTA MATRISBIBLIOTEK

    }
}
