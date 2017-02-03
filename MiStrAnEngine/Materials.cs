using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiStrAnEngine
{
    public class Materials
    {
        //Make lists for E1, E2 etc
        public static Matrix eqModulus(double E1, double E2, double G12, double v12, double angle)
        {
            
            //Just loop through 4 times
            for(int i=0; i<4;i++)
            { 
                
                double v21 = v12 * E2 / E1; // Stiffness and strength analysis (Bokmärke) 4.2

                //Reduced stiffness terms (EUROCOMP eq4.50)
                double Q11 = E1 / (1 - v12 * v21);
                double Q12 = v21 * E1 / (1 - v12 * v21);
                double Q22 = E2 / (1 - v12 * v21);
                double Q66 = G12;

                //angle in degree
                double m = Math.Cos(angle * 2 * Math.PI / 360); //Math.Cos uses radians
                double n = Math.Sin(angle * 2 * Math.PI / 360); //Math.Cos uses radians
            }

            return new Matrix(1, 1);
        }
    }
}
