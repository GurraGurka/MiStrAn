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
            // Platre test case
            Node node1 = new Node(0, 0, 0);
            Node node2 = new Node(2, 0, 0);
            Node node3 = new Node(2, 1, 0);
            Node node4 = new Node(0, 1, 0);

            List<Node> nodeList = new List<Node>() { node1, node2, node3, node4 };

            ShellElement element = new ShellElement(nodeList, 1);

            element.D = new Matrix(new double[,] { { 1, 2, 3 }, { 2, 2, 2 }, { 3, 2, 1 } });
            element.t = 2;
            element.eq = 5;


            Matrix Ke;
            Matrix fe;

            element.GenerateKefe(out Ke, out fe);

            Ke = Ke;
        }
    }
}
