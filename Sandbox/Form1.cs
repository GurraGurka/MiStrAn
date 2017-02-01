using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
            Matrix dofPlan = new Matrix(new double[,] { { 1, 2, 6, 7, 11, 12, 16, 17 }, { 1, 2, 6, 7, 11, 12, 16, 18 } });

            Matrix test = dofPlan["1:2", "1:3"];

            test = test;

            dofPlan["1:2", "1:2"] = new Matrix(new double[,] { { 0, 0 }, { 0, 0 } });

            dofPlan = dofPlan;
        }
    }
}
