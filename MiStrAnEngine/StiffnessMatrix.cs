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
        // D MATRIX BASED ON HOOKE PLANE STRESS (OTHERWISE CHANGE)

        
        //BASED ON PLANRE
        public static Matrix planre(ShellElement shell, double thickness,Matrix D)
        {
            //Add so we can get the list with shell elements from the structure
            
                List<Node> shellNodes = shell.nodes;

                //GÖR EN KONTROLL SÅ ATT DET VERKLIGEN BARA ÄR 4 (3 I FRAMTIDEN) NODER I DENNA LISTA

                //Det är konstigt att bara punkt 1 och 3 är med, men alla punkter används sen i Kirchoff och det baseras på rektangulära element
                double a = (shellNodes[2].x - shellNodes[0].x) / 2;
                double b = (shellNodes[2].y - shellNodes[0].y) / 2;

                Matrix xgp = new Matrix(new double[,] { { -1, 1, 1, -1 }});
                xgp = (1 / Math.Sqrt(3)) * xgp;

                Matrix ygp = new Matrix(new double[,] { { -1, -1, 1, 1 } });
                ygp = (1 / Math.Sqrt(3)) * ygp;

              

                Matrix Ke = Matrix.ZeroMatrix(8, 8); //8x8 matrix
                
                
                for(int i=0;i<4;i++)
                {
                    double x = a * xgp[0, i];
                    double y = b * ygp[0, i];
                    Matrix B = new Matrix(new double[,] { { -(b-y), 0, b-y,0,b+y,0,-(b+y),0 }, { 0, -(a-x), 0, -(a+x), 0, a+x,0,a-x }, { -(a-x), -(b-y), -(a+x),b-y,a+x,b+y,a-x, (-b+y) } });
                    B = 1/(4*a*b)*B;

                    Ke = Ke + a * b * thickness*Matrix.Transpose(B) * D * B;
                }

            // Matrix fe = new Matrix(new double[,] { {bx}, {by}, {bx}, { by }, { bx }, { by }, { bx }, { by } });
            Matrix fe = new Matrix(new double[,] { { 0 }, { 0 }, { 0 }, { 0 }, { 0 }, { 0 }, { 0 }, { 0 } });
            fe = a * b * thickness * fe;

            return new Matrix(1,1);
        }

    }
}
