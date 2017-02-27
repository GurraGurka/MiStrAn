using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiStrAnEngine
{
    public class Vector3D
    {
        public double X, Y, Z;

        public Vector3D(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Vector3D() : this(0, 0, 0) { }


        public Vector3D(Vector3D copy) : this(copy.X, copy.Y, copy.Z) { }

        public Vector3D Normalize(bool overwrite = true)
        {
            double L = this.Length;
            Vector3D v = new Vector3D();

            v.X = this.X / L;
            v.Y = this.Y / L;
            v.Z = this.Z / L;

            if (overwrite)
            {
                this.X = v.X;
                this.Y = v.Y;
                this.Z = v.Z;
            }

            return v;
        }

        public Matrix ToMatrix()
        {
            return new Matrix(new double[,] { { this.X }, { this.Y }, { this.Z }, });
        }

        public Vector ToVector()
        {
            Vector ret =  new Vector(3);
            ret[0] = X;
            ret[1] = Y;
            ret[2] = Z;

            return ret;
        }

        public static Vector3D operator +(Vector3D a, Vector3D b)
        { return new Vector3D(a.X + b.X, a.Y + b.Y, a.Z + b.Z); }
        
        public static Vector3D operator -(Vector3D a, Vector3D b)
        { return new Vector3D(a.X - b.X, a.Y - b.Y, a.Z - b.Z); }

        public static Vector3D operator *(double a, Vector3D b)
        { return new Vector3D(a * b.X, a * b.Y, a * b.Z);   }

        public static Vector3D operator *(Vector3D a, double b)
        { return b * a; }

        public static Vector3D operator -(Vector3D a)
        { return new Vector3D(-a.X, -a.Y, -a.Z); }

        public static double operator*(Vector3D a, Vector3D b)
        { return DotProduct(a, b); }

        public static Vector3D operator /(Vector3D a, double b)
        { return a*(1/b); }

        public static double DotProduct(Vector3D a, Vector3D b)
        { return a.X * b.X + a.Y * b.Y + a.Z * b.Z; }

        public static Vector3D CrossProduct(Vector3D a, Vector3D b)
        { return new Vector3D(a.Y * b.Z - a.Z * b.Y, a.Z * b.X - a.X * b.Z, a.X * b.Y - a.Y * b.X); }

        public double Length
        { get { return Math.Sqrt(Math.Pow(this.X, 2) + Math.Pow(this.Y, 2) + Math.Pow(this.Z, 2)); } }

        public static Vector3D e1
        {
            get { return new Vector3D(1, 0, 0); }
        }

        public static Vector3D e2
        {
            get { return new Vector3D(0, 1, 0); }
        }

        public static Vector3D e3
        {
            get { return new Vector3D(0, 0, 1); }
        }

        public bool IsZeroVector()
        {
            return (X == 0 && Y == 0 && Z == 0);

        }

        public static Vector3D ZeroVector
        {
            get { return new Vector3D(); }
        }


    }
}
