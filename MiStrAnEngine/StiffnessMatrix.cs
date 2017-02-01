using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiStrAnEngine
{
    class StiffnessMatrix
    {
        //Obtain element stiffness and force contribution for the global stiffness and force matrix.
        //Only Kirchoff plate, need stiffness contribution for Melosh and Kirchoff (from assignment3)

        //BASED ON 4 POINT ELEMENTS
        //PLANE STRESS CHOSEN, WE CAN CHANGE TO PLANE STRAIN ALSO
        //NO BODY FORCE IN X- AND Y-DIRECTION (SAME AS ASSIGNMENT 3)
        // D MATRIX BASED ON HOOKE PLANE STRESS

        //THIS SHOULD BE AN INPUT FROM GRASSHOPPER
        double E = 210 * Math.Pow(10, 9);
        double v = 0.3; //Maybe make this input

        public static Matrix GetMeloshMatrix(Structure struc)
        {
            //Add so we can get the list with shell elements from the structure
            List<ShellElement> shellList = struc.shells;


            foreach (ShellElement shell in shellList)
            {
                List<Node> shellNodes = shell.nodes;

                //GÖR EN KONTROLL SÅ ATT DET VERKLIGEN BARA ÄR 4 (3 I FRAMTIDEN) NODER I DENNA LISTA

                //Det är konstigt att bara punkt 1 och 3 är med, men alla punkter används sen i Kirchoff och det baseras på rektangulära element
                double a = (shellNodes[2].x - shellNodes[0].x) / 2;
                double b = (shellNodes[2].y - shellNodes[0].y) / 2;

                Matrix xgp = new Matrix(new double[,] { { -1, 1, 1, -1 }});
                xgp = (1 / Math.Sqrt(3)) * xgp;

                Matrix ygp = new Matrix(new double[,] { { -1, -1, 1, 1 } });
                ygp = (1 / Math.Sqrt(3)) * ygp;

                //Constitutive matrix
                Matrix D = E/(1-Math.Pow(v,2))*
            }

            return new Matrix(1,1);
        }

    }
}
