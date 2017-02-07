using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SF = MiStrAnEngine.StaticFunctions;
using mat = MiStrAnEngine.Materials;
using MiStrAnEngine;

namespace Sandbox
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // shell
            Node node1 = new Node(1, 1, 0);
            Node node2 = new Node(3, 2, 1);
            Node node3 = new Node(1, 4, 1);

            List<Node> nodeList = new List<Node>() { node1, node2, node3 };

            ShellElement element = new ShellElement(nodeList, 1);
            element.thickness = 1;

            Matrix Ke, fe;

            element.ShellTesting();

            //element.GetLocalNodeCoordinates();
            //element.TestingShell2();

            //element.D = new Matrix(new double[,] { { 1, 2, 3 }, { 2, 2, 2 }, { 3, 2, 1 } });
            //element.t = 2;
            //element.eq = new Matrix(new double[,] { { 5,5,5} });


            //Matrix Ke;
            //Matrix fe;

            //element.GenerateKefe(out Ke, out fe);

            //Ke = Ke;

            //Solveq test case

            //Matrix K = new Matrix(new double[,] { { 2, -1 }, { -1, 2 } });
            //Matrix f = new Matrix(new double[,] { { 0}, { 10 } });
            //Matrix bc = new Matrix(new double[,] { { 0 , 0 } });

            //Matrix d, Q;

            //SF.solveq(K, f, bc, out d, out Q);

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            double E1 = 33.18; //GPa
            double E2 = 7.74; //GPA
            double G12 = 2.91; //GPa
            double v12 = 0.267;
            double[] angle = new double[4] { 45,30, 30, 45};
            double laminaThickness = 0.004; 
            mat.eqModulus(E1, E2, G12, v12, angle, laminaThickness);
        }
    }
}
