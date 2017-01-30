using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiStrAnEngine
{
    public class Node
    {

        // Field variables
        public double x;
        public double y;
        public double z;

        public int dofX;
        public int dofY;
        public int dofZ;
        public int dofXX;
        public int dofYY;
        public int dofZZ;


        private int id;

        // Gets and sets

        public int Id
        {
            get
            {
                return id;
            }


            set
            {
                id = value;
                dofX = id * 6 - 5;
                dofY = id * 6 - 4;
                dofZ = id * 6 - 3;
                dofXX = id * 6 - 2;
                dofYY = id * 6 - 1;
                dofZZ = id * 6;
            }
        }

        // Constructors
        public Node(double _x, double _y, double _z, int _id)
        {
            x = _x;
            y = _y;
            z = _z;

            id = _id;
        }


        // Sets id to -1
        public Node(double _x, double _y, double _z) : this(_x, _y, _z, -1)
        { }




    }
}
