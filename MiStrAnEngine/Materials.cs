using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiStrAnEngine
{
    public class Materials
    {
        //Make lists for E1, E2 etc
        public static Matrix eqModulus(double E1, double E2, double G12, double v12, double angle, double thickLam)
        {
            //Global matrices for the laminate
            Matrix A = new Matrix(6, 6); //Extensional or membrane stiffness terms of a laminate
            Matrix B = new Matrix(6, 6); //Coupling stiffness terms of a laminate
            Matrix D = new Matrix(6, 6); //Bending stiffness n therms of a laminate

            //Just loop through 4 times now (otherwise listlength)
            int listLength = 4;
            for (int i=0; i< listLength; i++)
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

                //EUROCOMP, fast första är fel där (m^4) (Step 2)
                double Q_11 = Q11 * Math.Pow(m, 4) + Q22 * Math.Pow(n, 4) + Q12 * 2 * Math.Pow(m, 2) * Math.Pow(n, 2) + Q66*4 * Math.Pow(m, 2) * Math.Pow(n, 2);
                double Q_22 = Q11* Math.Pow(n, 4) + Q22 * Math.Pow(m, 4) + Q12 * 2 * Math.Pow(m, 2) * Math.Pow(n, 2) + Q66 * 4 * Math.Pow(m, 2) * Math.Pow(n, 2);
                double Q_12 = (Q11 + Q22 - 2 * Q12) * Math.Pow(m, 2) * Math.Pow(n, 2) + Q66 * Math.Pow(Math.Pow(m, 2) - Math.Pow(n, 2), 2);
                double Q_66 = Q11 * Math.Pow(n, 4) + Q22 * Math.Pow(m, 2) * Math.Pow(n, 2) + Q12 * (Math.Pow(m, 4) + Math.Pow(n, 4)) - Q66 * 4 * Math.Pow(m, 2) * Math.Pow(n, 2);
                double Q_16 = Q11 * Math.Pow(m, 3) * n - Q22 * Math.Pow(n, 3) * m + Q12 * (Math.Pow(n, 3) * m - Math.Pow(m, 3) * n) + Q66 * 2 * (Math.Pow(n, 3) * m - Math.Pow(m, 3) * n);
                double Q_26 = Q11 * Math.Pow(n, 3) * m - Q22 * Math.Pow(m, 3) * n + Q12 * (Math.Pow(m, 3) * n - Math.Pow(n, 3) * m) + Q66 * 2 * (Math.Pow(m, 3) * n - Math.Pow(n, 3) * m);

                Matrix Q_ = new Matrix(new double[,] { { Q_11, Q_12, Q_16}, { Q_12, Q_22, Q_26 }, {Q_16, Q_26, Q_66} });
               
                
                //Determine distances from the mid-plane
                //Good picture in laminatedComposite PDF

                double hk = 0;
                double hkMinus1 = 0;

                //Only symmetrical cases(from neutral axis)

                ///// LAMELAS DEFINED FROM MIDDLE AND OUT TOWARDS OUTER LAMELAS
                /////  IF listLength = 4 LAMELAS =8, WILL CHANGE TO BETTER WHEN WE DETERMINE HOW TO HAVE IT

                //even
                if (listLength % 2 == 0)
                {
                    hk = (i +1)*thickLam;
                    hkMinus1 = i  * thickLam;
                }
                //odd numbers of laminas
                else if (i > 0)
                {
                    hk = i * thickLam + 0.5 * thickLam;
                    hkMinus1 = (i - 1) * thickLam + 0.5 * thickLam;
                }

                //Add local modulus to globals
                A = (hk - hkMinus1) * A;
                B = (Math.Pow(hk,2) - Math.Pow(hkMinus1,2)) * B;
                D = (Math.Pow(hk, 3) - Math.Pow(hkMinus1, 3)) *D;


            }

            //Detta kanske kan göras i loopen men orkade inte tänka nu
            B = (1 / 2) * B;
            D = (1 / 3) * D;

            //Step 5 i euroCOMP
            Matrix a = A.Invert();
            Matrix b = B.Invert();
            Matrix d = D.Invert();

            //Step 6 euroCOMP
            double totalThick = listLength * thickLam;
            double Ex = 1 / (totalThick * a[1, 1]);
            double Ey = 1 / (totalThick * a[2, 2]);
            double Gxy = 1 / (totalThick * a[6, 6]);
            double vxy = -a[1, 2] / a[1, 1];
            double vyx = -a[1, 2] / a[2, 2];

            //d and b matrices can be used for the equivalent bending elastic constants (step 6 euroCOMP)

            return new Matrix(1, 1);
        }
    }
}
