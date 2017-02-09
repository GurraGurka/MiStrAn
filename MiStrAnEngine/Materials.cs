using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiStrAnEngine
{
    public class Materials
    {
        //Make lists for E1, E2 etc
        //Only verified to MATLAB-cod with configurations: 4 laminas (all 0 degrees), 4 laminas (45,30,30,45) degrees
        public static Matrix eqModulus(double E1, double E2, double G12, double v12, double[] angle, double thickLam)
        {
            //Global matrices for the laminate
            Matrix AA = new Matrix(3, 3); //Extensional or membrane stiffness terms of a laminate
            Matrix BB = new Matrix(3, 3); //Coupling stiffness terms of a laminate
            Matrix DD = new Matrix(3, 3); //Bending stiffness n therms of a laminate

            
            int listLength = angle.Length;

            //if odd numbers of numbers
            double iterations = Math.Ceiling(listLength / 2.0);
            for (int i=0; i< iterations; i++)
            { 
                
                double v21 = v12 * E2 / E1; // Stiffness and strength analysis (Bokmärke) 4.2

                //Reduced stiffness terms (EUROCOMP eq4.50)
                double Q11 = E1 / (1 - v12 * v21);
                double Q12 = v21 * E1 / (1 - v12 * v21);
                double Q22 = E2 / (1 - v12 * v21);
                double Q66 = G12;

                //angle in degree
                double m = Math.Cos(angle[i] * 2 * Math.PI / 360); //Math.Cos uses radians
                double n = Math.Sin(angle[i] * 2 * Math.PI / 360); //Math.Cos uses radians

                //EUROCOMP, fast första är fel där (m^4) (Step 2)
                double Q_11 = Q11 * Math.Pow(m, 4) + Q22 * Math.Pow(n, 4) + Q12 * 2 * Math.Pow(m, 2) * Math.Pow(n, 2) + Q66*4 * Math.Pow(m, 2) * Math.Pow(n, 2);
                double Q_22 = Q11* Math.Pow(n, 4) + Q22 * Math.Pow(m, 4) + Q12 * 2 * Math.Pow(m, 2) * Math.Pow(n, 2) + Q66 * 4 * Math.Pow(m, 2) * Math.Pow(n, 2);
                double Q_66 = (Q11 + Q22 - 2 * Q12) * Math.Pow(m, 2) * Math.Pow(n, 2) + Q66 * Math.Pow(Math.Pow(m, 2) - Math.Pow(n, 2), 2);
                //double Q_12 = Q11 * Math.Pow(n, 4) + Q22 * Math.Pow(m, 2) * Math.Pow(n, 2) + Q12 * (Math.Pow(m, 4) + Math.Pow(n, 4)) - Q66 * 4 * Math.Pow(m, 2) * Math.Pow(n, 2);
                double Q_12 = (Q11 + Q22 - 4 * Q66) * Math.Pow(n, 2) * Math.Pow(m, 2) + Q12 * (Math.Pow(n, 4) + Math.Pow(m, 4));
                double Q_16 = Q11 * Math.Pow(m, 3) * n - Q22 * Math.Pow(n, 3) * m + Q12 * (Math.Pow(n, 3) * m - Math.Pow(m, 3) * n) + Q66 * 2 * (Math.Pow(n, 3) * m - Math.Pow(m, 3) * n);
                double Q_26 = Q11 * Math.Pow(n, 3) * m - Q22 * Math.Pow(m, 3) * n + Q12 * (Math.Pow(m, 3) * n - Math.Pow(n, 3) * m) + Q66 * 2 * (Math.Pow(m, 3) * n - Math.Pow(n, 3) * m);

                Matrix Q_ = new Matrix(new double[,] { { Q_11, Q_12, Q_16}, { Q_12, Q_22, Q_26 }, {Q_16, Q_26, Q_66} });


                //Determine distances from the mid-plane
                //Good picture in laminatedComposite PDF

                double hk =  thickLam / 2.0; //TEMP
                double hkMinus1 = 0; //-hk; //TEMP

                //Only symmetrical cases(from neutral axis)

                ///// LAMELAS DEFINED FROM MIDDLE AND OUT TOWARDS OUTER LAMELAS
                /////  BUT INPUT IS TAKING OUTER SURFACE FIRST, CHANGE THIS (ONLY MATTERS IF DIFFERENT LAMINA THICKNESS)

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

                //Add local modulus to globals (times 2 to account for both sides of neutral axes)
                AA =AA+ (hk - hkMinus1) * Q_;
                BB =BB+ (Math.Pow(hk,2) - Math.Pow(hkMinus1,2)) * Q_;
                DD =DD+ (Math.Pow(hk, 3) - Math.Pow(hkMinus1, 3)) * Q_;


            }

            
            BB = (1.0 / 2.0) * BB;
            DD = (1.0 / 3.0) * DD;

            //Step 5 i euroCOMP
         //   Matrix a = AA.Invert();

            //TEMPORARY. THEY GET SINGULAR
           // Matrix b = B.Invert();
         //   Matrix d = DD.Invert();

            //Step 6 euroCOMP
            /*   double totalThick = listLength * thickLam;
               double Ex = 1 / (totalThick * a[0, 0]);
               double Ey = 1 / (totalThick * a[1, 1]);
               double Gxy = 1 / (totalThick * a[2, 2]);
               double vxy = -a[0, 1] / a[0, 0];
               double vyx = -a[0, 1] / a[1, 1]; */
            //d and b matrices can be used for the equivalent bending elastic constants (step 6 euroCOMP)

            //Total D-matrix D=[AA -BB;-BB DD]
            Matrix D = new Matrix(6, 6);

            D[new int[]  { 0, 1, 2 } ,new int[] { 0, 1, 2 }] = AA;
            D[new int[] { 3, 4, 5 }, new int[] { 3, 4, 5 }] = DD;
            D[new int[] { 0, 1, 2 }, new int[] { 3, 4, 5 }] = -BB;
            D[new int[] { 3, 4, 5 }, new int[] { 0, 1, 2 }] = -BB;

            return D;
        }
    }
}
