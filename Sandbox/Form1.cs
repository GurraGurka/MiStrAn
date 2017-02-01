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
            Matrix dofPlan = new Matrix(new double[,] { { 1,2,3 }, {4,5,6}, { 7, 8, 9 } });

            Matrix test = Matrix.MergeHorizontal(dofPlan,dofPlan,dofPlan);


            dofPlan = dofPlan;
        }
    }
}
