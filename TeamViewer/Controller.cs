using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TeamViewer
{
    public partial class Controller : Form
    {
        public Controller()
        {
            InitializeComponent();
        }

        private void Controller_Load(object sender, EventArgs e)
        {
            MainForm mainForm = new MainForm();
            mainForm.Show();
            mainForm = new MainForm();
            mainForm.Show();
        }
    }
}
