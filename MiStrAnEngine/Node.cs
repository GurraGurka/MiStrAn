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
                dofX = (id + 1) * 6 - 6;
                dofY = (id + 1) * 6 - 5;
                dofZ = (id + 1) * 6 - 4;
                dofXX = (id + 1) * 6 - 3;
                dofYY = (id + 1) * 6 - 2;
                dofZZ = (id + 1) * 6 - 1;
            }
        }

        // Constructors
        public Node(double _x, double _y, double _z, int _id)
        {
            x = _x;
            y = _y;
            z = _z;

            Id = _id;
        }


        // Sets id to -1
        public Node(double _x, double _y, double _z) : this(_x, _y, _z, -1)
        { }




    }
}
