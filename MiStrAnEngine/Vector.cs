using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiStrAnEngine
{
    public class Vector
    {
        public double[] values;

        public Vector(int length)
        {
            values = new double[length];

        }

        public Vector(Vector copy) : this(copy.Length)  
        {
            copy.values.CopyTo(values, 0);
        }

        public double Norm
        {
            get { return values.Sum(x => Math.Pow(x, 2)); }

        }

        public double this[int i]
        {
            get { return values[i]; }

            set { values[i] = value; }

        }

        public int Length
        { get { return values.Length; } }

        public Matrix ToMatrix()
        {
            Matrix ret = new Matrix(Length, 1);

            for (int i = 0; i < Length; i++)
            {
                ret[i, 0] = values[i];
            }
            return ret;
        }

        private static Vector Multiply(double a, Vector v)
        {
            Vector ret = new Vector(v.Length);

            for (int i = 0; i < v.Length; i++)
            {
                ret[i] = v[i] * a;
            }

            return ret;
        }

        private static Vector Add(Vector a, Vector b)
        {
            if (a.Length != b.Length) throw new MException("Vectors must share length");

            Vector ret = new Vector(a.Length);
            for (int i = 0; i < a.Length; i++)
            {
                ret[i] = a[i] + b[i];
            }

            return ret;
        }

        private static double ScalarProduct(Vector a, Vector b)
        {
            if (a.Length != b.Length) throw new MException("Vectors must share length");

            double sum = 0;

            for (int i = 0; i < a.Length; i++)
            {
                sum += a[i] * b[i];
            }

            return sum;
        }

        //   O P E R A T O R S

        public static Vector operator -(Vector v)
        { return Vector.Multiply(-1, v); }

        public static Vector operator +(Vector m1, Vector m2)
        { return Vector.Add(m1, m2); }

        public static Vector operator -(Vector m1, Vector m2)
        { return Vector.Add(m1, -m2); }

        public static double operator *(Vector m1, Vector m2)
        { return Vector.ScalarProduct(m1, m2); }

        public static Vector operator *(double n, Vector m)
        { return Vector.Multiply(n, m); }

    }
}
